using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Write up the JSON source generator and make sure we remove the reflection fallback.
    // this is a good way to verify that the source generator is working as expected.
    options.SerializerOptions.TypeInfoResolverChain.Clear();
    options.SerializerOptions.TypeInfoResolverChain.Add(CatalogServiceJsonContext.Default);

    // The consumer of this API isn't JavaScript so we don't need to escape the HTML characters.
    options.SerializerOptions.Encoder = null;
});

builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("CatalogDb") ??
        throw new InvalidDataException("Missing connection string CatalogDb");

    return new NpgsqlDataSourceBuilder(connectionString).Build();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCatalogApi();

// await app.Services.InitializeDatabaseAsync();

app.Run();
