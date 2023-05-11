using System.Text.Json;

namespace Frontend;

public class CatalogService(HttpClient client)
{
    public async Task<Catalog?> GetItemsAsync(int pageIndex = 0)
    {
        var response = await client.GetStringAsync($"/api/v1/catalog/items/type/all/brand?pageIndex={pageIndex}");
        return JsonSerializer.Deserialize<Catalog>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }

    public Task<HttpResponseMessage> GetImageAsync(int catalogItemId) =>
        client.GetAsync($"/api/v1/catalog/items/{catalogItemId}/image");
}

public record Catalog(int PageIndex, int PageSize, int Count, List<CatalogItem> Data);

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
