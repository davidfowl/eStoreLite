using Nanorm.Npgsql;
using Npgsql;
using Systems.Collections.Generic;

namespace CatalogService;

public interface ICatalogDb
{
    Task<List<CatalogItem>> GetCatalogItemsAsync(int? catalogBrandId, int? before, int? after, int pageSize);
    Task<CatalogItem?> GetCatalogItemAsync(int catalogItemId);
}

public sealed class CatalogDb(NpgsqlDataSource dataSource) : ICatalogDb
{
    public Task<CatalogItem?> GetCatalogItemAsync(int catalogItemId)
    {
        const string sql =
        """
            SELECT *
            FROM "Catalog" AS c
            WHERE c."Id" = $1
        """;

        return dataSource.QuerySingleAsync<CatalogItem>(sql, catalogItemId.AsTypedDbParameter());
    }

    public Task<List<CatalogItem>> GetCatalogItemsAsync(int? catalogBrandId, int? before, int? after, int pageSize)
    {
        const string sql =
        """
            SELECT *
            FROM "Catalog" AS c
            WHERE ($1 = -1 OR c."CatalogBrandId" = $1)
            AND ($2 = -1 OR c."Id" <= $2)
            AND ($3 = -1 OR c."Id" >= $3)
            ORDER BY c."Id"
            LIMIT $4
        """;

        return dataSource.QueryAsync<CatalogItem>(sql,
            (catalogBrandId ?? -1).AsTypedDbParameter(),
            (before ?? -1).AsTypedDbParameter(),
            (after ?? -1).AsTypedDbParameter(),
            (pageSize + 1).AsTypedDbParameter())
            .ToListAsync();
    }
}

public record Catalog(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);

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

public class CatalogItem : IDataReaderMapper<CatalogItem>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string PictureFileName { get; set; } = default!;

    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; } = default!;

    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; } = default!;
    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }

    public static CatalogItem Map(NpgsqlDataReader dataReader)
    {
        return new()
        {
            Id = dataReader.GetInt32(dataReader.GetOrdinal(nameof(Id))),
            Name = dataReader.GetString(dataReader.GetOrdinal(nameof(Name))),
            Description = dataReader.GetString(dataReader.GetOrdinal(nameof(Description))),
            Price = dataReader.GetDecimal(dataReader.GetOrdinal(nameof(Price))),
            PictureFileName = dataReader.GetString(dataReader.GetOrdinal(nameof(PictureFileName))),
            CatalogTypeId = dataReader.GetInt32(dataReader.GetOrdinal(nameof(CatalogTypeId))),
            CatalogBrandId = dataReader.GetInt32(dataReader.GetOrdinal(nameof(CatalogBrandId))),
            AvailableStock = dataReader.GetInt32(dataReader.GetOrdinal(nameof(AvailableStock))),
            RestockThreshold = dataReader.GetInt32(dataReader.GetOrdinal(nameof(RestockThreshold))),
            MaxStockThreshold = dataReader.GetInt32(dataReader.GetOrdinal(nameof(MaxStockThreshold))),
            OnReorder = dataReader.GetBoolean(dataReader.GetOrdinal(nameof(OnReorder))),
        };
    }
}
