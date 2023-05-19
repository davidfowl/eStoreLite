var builder = WebApplication.CreateBuilder(args);

var catalogServiceUrl = builder.Configuration["CatalogServiceUrl"] ??
    throw new InvalidDataException("Missing configuration for CatalogServiceUrl");

builder.Services.AddHttpForwarder();

builder.Services.AddHttpClient<CatalogService>(c =>
{
    c.BaseAddress = new(catalogServiceUrl);
});

builder.Services.AddRazorComponents();

var app = builder.Build();

app.UseStaticFiles();

app.MapRazorComponents<App>();

app.MapForwarder("/catalog/images/{id}", catalogServiceUrl, "/api/v1/catalog/items/{id}/image");

app.Run();
