﻿using Npgsql;

namespace CatalogService;

internal static class CatalogContextSeed
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();
        var dataSource = serviceProvider.GetRequiredService<NpgsqlDataSource>();

        var sql = File.ReadAllText(Path.Combine(env.ContentRootPath, "Schema.sql"));
        await dataSource.ExecuteAsync(sql);

        await SeedAsync(dataSource);
    }

    private static async Task SeedAsync(NpgsqlDataSource dataSource)
    {
        static List<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new()
            {
                new() { Brand = "Azure"},
                new() { Brand = ".NET" },
                new() { Brand = "Visual Studio" },
                new() { Brand = "SQL Server" },
                new() { Brand = "Other" }
            };
        }

        static List<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new()
            {
                new() { Type = "Mug"},
                new() { Type = "T-Shirt" },
                new() { Type = "Sheet" },
                new() { Type = "USB Memory Stick" }
            };
        }

        static List<CatalogItem> GetPreconfiguredItems()
        {
            return new ()
            {
                new() { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Bot Black Hoodie", Name = ".NET Bot Black Hoodie", Price = 19.5M, PictureFileName = "1.png" },
                new() { CatalogTypeId = 1, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price= 8.50M, PictureFileName = "2.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White T-Shirt", Name = "Prism White T-Shirt", Price = 12, PictureFileName = "3.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation T-shirt", Name = ".NET Foundation T-shirt", Price = 12, PictureFileName = "4.png" },
                new() { CatalogTypeId = 3, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5M, PictureFileName = "5.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Blue Hoodie", Name = ".NET Blue Hoodie", Price = 12, PictureFileName = "6.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Roslyn Red T-Shirt", Name = "Roslyn Red T-Shirt", Price = 12, PictureFileName = "7.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Kudu Purple Hoodie", Name = "Kudu Purple Hoodie", Price = 8.5M, PictureFileName = "8.png" },
                new() { CatalogTypeId = 1, CatalogBrandId = 5, AvailableStock = 100, Description = "Cup<T> White Mug", Name = "Cup<T> White Mug", Price = 12, PictureFileName = "9.png" },
                new() { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = ".NET Foundation Sheet", Name = ".NET Foundation Sheet", Price = 12, PictureFileName = "10.png" },
                new() { CatalogTypeId = 3, CatalogBrandId = 2, AvailableStock = 100, Description = "Cup<T> Sheet", Name = "Cup<T> Sheet", Price = 8.5M, PictureFileName = "11.png" },
                new() { CatalogTypeId = 2, CatalogBrandId = 5, AvailableStock = 100, Description = "Prism White TShirt", Name = "Prism White TShirt", Price = 12, PictureFileName = "12.png" },
            };
        }

        foreach (var brand in GetPreconfiguredCatalogBrands())
        {
            await dataSource.ExecuteAsync("""
                INSERT INTO CatalogBrands (Id, Brand)
                VALUES (@Id, @Brand)
            """, 
            brand.Id.AsTypedDbParameter(), brand.Brand.AsTypedDbParameter());
        }

        foreach (var type in GetPreconfiguredCatalogTypes())
        {
            await dataSource.ExecuteAsync("""
                INSERT INTO CatalogTypes (Id, Type)
                VALUES (@Id, @Type)
             """, 
             type.Id.AsTypedDbParameter(), type.Type.AsTypedDbParameter());
        }

        foreach (var item in GetPreconfiguredItems())
        {
            await dataSource.ExecuteAsync("""
                INSERT INTO CatalogItems (Id, Name, Description, Price, PictureFileName, AvailableStock, RestockThreshold, MaxStockThreshold, OnReorder, CatalogTypeId, CatalogBrandId)
                               VALUES (@Id, @Name, @Description, @Price, @PictureFileName, @AvailableStock, @RestockThreshold, @MaxStockThreshold, @OnReorder, @CatalogTypeId, @CatalogBrandId)
                """,
                item.Id.AsTypedDbParameter(),
                item.Name.AsTypedDbParameter(),
                item.Description.AsTypedDbParameter(),
                item.Price.AsTypedDbParameter(),
                item.PictureFileName.AsTypedDbParameter(),
                item.AvailableStock.AsTypedDbParameter(),
                item.RestockThreshold.AsTypedDbParameter(),
                item.MaxStockThreshold.AsTypedDbParameter(),
                item.OnReorder.AsTypedDbParameter(),
                item.CatalogTypeId.AsTypedDbParameter(),
                item.CatalogBrandId.AsTypedDbParameter());
        }
    }
}
