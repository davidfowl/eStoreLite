using Microsoft.AspNetCore.Components.Endpoints;

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

app.MapGet("/", (int page = 0) => new RazorComponentResult<App>(new { page }));

app.MapForwarder("/catalog/images/{id}", catalogServiceUrl, (context, proxyRequest, destinationPrefix, token) =>
{
    if (context.Request.RouteValues["id"] is string id)
    {
        // Rewrite the path to point to the catalog service's image endpoint
        proxyRequest.RequestUri = new Uri($"{destinationPrefix}/api/v1/catalog/items/{id}/image");
    }

    return ValueTask.CompletedTask;
});

app.Run();
