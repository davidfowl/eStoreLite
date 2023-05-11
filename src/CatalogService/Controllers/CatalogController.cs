﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController(CatalogDbContext catalogContext) : Controller
{
    [HttpGet]
    [Route("items/type/all/brand/{catalogBrandId?}")]
    [ProducesResponseType(typeof(Catalog), StatusCodes.Status200OK)]
    public async Task<ActionResult<Catalog>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
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
}

