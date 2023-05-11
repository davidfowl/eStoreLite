namespace Frontend;

public class CatalogService(HttpClient client)
{
    public Task<Catalog?> GetItemsAsync(int? before = null, int? after = null)
    {
        var qs = (before, after) switch
        {
            (null, null) => "",
            (var b, null) => $"?before={b}",
            (null, var a) => $"?after={a}",
            (var b, var a) => $"?before={b}&after={a}"
        };

        return client.GetFromJsonAsync<Catalog>($"/api/v1/catalog/items/type/all/brand{qs}");
    }
}

public record Catalog(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);

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
