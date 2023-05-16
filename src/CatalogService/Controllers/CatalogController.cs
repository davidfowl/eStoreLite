﻿using System;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController(CatalogDbContext catalogContext, IHostEnvironment environment) : Controller
{
    [HttpGet]
    [Route("items/type/all/brand/{catalogBrandId?}")]
    [ProducesResponseType(typeof(Catalog), StatusCodes.Status200OK)]
    public async Task<ActionResult<Catalog>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int pageSize = 8, [FromQuery] int pageIndex = 0)
    //public async Task<ActionResult<Catalog>> ItemsByBrandIdAsync(int? catalogBrandId, [FromQuery] int? before, [FromQuery] int? after, [FromQuery] int pageSize = 8)
    {
        //IQueryable<CatalogItem> root = catalogContext.CatalogItems;//.AsNoTracking();

        //if (catalogBrandId.HasValue)
        //{
        //    root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
        //}

        //var totalItems = await root
        //    .LongCountAsync();

        //var itemsOnPage = await root
        //    .Skip(pageSize * pageIndex)
        //    .Take(pageSize)
        //    .ToListAsync();

        var itemsOnPage = await catalogContext.GetCatalogItemsAsync(catalogBrandId, pageIndex, pageSize);
        var totalItems = await catalogContext.GetCatalogItemsCountAsync(catalogBrandId);

        //var totalItems = await GetTotalCatalogItemsQuery(catalogContext, catalogBrandId);
        //var itemsOnPage = await ToListAsync(GetCatalogItemsQuery(catalogContext, catalogBrandId, pageSize, pageIndex));

        return new Catalog(pageIndex, pageSize, totalItems, itemsOnPage);

        //var itemsOnPage = await catalogContext.CatalogItems
        //    .OrderBy(ci => ci.Id)
        //    .Where(ci => catalogBrandId == null || ci.CatalogBrandId == catalogBrandId)
        //    // https://learn.microsoft.com/ef/core/querying/pagination#keyset-pagination
        //    .Where(ci => before == null || ci.Id <= before)
        //    .Where(ci => after == null || ci.Id >= after)
        //    .Take(pageSize + 1)
        //    .ToListAsync();
        //var itemsOnPage = await catalogContext.GetCatalogItemsKeySetPagingAsync(catalogBrandId, before, after, pageSize);

        //var (firstId, nextId) = itemsOnPage switch
        //{
        //    [] => (0, 0),
        //    [var only] => (only.Id, only.Id),
        //    [var first, .., var last] => (first.Id, last.Id)
        //};

        //return new CatalogKeySet(firstId, nextId, itemsOnPage.Count < pageSize, itemsOnPage.Take(pageSize));
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

