using HotChocolate.AspNetCore.Tests.Utilities;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using HotChocolate.Language;
using HotChocolate.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using DirectiveLocation = HotChocolate.Types.DirectiveLocation;

namespace HotChocolate.Fusion.Shared;

public record TestSubgraph(
    TestServer TestServer,
    ISchema Schema,
    SubgraphTestContext Context,
    string SchemaExtensions = "",
    bool IsOffline = false)
{
    public static Task<TestSubgraph> CreateAsync(
        string schemaText,
        bool isOffline = false,
        bool enableSemanticNonNull = false)
        => CreateAsync(
            configure: builder => builder
                .AddDocumentFromString(schemaText)
                .AddResolverMocking()
                .AddTestDirectives(),
            isOffline: isOffline,
            enableSemanticNonNull: enableSemanticNonNull);

    public static async Task<TestSubgraph> CreateAsync(
        Action<IRequestExecutorBuilder> configure,
        string extensions = "",
        bool isOffline = false,
        bool enableSemanticNonNull = false)
    {
        var testServerFactory = new TestServerFactory();
        var testContext = new SubgraphTestContext();

        var testServer = testServerFactory.Create(
            services =>
            {
                var builder = services
                    .AddRouting()
                    .AddGraphQLServer(disableDefaultSecurity: true)
                    .ModifyOptions(o => o.EnableSemanticNonNull = enableSemanticNonNull);

                configure(builder);
            },
            app =>
            {
                app.Use(next => context =>
                {
                    testContext.HasReceivedRequest = true;
                    return next(context);
                });

                app.UseRouting().UseEndpoints(endpoints => endpoints.MapGraphQL());
            });

        var schema = await testServer.Services.GetSchemaAsync();

        return new TestSubgraph(testServer, schema, testContext, extensions, isOffline);
    }

    public bool HasReceivedRequest => Context.HasReceivedRequest;
}

public class SubgraphTestContext
{
    public bool HasReceivedRequest { get; set; }
}
