using System.Net;
using Microsoft.AspNetCore.Components.Endpoints;

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

//app.MapGet("/", (int page = 0) => new RazorComponentResult<App>(new { page }));

app.MapGet("/", (int? before, int? after) => new RazorComponentResult<App>(new { before, after }));

app.MapGet("/catalog/images/{id}", async (int id, CatalogService catalogService) =>
{
    var response = await catalogService.GetImageAsync(id);

    return response.StatusCode switch
    {
        HttpStatusCode.NotFound => Results.NotFound(),
        _ => Results.File(await response.Content.ReadAsByteArrayAsync(), "image/jpeg")
    };
});

app.Run();
