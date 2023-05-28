using Grpc;
using Grpc.Core;

namespace CatalogService;

public class CatalogGrpcService(CatalogDbContext catalogDbContext) : Grpc.CatalogService.CatalogServiceBase
{
    public override async Task<CatalogItemsResponse> GetCatalogItems(CatalogItemsRequest request, ServerCallContext context)
    {
        static int? GetValueOrNull(int value) => value <= 0 ? null : value;

        var catalogBrandId = GetValueOrNull(request.CatalogBrandId);
        var before = GetValueOrNull(request.Before);
        var after = GetValueOrNull(request.After);

        var itemsOnPage = await catalogDbContext.GetCatalogItemsCompiledAsync(catalogBrandId, before, after, request.PageSize);

        var (firstId, nextId) = itemsOnPage switch
        {
            [] => (0, 0),
            [var only] => (only.Id, only.Id),
            [var first, .., var last] => (first.Id, last.Id)
        };

        return new()
        {
            FirstId = firstId,
            LastId = nextId,
            HasNextPage = itemsOnPage.Count < request.PageSize,
            CatalogItems = { itemsOnPage.Select(item => new CatalogItemDto { Id = item.Id, Name = item.Name, Description = item.Description, Price = (double)item.Price }).Take(request.PageSize) }
        };
    }
}
