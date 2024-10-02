using System.Collections.Immutable;
using CookieCrumble;
using GreenDonut;
using GreenDonut.Projections;
using HotChocolate.Data.TestContext;
using HotChocolate.Execution;
using HotChocolate.Execution.Processing;
using HotChocolate.Pagination;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Squadron;

namespace HotChocolate.Data;

[Collection(PostgresCacheCollectionFixture.DefinitionName)]
public class InterfaceIntegrationTests(PostgreSqlResource resource)
{
    public PostgreSqlResource Resource { get; } = resource;

    private string CreateConnectionString()
        => Resource.GetConnectionString($"db_{Guid.NewGuid():N}");

    [Fact]
    public async Task Query_Owner_Animals()
    {
        var connectionString = CreateConnectionString();
        await SeedAsync(connectionString);

        var queries = new List<QueryInfo>();

        var result = await new ServiceCollection()
            .AddScoped(_ => new AnimalContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddTypeExtension(typeof(OwnerExtensions))
            .AddDataLoader<AnimalsByOwnerDataLoader>()
            .AddObjectType<Cat>()
            .AddObjectType<Dog>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                OperationRequestBuilder.New()
                    .SetDocument(
                        """
                        {
                            owners(first: 10) {
                                nodes {
                                    id
                                    name
                                    pets(first: 10) {
                                        nodes {
                                            __typename
                                            id
                                            name
                                        }
                                    }
                                }
                            }
                        }
                        """)
                    .AddQueries(queries)
                    .Build());

        var operationResult = result.ExpectOperationResult();

        await Snapshot.Create()
            .AddQueries(queries)
            .Add(operationResult.WithExtensions(ImmutableDictionary<string, object?>.Empty))
            .MatchMarkdownAsync();
    }

    [Fact]
    public async Task Query_Owner_Animals_With_Fragments()
    {
        var connectionString = CreateConnectionString();
        await SeedAsync(connectionString);

        var queries = new List<QueryInfo>();

        var result = await new ServiceCollection()
            .AddScoped(_ => new AnimalContext(connectionString))
            .AddGraphQL()
            .AddQueryType<Query>()
            .AddTypeExtension(typeof(OwnerExtensions))
            .AddDataLoader<AnimalsByOwnerDataLoader>()
            .AddObjectType<Cat>()
            .AddObjectType<Dog>()
            .AddPagingArguments()
            .ModifyRequestOptions(o => o.IncludeExceptionDetails = true)
            .ExecuteRequestAsync(
                OperationRequestBuilder.New()
                    .SetDocument(
                        """
                        {
                            owners(first: 10) {
                                nodes {
                                    id
                                    name
                                    pets(first: 10) {
                                        nodes {
                                            __typename
                                            id
                                            name
                                            ... on Dog {
                                                isBarking
                                            }
                                            ... on Cat {
                                                isPurring
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        """)
                    .AddQueries(queries)
                    .Build());

        var operationResult = result.ExpectOperationResult();

        await Snapshot.Create()
            .AddQueries(queries)
            .Add(operationResult.WithExtensions(ImmutableDictionary<string, object?>.Empty))
            .MatchMarkdownAsync();
    }


    private static async Task SeedAsync(string connectionString)
    {
        await using var context = new AnimalContext(connectionString);
        await context.Database.EnsureCreatedAsync();

        var owners = new List<Owner>
        {
            new Owner
            {
                Name = "Owner 1",
                Pets =
                [
                    new Cat { Name = "Cat 1" },
                    new Dog { Name = "Dog 1", IsBarking = true },
                    new Dog { Name = "Dog 2", IsBarking = false }
                ]
            },
            new Owner
            {
                Name = "Owner 2",
                Pets =
                [
                    new Cat { Name = "Cat 2" },
                    new Dog { Name = "Dog 3", IsBarking = true },
                    new Dog { Name = "Dog 4", IsBarking = false }
                ]
            },
            new Owner
            {
                Name = "Owner 3",
                Pets =
                [
                    new Cat { Name = "Cat 3 (Not Pure)", IsPurring = true },
                    new Dog { Name = "Dog 5", IsBarking = true },
                    new Dog { Name = "Dog 6", IsBarking = false }
                ]
            },
            new Owner
            {
                Name = "Owner 4 - No Pets"
            },
            new Owner
            {
                Name = "Owner 5 - Only Cat",
                Pets = [new Cat { Name = "Only Cat" }]
            },
            new Owner
            {
                Name = "Owner 6 - Only Dog",
                Pets = [new Dog { Name = "Only Dog", IsBarking = true }]
            }
        };

        context.Owners.AddRange(owners);
        await context.SaveChangesAsync();
    }

    public class Query
    {
        [UsePaging]
        public async Task<Connection<Owner>> GetOwnersAsync(
            PagingArguments pagingArgs,
            AnimalContext context,
            ISelection selection,
            IResolverContext resolverContext,
            [GlobalState] List<QueryInfo> queries,
            CancellationToken cancellationToken)
        {
            return await context.Owners
                .Select(selection.AsSelector<Owner>())
                .OrderBy(t => t.Name)
                .ThenBy(t => t.Id)
                .Capture(queries)
                .ToPageAsync(pagingArgs, cancellationToken)
                .ToConnectionAsync();
        }
    }

    [ExtendObjectType<Owner>]
    public static class OwnerExtensions
    {
        [BindMember(nameof(Owner.Pets))]
        [UsePaging]
        public static async Task<Connection<Animal>> GetPetsAsync(
            [Parent("Id")] Owner owner,
            PagingArguments pagingArgs,
            AnimalsByOwnerDataLoader animalsByOwner,
            ISelection selection,
            [GlobalState] List<QueryInfo> queries,
            CancellationToken cancellationToken)
        {
            return await animalsByOwner
                .WithPagingArguments(pagingArgs)
                .Select(selection)
                .SetState(queries)
                .LoadAsync(owner.Id, cancellationToken)
                .ToConnectionAsync();
        }
    }

    public sealed class AnimalsByOwnerDataLoader
        : StatefulBatchDataLoader<int, Page<Animal>>
    {
        private readonly IServiceProvider _services;

        public AnimalsByOwnerDataLoader(
            IServiceProvider services,
            IBatchScheduler batchScheduler,
            DataLoaderOptions options)
            : base(batchScheduler, options)
        {
            _services = services;
        }

        protected override async Task<IReadOnlyDictionary<int, Page<Animal>>> LoadBatchAsync(
            IReadOnlyList<int> keys,
            DataLoaderFetchContext<Page<Animal>> context,
            CancellationToken cancellationToken)
        {
            var pagingArgs = context.GetPagingArguments();
            var selector = context.GetSelector();

            await using var scope = _services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AnimalContext>();

            return await dbContext.Owners
                .Where(t => keys.Contains(t.Id))
                .SelectMany(t => t.Pets)
                .OrderBy(t => t.Name)
                .ThenBy(t => t.Id)
                .Select(selector, t => t.OwnerId)
                .Capture(context.GetQueries())
                .ToBatchPageAsync(
                    t => t.OwnerId,
                    pagingArgs,
                    cancellationToken);
        }
    }
}

file static class Extensions
{
    public static IQueryable<T> Capture<T>(
        this IQueryable<T> query,
        List<QueryInfo> queryInfos)
    {
        queryInfos.Add(
            new QueryInfo
            {
                QueryText = query.ToQueryString(),
                ExpressionText = query.Expression.ToString()
            });
        return query;
    }

    public static List<QueryInfo> GetQueries<T>(
        this DataLoaderFetchContext<Page<T>> context)
        => context.GetRequiredState<List<QueryInfo>>();

    public static OperationRequestBuilder AddQueries(
        this OperationRequestBuilder builder,
        List<QueryInfo> queries)
        => builder.SetGlobalState("queries", queries);

    public static Snapshot AddQueries(
        this Snapshot snapshot,
        List<QueryInfo> queries)
    {
        for (var i = 0; i < queries.Count; i++)
        {
            snapshot
                .Add(queries[i].QueryText, $"SQL {i}", "sql")
                .Add(queries[i].ExpressionText, $"Expression {i}");
        }

        return snapshot;
    }
}
