using Microsoft.AspNetCore.Components.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var catalogServiceUrl = builder.Configuration["CatalogServiceUrl"] ??
    throw new InvalidDataException("Missing configuration for CatalogServiceUrl");

var catalogServiceGrpcUrl = builder.Configuration["CatalogServiceGrpcUrl"] ??
    throw new InvalidDataException("Missing configuration for CatalogServiceGrpcUrl");

builder.Services.AddHttpForwarder();

builder.Services.AddTransient<CatalogService>();

builder.Services.AddGrpcClient<Grpc.CatalogService.CatalogServiceClient>(c =>
{
    c.Address = new Uri(catalogServiceGrpcUrl);
});

builder.Services.AddRazorComponents();

var app = builder.Build();

app.UseStaticFiles();

app.MapRazorComponents<MainLayout>();

app.MapGet("/", (int? before, int? after) => new RazorComponentResult<App>(new { before, after }));

app.MapForwarder("/catalog/images/{id}", catalogServiceUrl, "/api/v1/catalog/items/{id}/image");

app.Run();
