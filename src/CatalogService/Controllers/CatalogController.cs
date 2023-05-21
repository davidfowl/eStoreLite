using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController(CatalogDbContext catalogContext, IHostEnvironment environment) : Controller
{
    [HttpGet("items/type/all/brand")]
    [ProducesResponseType(typeof(Catalog), StatusCodes.Status200OK)]
    public Task<ActionResult<Catalog>> ItemsAsync([FromQuery] int pageSize = 8, [FromQuery] int pageIndex = 0)
        => ItemsByBrandIdAsync(null, pageSize, pageIndex);

    [HttpGet("items/type/all/brand/{catalogBrandId?}")]
    [ProducesResponseType(typeof(Catalog), StatusCodes.Status200OK)]
    public async Task<ActionResult<Catalog>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int pageSize = 8, [FromQuery] int pageIndex = 0)
    {
        IQueryable<CatalogItem> root = catalogContext.CatalogItems;

        if (catalogBrandId.HasValue)
        {
            root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        }

        var totalItems = await root
            .LongCountAsync();

        var itemsOnPage = await root
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .ToListAsync();

        return new Catalog(pageIndex, pageSize, totalItems, itemsOnPage);
    }

    [HttpGet]
    [Route("items/{catalogItemId:int}/image")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImageAsync(int catalogItemId)
    {
        var item = await catalogContext.CatalogItems.FindAsync(catalogItemId);

        if (item is null)
        {
            return NotFound();
        }

        var path = Path.Combine(environment.ContentRootPath, "Images", item.PictureFileName);

        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        var bytes = await System.IO.File.ReadAllBytesAsync(path);

        return File(bytes, "image/jpeg");
    }
}

