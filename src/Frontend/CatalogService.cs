using Grpc;

namespace Frontend;

public class CatalogService(Grpc.CatalogService.CatalogServiceClient client)
{
    public async Task<Catalog?> GetItemsAsync(int? before = null, int? after = null)
    {
        var response = await client.GetCatalogItemsAsync(new CatalogItemsRequest
        {
            PageSize = 8,
            Before = before ?? -1,
            After = after ?? -1
        });

        return new(
            response.FirstId, 
            response.LastId, 
            response.HasNextPage, 
            response.CatalogItems.Select(item => new CatalogItem(item.Id, item.Name, item.Description, (decimal)item.Price)));
    }
}

public record Catalog(int FirstId, int NextId, bool IsLastPage, IEnumerable<CatalogItem> Data);

public record CatalogItem(int Id, string Name, string? Description, decimal Price);