using System.Collections.Immutable;
using System.Text;
using HotChocolate.Types.Analyzers.Helpers;
using HotChocolate.Types.Analyzers.Inspectors;
using HotChocolate.Types.Analyzers.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using static HotChocolate.Types.Analyzers.Helpers.GeneratorUtils;

namespace HotChocolate.Types.Analyzers.FileBuilders;

public sealed class DataLoaderFileBuilder : IDisposable
{
    private StringBuilder _sb;
    private CodeWriter _writer;
    private bool _disposed;

    public DataLoaderFileBuilder()
    {
        _sb = PooledObjects.GetStringBuilder();
        _writer = new CodeWriter(_sb);
    }

    public void WriteHeader()
    {
        _writer.WriteIndentedLine("// <auto-generated/>");
        _writer.WriteLine();
        _writer.WriteIndentedLine("#nullable enable");
        _writer.WriteIndentedLine("#pragma warning disable");
        _writer.WriteLine();
        _writer.WriteIndentedLine("using System;");
        _writer.WriteIndentedLine("using System.Runtime.CompilerServices;");
        _writer.WriteIndentedLine("using Microsoft.Extensions.DependencyInjection;");
        _writer.WriteIndentedLine("using GreenDonut;");
        _writer.WriteLine();
    }

    public void WriteBeginNamespace(string ns)
    {
        _writer.WriteIndentedLine("namespace {0}", ns);
        _writer.WriteIndentedLine("{");
        _writer.IncreaseIndent();
    }

    public void WriteEndNamespace()
    {
        _writer.DecreaseIndent();
        _writer.WriteIndentedLine("}");
        _writer.WriteLine();
    }

    public void WriteDataLoaderInterface(
        string name,
        bool isPublic,
        DataLoaderKind kind,
        ITypeSymbol key,
        ITypeSymbol value)
    {
        _writer.WriteIndentedLine(
            "{0} interface {1}",
            isPublic
                ? "public"
                : "internal",
            name);
        _writer.IncreaseIndent();

        _writer.WriteIndentedLine(
            kind is DataLoaderKind.Group
                ? ": global::GreenDonut.IDataLoader<{0}, {1}[]>"
                : ": global::GreenDonut.IDataLoader<{0}, {1}>",
            key.ToFullyQualified(),
            value.ToFullyQualified());

        _writer.DecreaseIndent();
        _writer.WriteIndentedLine("{");
        _writer.WriteIndentedLine("}");
        _writer.WriteLine();
    }

    public void WriteBeginDataLoaderClass(
        string name,
        string interfaceName,
        bool isPublic,
        DataLoaderKind kind,
        ITypeSymbol key,
        ITypeSymbol value)
    {
        _writer.WriteIndentedLine(
            "{0} sealed class {1}",
            isPublic
                ? "public"
                : "internal",
            name);
        _writer.IncreaseIndent();
        _writer.WriteIndentedLine(
            kind is DataLoaderKind.Group
                ? ": global::GreenDonut.DataLoaderBase<{0}, {1}[]>"
                : ": global::GreenDonut.DataLoaderBase<{0}, {1}>",
            key.ToFullyQualified(),
            value.ToFullyQualified());
        _writer.WriteIndentedLine(", {0}", interfaceName);
        _writer.DecreaseIndent();
        _writer.WriteIndentedLine("{");
        _writer.IncreaseIndent();
    }

    public void WriteEndDataLoaderClass()
    {
        _writer.DecreaseIndent();
        _writer.WriteIndentedLine("}");
    }

    public void WriteDataLoaderConstructor(
        string name,
        DataLoaderKind kind,
        ITypeSymbol keyType,
        ITypeSymbol valueType,
        ImmutableArray<CacheLookup> lookupMethods)
    {
        _writer.WriteIndentedLine("private readonly global::System.IServiceProvider _services;");
        _writer.WriteLine();

        if (kind is DataLoaderKind.Batch or DataLoaderKind.Group)
        {
            _writer.WriteIndentedLine("public {0}(", name);

            using (_writer.IncreaseIndent())
            {
                _writer.WriteIndentedLine("global::System.IServiceProvider services,");
                _writer.WriteIndentedLine("global::GreenDonut.IBatchScheduler batchScheduler,");
                _writer.WriteIndentedLine("global::GreenDonut.DataLoaderOptions options)");
                _writer.WriteIndentedLine(": base(batchScheduler, options)");
            }
        }
        else
        {
            _writer.WriteIndentedLine("public {0}(", name);

            using (_writer.IncreaseIndent())
            {
                _writer.WriteIndentedLine("global::System.IServiceProvider services,");
                _writer.WriteIndentedLine("global::GreenDonut.DataLoaderOptions options)");
                _writer.WriteIndentedLine(": base(AutoBatchScheduler.Default, options)");
            }
        }

        _writer.WriteIndentedLine("{");

        using (_writer.IncreaseIndent())
        {
            _writer.WriteIndentedLine("_services = services ??");

            using (_writer.IncreaseIndent())
            {
                _writer.WriteIndentedLine("throw new global::System.ArgumentNullException(nameof(services));");
            }

            if (lookupMethods.Length > 0)
            {
                _writer.WriteLine();

                foreach (var lookup in lookupMethods)
                {
                    _writer.WriteIndentedLine(
                        "global::{0}",
                        WellKnownTypes.PromiseCacheObserver);

                    using (_writer.IncreaseIndent())
                    {
                        if (lookup.IsTransform)
                        {
                            _writer.WriteIndentedLine(
                                ".Create<{0}, {1}{2}, {3}{4}>({5}.{6}, this)",
                                keyType.ToFullyQualified(),
                                valueType.ToFullyQualified(),
                                valueType.PrintNullRefQualifier(),
                                lookup.Method.Parameters[0].Type.ToFullyQualified(),
                                lookup.Method.Parameters[0].Type.PrintNullRefQualifier(),
                                lookup.Method.ContainingType.ToFullyQualified(),
                                lookup.Method.Name);
                        }
                        else
                        {
                            _writer.WriteIndentedLine(
                                ".Create<{0}, {1}{2}>({3}.{4}, this)",
                                keyType.ToFullyQualified(),
                                valueType.ToFullyQualified(),
                                valueType.PrintNullRefQualifier(),
                                lookup.Method.ContainingType.ToFullyQualified(),
                                lookup.Method.Name);
                        }

                        _writer.WriteIndentedLine(".Accept(this);");
                    }
                }
            }
        }

        _writer.WriteIndentedLine("}");
    }

    public void WriteDataLoaderLoadMethod(
        string containingType,
        IMethodSymbol method,
        bool isScoped,
        DataLoaderKind kind,
        ITypeSymbol key,
        ITypeSymbol value,
        ImmutableArray<DataLoaderParameterInfo> parameters)
    {
        _writer.WriteIndentedLine(
            "protected override async global::{0} FetchAsync(",
            WellKnownTypes.ValueTask);

        using (_writer.IncreaseIndent())
        {
            _writer.WriteIndentedLine(
                "global::{0}<{1}> keys,",
                WellKnownTypes.ReadOnlyList,
                key.ToFullyQualified());
            _writer.WriteIndentedLine(
                "global::{0}<{1}<{2}{3}{4}>> results,",
                WellKnownTypes.Memory,
                WellKnownTypes.Result,
                value.ToFullyQualified(),
                kind is DataLoaderKind.Group ? "[]" : string.Empty,
                value.IsValueType ? string.Empty : "?");
                _writer.WriteIndentedLine(
            "global::{0}<{1}{2}> context,",
                WellKnownTypes.DataLoaderFetchContext,
                value.ToFullyQualified(),
                kind is DataLoaderKind.Group ? "[]" : string.Empty);
            _writer.WriteIndentedLine(
                "global::{0} ct)",
                WellKnownTypes.CancellationToken);
        }

        _writer.WriteIndentedLine("{");

        using (_writer.IncreaseIndent())
        {
            if (isScoped && parameters.Any(p => p.Kind == DataLoaderParameterKind.Service))
            {
                _writer.WriteIndentedLine("await using var scope = _services.CreateAsyncScope();");
            }

            foreach (var parameter in parameters)
            {
                if (parameter.Kind is DataLoaderParameterKind.Service)
                {
                    _writer.WriteIndentedLine(
                        "var {0} = {1}.GetRequiredService<{2}>();",
                        parameter.VariableName,
                        isScoped ? "scope.ServiceProvider" : "_services",
                        parameter.Type.ToFullyQualified());
                }
                else if (parameter.Kind is DataLoaderParameterKind.SelectorBuilder
                    || parameter.Kind is DataLoaderParameterKind.PagingArguments)
                {
                    _writer.WriteIndentedLine(
                        "var {0} = context.GetRequiredState<{1}>(\"{2}\");",
                        parameter.VariableName,
                        parameter.Type.ToFullyQualified(),
                        parameter.StateKey);
                }
                else if (parameter.Kind is DataLoaderParameterKind.ContextData)
                {
                    if (parameter.Parameter.HasExplicitDefaultValue)
                    {
                        var defaultValue = parameter.Parameter.ExplicitDefaultValue;
                        var defaultValueString = ConvertDefaultValueToString(defaultValue, parameter.Type);

                        _writer.WriteIndentedLine(
                            "var {0} = context.GetStateOrDefault<{1}{2}>(\"{3}\", {4});",
                            parameter.VariableName,
                            parameter.Type.ToFullyQualified(),
                            parameter.Type.PrintNullRefQualifier(),
                            parameter.StateKey,
                            defaultValueString);

                    }
                    else if (parameter.Type.IsNullableType())
                    {
                        _writer.WriteIndentedLine(
                            "var {0} = context.GetState<{1}{2}>(\"{3}\");",
                            parameter.VariableName,
                            parameter.Type.ToFullyQualified(),
                            parameter.Type.PrintNullRefQualifier(),
                            parameter.StateKey);
                    }
                    else
                    {
                        _writer.WriteIndentedLine(
                            "var {0} = context.GetRequiredState<{1}>(\"{2}\");",
                            parameter.VariableName,
                            parameter.Type.ToFullyQualified(),
                            parameter.StateKey);
                    }
                }
            }

            if (kind is DataLoaderKind.Cache)
            {
                _writer.WriteIndentedLine("for (var i = 0; i < keys.Count; i++)");
                _writer.WriteIndentedLine("{");

                using (_writer.IncreaseIndent())
                {
                    _writer.WriteIndentedLine("try");
                    _writer.WriteIndentedLine("{");

                    using (_writer.IncreaseIndent())
                    {
                        _writer.WriteIndentedLine("var key = keys[i];");
                        _writer.WriteIndented("var value = ");
                        WriteFetchCall(method, containingType, kind, parameters);
                        _writer.WriteIndentedLine(
                            "results.Span[i] = Result<{0}{1}>.Resolve(value);",
                            value.ToFullyQualified(),
                            value.IsValueType ? string.Empty : "?");
                    }

                    _writer.WriteIndentedLine("}");
                    _writer.WriteIndentedLine("catch (global::System.Exception ex)");
                    _writer.WriteIndentedLine("{");

                    using (_writer.IncreaseIndent())
                    {
                        _writer.WriteIndentedLine(
                            "results.Span[i] = Result<{0}{1}>.Reject(ex);",
                            value.ToFullyQualified(),
                            value.IsValueType ? string.Empty : "?");
                    }

                    _writer.WriteIndentedLine("}");
                }

                _writer.WriteIndentedLine("}");
            }
            else
            {
                _writer.WriteIndented("var temp = ");
                WriteFetchCall(method, containingType, kind, parameters);
                _writer.WriteIndentedLine("CopyResults(keys, results.Span, temp);");
            }
        }

        _writer.WriteIndentedLine("}");

        if (kind is DataLoaderKind.Cache)
        {
            return;
        }

        _writer.WriteLine();
        _writer.WriteIndentedLine("private void CopyResults(");
        using (_writer.IncreaseIndent())
        {
            _writer.WriteIndentedLine(
                "global::{0}<{1}> keys,",
                WellKnownTypes.ReadOnlyList,
                key.ToFullyQualified());
            _writer.WriteIndentedLine(
                "global::{0}<{1}<{2}{3}{4}>> results,",
                WellKnownTypes.Span,
                WellKnownTypes.Result,
                value.ToFullyQualified(),
                kind is DataLoaderKind.Group ? "[]" : string.Empty,
                value.IsValueType ? string.Empty : "?");
            _writer.WriteIndentedLine(
                "global::{0} resultMap)",
                ExtractMapType(method.ReturnType));
        }

        _writer.WriteIndentedLine("{");
        using (_writer.IncreaseIndent())
        {
            _writer.WriteIndentedLine("for (var i = 0; i < keys.Count; i++)");
            _writer.WriteIndentedLine("{");
            using (_writer.IncreaseIndent())
            {
                _writer.WriteIndentedLine("var key = keys[i];");
                if (kind is DataLoaderKind.Group)
                {
                    _writer.WriteIndentedLine("if (resultMap.Contains(key))");
                    _writer.WriteIndentedLine("{");

                    using (_writer.IncreaseIndent())
                    {
                        _writer.WriteIndentedLine(
                            "var items = resultMap[key];");
                        _writer.WriteIndentedLine(
                            "results[i] = global::{0}<{1}{2}[]?>.Resolve(global::{3}.ToArray(items));",
                            WellKnownTypes.Result,
                            value.ToFullyQualified(),
                            value.PrintNullRefQualifier(),
                            WellKnownTypes.EnumerableExtensions);
                    }

                    _writer.WriteIndentedLine("}");
                    _writer.WriteIndentedLine("else");
                    _writer.WriteIndentedLine("{");

                    using (_writer.IncreaseIndent())
                    {
                        _writer.WriteIndentedLine(
                            "results[i] = global::{0}<{1}{2}[]?>.Resolve(global::{3}.Empty<{1}{2}>());",
                            WellKnownTypes.Result,
                            value.ToFullyQualified(),
                            value.PrintNullRefQualifier(),
                            WellKnownTypes.Array);
                    }

                    _writer.WriteIndentedLine("}");
                }
                else
                {
                    _writer.WriteIndentedLine("if (resultMap.TryGetValue(key, out var value))");
                    _writer.WriteIndentedLine("{");

                    using (_writer.IncreaseIndent())
                    {
                        _writer.WriteIndentedLine(
                            "results[i] = global::{0}<{1}{2}>.Resolve(value);",
                            WellKnownTypes.Result,
                            value.ToFullyQualified(),
                            value.IsValueType ? string.Empty : "?");
                    }

                    _writer.WriteIndentedLine("}");
                    _writer.WriteIndentedLine("else");
                    _writer.WriteIndentedLine("{");

                    using (_writer.IncreaseIndent())
                    {
                        _writer.WriteIndentedLine(
                            "results[i] = global::{0}<{1}{2}>.Resolve(default({3}));",
                            WellKnownTypes.Result,
                            value.ToFullyQualified(),
                            value.IsValueType ? string.Empty : "?",
                            value.ToFullyQualified());
                    }

                    _writer.WriteIndentedLine("}");
                }
            }

            _writer.WriteIndentedLine("}");
        }

        _writer.WriteIndentedLine("}");
    }

    private void WriteFetchCall(
        IMethodSymbol fetchMethod,
        string containingType,
        DataLoaderKind kind,
        ImmutableArray<DataLoaderParameterInfo> parameters)
    {
        _writer.Write("await {0}.{1}(", containingType, fetchMethod.Name);

        for (var i = 0; i < parameters.Length; i++)
        {
            if (i > 0)
            {
                _writer.Write(", ");
            }

            var parameter = parameters[i];

            if (i == 0)
            {
                _writer.Write(
                    kind is DataLoaderKind.Cache
                        ? "key"
                        : "keys");
            }
            else
            {
                _writer.Write(parameter.VariableName);
            }
        }

        _writer.WriteLine(").ConfigureAwait(false);");
    }

    public void WriteLine() => _writer.WriteLine();

    private static ITypeSymbol ExtractMapType(ITypeSymbol returnType)
    {
        if (returnType is INamedTypeSymbol { TypeArguments.Length: 1, } namedType
            && namedType.TypeArguments[0] is INamedTypeSymbol { TypeArguments.Length: 2 } dict)
        {
            return dict;
        }

        throw new InvalidOperationException();
    }

    public override string ToString()
        => _sb.ToString();

    public SourceText ToSourceText()
        => SourceText.From(ToString(), Encoding.UTF8);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        PooledObjects.Return(_sb);
        _sb = default!;
        _writer = default!;
        _disposed = true;
    }
}
