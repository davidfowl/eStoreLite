namespace CatalogService;

public static class CatalogApi
{
    public static RouteGroupBuilder MapCatalogApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/catalog");

        group.WithTags("Catalog");

        group.MapGet("items/type/all/brand/{catalogBrandId?}", async (int? catalogBrandId, ICatalogDb db, int? before, int? after, int? pageSize) =>
        {
            var ps = pageSize ?? 8;

            var itemsOnPage = await db.GetCatalogItemsAsync(catalogBrandId, before, after, ps);

            var (firstId, nextId) = itemsOnPage switch
            {
                [] => (0, 0),
                [var only] => (only.Id, only.Id),
                [var first, .., var last] => (first.Id, last.Id)
            };

            return new Catalog(
                firstId,
                nextId,
                itemsOnPage.Count < ps,
                itemsOnPage.Take(ps));
        });

        group.MapGet("items/{catalogItemId:int}/image", async (int catalogItemId, ICatalogDb db, IHostEnvironment environment) =>
        {
            var item = await db.GetCatalogItemAsync(catalogItemId);

            if (item is null)
            {
                return Results.NotFound();
            }

            var path = Path.Combine(environment.ContentRootPath, "Images", item.PictureFileName);

            if (!File.Exists(path))
            {
                return Results.NotFound();
            }

            return Results.File(path, "image/jpeg");
        })
        .Produces(404)
        .Produces(200, contentType: "image/jpeg");

        return group;
    }
}