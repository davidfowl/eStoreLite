using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService;

public record Catalog(int PageIndex, int PageSize, long Count, List<CatalogItem> Data);

//public record Catalog(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        DefineCatalogBrand(builder.Entity<CatalogBrand>());

        DefineCatalogItem(builder.Entity<CatalogItem>());

        DefineCatalogType(builder.Entity<CatalogType>());
    }

    private void DefineCatalogType(EntityTypeBuilder<CatalogType> builder)
    {
        builder.ToTable("CatalogType");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("catalog_type_hilo")
            .IsRequired();

        builder.Property(cb => cb.Type)
            .IsRequired()
            .HasMaxLength(100);
    }

    private static void DefineCatalogItem(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("Catalog");

        builder.Property(ci => ci.Id)
                    .UseHiLo("catalog_hilo")
                    .IsRequired();

        builder.Property(ci => ci.Name)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(ci => ci.Price)
            .IsRequired(true);

        builder.Property(ci => ci.PictureFileName)
            .IsRequired(false);

        builder.Ignore(ci => ci.PictureUri);

        builder.HasOne(ci => ci.CatalogBrand)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogBrandId);

        builder.HasOne(ci => ci.CatalogType)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogTypeId);
    }

    private static void DefineCatalogBrand(EntityTypeBuilder<CatalogBrand> builder)
    {
        builder.ToTable("CatalogBrand");
        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.Id)
            .UseHiLo("catalog_brand_hilo")
            .IsRequired();

        builder.Property(cb => cb.Brand)
            .IsRequired()
            .HasMaxLength(100);
    }
}

public class CatalogType
{
    public int Id { get; set; }
    public string Type { get; set; } = default!;
}

public class CatalogBrand
{
    public int Id { get; set; }
    public string Brand { get; set; } = default!;
}

public class CatalogItem
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string PictureFileName { get; set; } = default!;
    public string? PictureUri { get; set; }

    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; } = default!;

    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; } = default!;
    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
}
