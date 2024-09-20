using System.Text;
using CookieCrumble;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate.AspNetCore.Tests.Utilities;
using HotChocolate.Execution;
using HotChocolate.Language;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.HttpSys;

namespace HotChocolate.AspNetCore;

public class PersistedOperationTests : ServerTestBase
{
    public PersistedOperationTests(TestServerFactory serverFactory)
        : base(serverFactory)
    {
    }

    [Fact]
    public async Task HotChocolateStyle_MD5Hash_Success()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new MD5DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Id = key, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task HotChocolateStyle_MD5Hash_NotFound()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new MD5DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        // we are not adding the query to the store so the server request should fail
        // storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Id = key, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task HotChocolateStyle_Sha1Hash_Success()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new Sha1DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddSha1DocumentHashProvider(HashFormat.Hex)
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Id = key, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task HotChocolateStyle_Sha256Hash_Success()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new Sha256DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddSha256DocumentHashProvider(HashFormat.Hex)
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Id = key, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task HotChocolateStyle_Sha256Hash_Query_Empty_String_Success()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new Sha256DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddSha256DocumentHashProvider(HashFormat.Hex)
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest
            {
                Id = key,
                Query = string.Empty
            },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ApolloStyle_MD5Hash_Success()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new MD5DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            CreateApolloStyleRequest(hashProvider.Name, key),
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ApolloStyle_MD5Hash_NotFound()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new MD5DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        // we are not adding the query to the store so the server request should fail
        // storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            CreateApolloStyleRequest(hashProvider.Name, key),
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ApolloStyle_Sha1Hash_Success()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new Sha1DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddSha1DocumentHashProvider(HashFormat.Hex)
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            CreateApolloStyleRequest(hashProvider.Name, key),
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task ApolloStyle_Sha256Hash_Success()
    {
        // arrange
        var storage = new OperationStorage();
        var hashProvider = new Sha256DocumentHashProvider(HashFormat.Hex);

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddSha256DocumentHashProvider(HashFormat.Hex)
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";
        var key = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(query));
        storage.AddOperation(key, query);

        // act
        var result = await server.PostAsync(
            CreateApolloStyleRequest(hashProvider.Name, key),
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task Standard_Query_By_Default_Works()
    {
        // arrange
        var storage = new OperationStorage();

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Query = query, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task Standard_Query_Not_Allowed()
    {
        // arrange
        var storage = new OperationStorage();

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ModifyRequestOptions(o => o.OnlyAllowPersistedOperations = true)
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Query = query, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task Standard_Query_Not_Allowed_Custom_Error()
    {
        // arrange
        var storage = new OperationStorage();

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ModifyRequestOptions(o =>
                {
                    o.OnlyAllowPersistedOperations = true;
                    o.OnlyPersistedOperationsAreAllowedError =
                        ErrorBuilder.New()
                            .SetMessage("Not allowed!")
                            .Build();
                })
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline());

        var query = "{ __typename }";

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Query = query, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    [Fact]
    public async Task Standard_Query_Not_Allowed_Override_Per_Request()
    {
        // arrange
        var storage = new OperationStorage();

        var server = CreateStarWarsServer(
            configureServices: s => s
                .AddGraphQL("StarWars")
                .ModifyRequestOptions(o =>
                {
                    o.OnlyAllowPersistedOperations = true;
                })
                .ConfigureSchemaServices(c => c.AddSingleton<IOperationDocumentStorage>(storage))
                .UsePersistedOperationPipeline()
                .AddHttpRequestInterceptor<AllowNonPersistedOperationInterceptor>());

        var query = "{ __typename }";

        // act
        var result = await server.PostAsync(
            new ClientQueryRequest { Query = query, },
            path: "/starwars");

        // assert
        result.MatchSnapshot();
    }

    private ClientQueryRequest CreateApolloStyleRequest(string hashName, string key)
        =>  new()
        {
            Extensions = new Dictionary<string, object?>
            {
                ["persistedQuery"] = new Dictionary<string, object?>
                {
                    ["version"] = 1,
                    [hashName] = key,
                },
            },
        };

    private sealed class OperationStorage : IOperationDocumentStorage
    {
        private readonly Dictionary<string, OperationDocument> _cache =
            new(StringComparer.Ordinal);

        public ValueTask<IOperationDocument?> TryReadAsync(
            OperationDocumentId documentId,
            CancellationToken cancellationToken = default)
            => _cache.TryGetValue(documentId.Value, out var value)
                ? new ValueTask<IOperationDocument?>(value)
                : new ValueTask<IOperationDocument?>(default(IOperationDocument));

        public ValueTask SaveAsync(
            OperationDocumentId documentId,
            IOperationDocument document,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public void AddOperation(string key, string sourceText)
        {
            var doc = new OperationDocument(Utf8GraphQLParser.Parse(sourceText));
            _cache.Add(key, doc);
        }
    }

    private sealed class AllowNonPersistedOperationInterceptor : DefaultHttpRequestInterceptor
    {
        public override ValueTask OnCreateAsync(
            HttpContext context,
            IRequestExecutor requestExecutor,
            OperationRequestBuilder requestBuilder,
            CancellationToken cancellationToken)
        {
            requestBuilder.AllowNonPersistedOperation();
            return base.OnCreateAsync(context, requestExecutor, requestBuilder, cancellationToken);
        }
    }
}
