namespace Frontend;

public class CatalogService(HttpClient client)
{
    public Task<Catalog?> GetItemsAsync() => 
        client.GetFromJsonAsync<Catalog>("/api/v1/catalog/items/type/all/brand");
}

public record Catalog
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public int Count { get; init; }
    public List<CatalogItem> Data { get; init; } = default!;
}

public record CatalogItem
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Description { get; init; } = default!;
    public decimal Price { get; init; }
    public string? PictureUri { get; init; }
    public int CatalogBrandId { get; init; }
    public string CatalogBrand { get; init; } = default!;
    public int CatalogTypeId { get; init; }
    public string CatalogType { get; init; } = default!;
}
