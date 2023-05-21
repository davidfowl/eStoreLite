using Microsoft.EntityFrameworkCore;

namespace CatalogService;

public static class CatalogApi
{
#if NET6_0
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder routes)
    {
        var api = routes;
#else
    public static RouteGroupBuilder MapCatalogApi(this IEndpointRouteBuilder routes)
    {
        var api = routes.MapGroup("/api/v1/catalog");
        api.WithTags("Catalog");
#endif

        // Offset paging
        //group.MapGet("items/type/all/brand/{catalogBrandId?}", async (int? catalogBrandId, CatalogDbContext catalogContext, int pageIndex = 0, int pageSize = 8) =>
        //{
        //    IQueryable<CatalogItem> root = catalogContext.CatalogItems;//.AsNoTracking();

        //    if (catalogBrandId.HasValue)
        //    {
        //        root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        //    }

        //    var totalItems = await root
        //        .LongCountAsync();

        //    var itemsOnPage = await root
        //        .Skip(pageSize * pageIndex)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    //var itemsOnPage = await catalogContext.GetCatalogItemsAsync(catalogBrandId, pageIndex, pageSize);
        //    //var totalItems = await catalogContext.GetCatalogItemsCountAsync(catalogBrandId);

        //    return new Catalog(pageIndex, pageSize, totalItems, itemsOnPage);
        //});

        // KeySet paging
        api.MapGet("items/type/all/brand/{catalogBrandId?}", async (int? catalogBrandId, CatalogDbContext catalogContext, int? before, int? after, int pageSize = 8) =>
        {
            var itemsOnPage = await catalogContext.GetCatalogItemsKeySetPagingCompiledAsync(catalogBrandId, before, after, pageSize);

            var (firstId, nextId) = itemsOnPage switch
            {
                [] => (0, 0),
                [var only] => (only.Id, only.Id),
                [var first, .., var last] => (first.Id, last.Id)
            };

            return new CatalogKeySet(
                firstId,
                nextId,
                pageSize,
                itemsOnPage.Count < pageSize,
                itemsOnPage.Take(pageSize));
        });

        api.MapGet("items/{catalogItemId:int}/image", async (int catalogItemId, CatalogDbContext catalogDbContext, IHostEnvironment environment) =>
        {
            var item = await catalogDbContext.CatalogItems.FindAsync(catalogItemId);

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

        return api;
    }
}