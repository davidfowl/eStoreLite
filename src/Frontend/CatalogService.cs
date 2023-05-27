using System.Globalization;

namespace Frontend;

public class CatalogService(HttpClient client)
{
    public Task<Catalog?> GetItemsAsync(int? before = null, int? after = null)
    {
        // Make the query string with encoded parameters
        var query = (before, after) switch
        {
            (null, null) => default,
            (int b, null) => QueryString.Create("before", b.ToString(CultureInfo.InvariantCulture)),
            (null, int a) => QueryString.Create("after", a.ToString(CultureInfo.InvariantCulture)),
            _ => throw new InvalidOperationException(),
        };

        return client.GetFromJsonAsync<Catalog>($"/api/v1/catalog/items/type/all/brand{query}");
    }
}

public record Catalog(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);

public record CatalogItem(int Id, string Name, string? Description, decimal Price);