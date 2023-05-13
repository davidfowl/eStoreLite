﻿using Microsoft.EntityFrameworkCore;

namespace CatalogService;
public static class CatalogApi
{
    public static IEndpointRouteBuilder MapCatalogApi(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/v1/catalog/items/type/all/brand/{catalogBrandId?}", async (int? catalogBrandId, CatalogDbContext catalogContext, int? before, int? after, int pageSize = 10) =>
        {
            IQueryable<CatalogItem> root = catalogContext.CatalogItems.AsNoTracking();

            if (catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            root = root.OrderBy(ci => ci.Id);

            if (before.HasValue)
            {
                root = root.Where(ci => ci.Id < before);
            }
            else if (after.HasValue)
            {
                root = root.Where(ci => ci.Id >= after);
            }

            var itemsOnPage = await root.Take(pageSize + 1).ToListAsync();

            var (firstId, lastId) = itemsOnPage switch
            {
                [] => (0, 0),
                [var only] => (only.Id, only.Id),
                [var first, .., var last] => (first.Id, last.Id)
            };

            return new Catalog(
                firstId,
                lastId,
                itemsOnPage.Count < pageSize,
                itemsOnPage.Take(pageSize));
        });

        routes.MapGet("\"/api/v1/catalog/items/{catalogItemId:int}/image", async (int catalogItemId, CatalogDbContext catalogDbContext, IHostEnvironment environment) =>
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
        });

        return routes;
    }
}