using Microsoft.AspNetCore.Mvc;

namespace CatalogService.Controllers;

[ApiController]
public class ImagesController(CatalogDbContext catalogDbContext, IHostEnvironment environment) : Controller
{
    [HttpGet]
    [Route("api/v1/catalog/items/{catalogItemId:int}/image")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetImageAsync(int catalogItemId)
    {
        var item = await catalogDbContext.CatalogItems.FindAsync(catalogItemId);

        if (item is null)
        {
            return NotFound();
        }

        var path = Path.Combine(environment.ContentRootPath, "Images", item.PictureFileName);

        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        return PhysicalFile(path, "image/jpeg");
    }
}
