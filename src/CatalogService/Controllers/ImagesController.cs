using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CatalogService.Controllers
{
    [ApiController]
    public class ImagesController : Controller
    {
        private readonly CatalogDbContext _catalogDbContext;
        private readonly IHostEnvironment _environment;

        public ImagesController(CatalogDbContext catalogDbContext, IHostEnvironment environment)
        {
            _catalogDbContext = catalogDbContext;
            _environment = environment;
        }

        [HttpGet]
        [Route("api/v1/catalog/items/{catalogItemId:int}/image")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetImageAsync(int catalogItemId)
        {
            var item = await _catalogDbContext.CatalogItems.FindAsync(catalogItemId);

            if (item is null)
            {
                return NotFound();
            }

            var path = Path.Combine(_environment.ContentRootPath, "Images", item.PictureFileName);

            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(path);

            return File(bytes, "image/jpeg");
        }
    }
}
