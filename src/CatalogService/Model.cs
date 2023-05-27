using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogService;

// DTOs
public record Catalog(int FirstId, int NextId, bool IsLastPage, IEnumerable<ICatalogItem> Data);

public interface ICatalogItem
{
    public int Id { get; }
    public string Name { get; }
    public string? Description { get; }
    public decimal Price { get; }
}

// Database Models

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    // https://learn.microsoft.com/ef/core/performance/advanced-performance-topics#compiled-queries

    private static readonly Func<CatalogDbContext, int?, int?, int?, int, IAsyncEnumerable<CatalogItem>> GetCatalogItemsQuery =
        EF.CompileAsyncQuery((CatalogDbContext context, int? catalogBrandId, int? before, int? after, int pageSize) =>
           // https://learn.microsoft.com/ef/core/performance/efficient-querying#tracking-no-tracking-and-identity-resolution
           context.CatalogItems.AsNoTracking()
                  .OrderBy(ci => ci.Id)
                  .Where(ci => catalogBrandId == null || ci.CatalogBrandId == catalogBrandId)
                  // https://learn.microsoft.com/ef/core/querying/pagination#keyset-pagination
                  .Where(ci => before == null || ci.Id <= before)
                  .Where(ci => after == null || ci.Id >= after)
                  .Take(pageSize + 1));

    public async Task<List<CatalogItem>> GetCatalogItemsCompiledAsync(int? catalogBrandId, int? before, int? after, int pageSize)
    {
        // Use the page size to avoid resizing
        var results = new List<CatalogItem>(pageSize);
        await foreach (var value in GetCatalogItemsQuery(this, catalogBrandId, before, after, pageSize))
        {
            results.Add(value);
        }

        return results;
    }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();
}

[Table("CatalogType")]
public class CatalogType
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Type { get; set; } = default!;
}

[Table("CatalogBrand")]
public class CatalogBrand
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string Brand { get; set; } = default!;
}

[Table("Catalog")]
public class CatalogItem : ICatalogItem
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public decimal Price { get; set; }
    public string? PictureFileName { get; set; }

    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; } = default!;

    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; } = default!;
    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
}
