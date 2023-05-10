var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<CatalogService>(c =>
{
    var catalogServiceUrl = builder.Configuration["CatalogServiceUrl"] ??
        throw new InvalidDataException("Missing configuration for CatalogServiceUrl");

    c.BaseAddress = new(catalogServiceUrl);
});

builder.Services.AddRazorComponents();

var app = builder.Build();

app.UseStaticFiles();

app.MapRazorComponents<App>();

app.Run();
