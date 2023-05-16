var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Write up the JSON source generator and make sure we remove the reflection fallback.
    // this is a good way to verify that the source generator is working as expected.
    options.SerializerOptions.TypeInfoResolverChain.Clear();
    options.SerializerOptions.TypeInfoResolverChain.Add(CatalogServiceJsonContext.Default);

    // The consumer of this API isn't JavaScript so we don't need to escape the HTML characters.
    options.SerializerOptions.Encoder = null;
});

builder.Services.AddCatalogDb(builder.Configuration);

var app = builder.Build();

app.MapCatalogApi();

// await app.Services.InitializeDatabaseAsync();

app.Run();
