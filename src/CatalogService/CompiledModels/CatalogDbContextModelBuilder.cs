﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace CatalogService.CompiledModels
{
    public partial class CatalogDbContextModel
    {
        partial void Initialize()
        {
            var catalogBrand = CatalogBrandEntityType.Create(this);
            var catalogItem = CatalogItemEntityType.Create(this);
            var catalogType = CatalogTypeEntityType.Create(this);

            CatalogItemEntityType.CreateForeignKey1(catalogItem, catalogBrand);
            CatalogItemEntityType.CreateForeignKey2(catalogItem, catalogType);

            CatalogBrandEntityType.CreateAnnotations(catalogBrand);
            CatalogItemEntityType.CreateAnnotations(catalogItem);
            CatalogTypeEntityType.CreateAnnotations(catalogType);

            var sequences = new SortedDictionary<(string, string?), ISequence>();
            var catalog_brand_hilo = new RuntimeSequence(
                "catalog_brand_hilo",
                this,
                typeof(long),
                incrementBy: 10);

            sequences[("catalog_brand_hilo", null)] = catalog_brand_hilo;

            var catalog_hilo = new RuntimeSequence(
                "catalog_hilo",
                this,
                typeof(long),
                incrementBy: 10);

            sequences[("catalog_hilo", null)] = catalog_hilo;

            var catalog_type_hilo = new RuntimeSequence(
                "catalog_type_hilo",
                this,
                typeof(long),
                incrementBy: 10);

            sequences[("catalog_type_hilo", null)] = catalog_type_hilo;

            AddAnnotation("Relational:Sequences", sequences);
            AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            AddAnnotation("ProductVersion", "7.0.5");
            AddAnnotation("Relational:MaxIdentifierLength", 63);
        }
    }
}