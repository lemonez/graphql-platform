using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace HotChocolate.Execution.Processing
{
    /// <summary>A task scheduler that allow tracking if work is still in progress</summary>
    /// <remarks>Based on code from QueuedTaskScheduler and ConcurrentExclusiveSchedulerPair</remarks>
    internal class TrackableTaskScheduler : TaskScheduler
    {
        private readonly object _lock = new();
        /// <summary>The queue holding pending tasks</summary>
        private readonly Channel<Task> _queue = Channel.CreateUnbounded<Task>(new UnboundedChannelOptions {
            AllowSynchronousContinuations = true,
            SingleReader = false,
            SingleWriter = false
        });
        /// <summary>The underlying task scheduler to which all work should be scheduled.</summary>
        private readonly TaskScheduler _underlyingScheduler;
        /// <summary>The maximum number of tasks processors that can be active at the same time.</summary>
        private readonly int _processingTaskMax = Environment.ProcessorCount;
        /// <summary>The amount of time in milliseconds a single processing task can keep the underlying scheduler occupied</summary>
        /// <remarks>If a task takes longer it will not quit, it will just result in a new processing task after it completes</remarks>
        private readonly int _processingTaskTimeout = 50;
        /// <summary>The number of tasks processors that are currently processing a task.</summary>
        private int _processingTaskCount = 0;
        /// <summary>If > 0, no more work will be accepted and the active processing tasks will stop as soon as possible</summary>
        private CancellationTokenSource _shutdown = new();
        /// <summary>This event is triggered whenever the processing comes to a complete stop (no items left in queue, and nothing running)</summary>
        private event EventHandler? ProcessingHalted;

        /// <summary>Create a new instance that uses the current scheduler to perform the actual work</summary>
        public TrackableTaskScheduler(TaskScheduler underlyingScheduler)
        {
            _underlyingScheduler = underlyingScheduler;
            StartupProcessors();
        }

        /// <returns>true if the scheduler has no remaining work (either on queue or in progress)</returns>
        public bool IsIdle
        {
            get
            {
                lock(_lock)
                {
                    return _processingTaskCount == 0;
                }
            }
        }

        public async Task WaitTillIdle(CancellationToken? ctx = null)
        {
            TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>();
            CancellationTokenRegistration? ctxRegistration = ctx?.Register(() => completion.TrySetCanceled());
            EventHandler completionHandler = (source, args) =>
            {
                try
                {
                    if (ctx?.IsCancellationRequested ?? false)
                    {
                        completion.TrySetCanceled();
                    }
                    else
                    {
                        completion.TrySetResult(true);
                    }
                }
                catch (Exception e)
                {
                    completion.TrySetException(e);
                }
            };

            lock (_lock)
            {
                ProcessingHalted += completionHandler;

                if (ctx?.IsCancellationRequested ?? false)
                {
                    completion.TrySetCanceled();
                }
                else if (_processingTaskCount == 0)
                {
                    completion.TrySetResult(true);
                }
            }

            try
            {
                await completion.Task.ConfigureAwait(false);
            }
            finally
            {
                ctxRegistration?.Dispose();
                lock (_lock)
                {
                    ProcessingHalted -= completionHandler;
                }
            }
        }

        /// <summary>Mark that no future work should be handled by this scheduler (will stop all processing tasks as soon as possible)
        /// 
        /// </summary>
        public void Complete()
        {
            _queue.Writer.Complete();
            _shutdown.Cancel();
        }

        protected override IEnumerable<Task>? GetScheduledTasks()
        {
            // for debugging only, try to use internal member of UnboundedQueue
            var itemsField = _queue.GetType()
                .GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
            return itemsField?.GetValue(_queue) as IEnumerable<Task> ?? new List<Task>();
        }

        protected override void QueueTask(Task task)
        {
            Debug.Assert(task != null, "Infrastructure should have provided a non-null task.");
            Debug.Assert(!_shutdown.IsCancellationRequested, "After completion no more work should be scheduled");
            _queue.Writer.WriteAsync(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return SafeExecuteTask(task);
        }

        /// <remarks>This method assumes that the caller takes a lock on _lock</remarks>
        private void StartupProcessors()
        {
            for(int i = 0; i < _processingTaskMax; ++i)
            {
                var options = TaskCreationOptions.DenyChildAttach | TaskCreationOptions.PreferFairness;
                Task.Factory.StartNew(() => ProcessTasks(), default, options, _underlyingScheduler);
            }
        }

        private async ValueTask ProcessTasks()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                try
                {
                    var ctxSource = CancellationTokenSource.CreateLinkedTokenSource(_shutdown.Token);
                    ctxSource.CancelAfter(_processingTaskTimeout);

                    Task task = await _queue.Reader.ReadAsync(ctxSource.Token);
                    Interlocked.Increment(ref _processingTaskCount);
                    try
                    {
                        SafeExecuteTask(task);
                        while (!ctxSource.IsCancellationRequested && _queue.Reader.TryRead(out task))
                        {
                            SafeExecuteTask(task);
                        }
                    }
                    finally
                    {
                        var value = Interlocked.Decrement(ref _processingTaskCount);
                        if (value < 0)
                        {
                            Debug.Fail("Inconstistent processing task count, investigate why this is happening");
                            value = Interlocked.CompareExchange(ref _processingTaskCount, 0, value);
                        }
                        if (value == 0)
                        {
                            lock (_lock)
                            {
                                ProcessingHalted?.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }
                    
                    if (!_shutdown.IsCancellationRequested)
                    {
                         // yield to give other tasks a chance to run fairly as well 
                         await Task.Yield();
                    }
                }
                catch(TaskCanceledException)
                {
                    // expected, ignore
                }
                catch(Exception e)
                {
                    Debug.Fail($"unexpected exception: {e}");
                }
            }
        }

        private bool SafeExecuteTask(Task task)
        {
            // Execute the task. If the scheduler was previously faulted,
            // this task could have been faulted when it was queued; ignore such tasks.
            bool result;
            if (task.IsCompleted)
            {
                result = true;
            }
            else
            {
                result = TryExecuteTask(task);
                Debug.Assert(task.IsCompleted, "After TaskScheduler.TryExecuteTask the task should be completed");
            }
            return result;
        }
    }
}
