START TRANSACTION;

CREATE SEQUENCE catalog_brand_hilo START WITH 1 INCREMENT BY 10 NO MINVALUE NO MAXVALUE NO CYCLE;

CREATE SEQUENCE catalog_hilo START WITH 1 INCREMENT BY 10 NO MINVALUE NO MAXVALUE NO CYCLE;

CREATE SEQUENCE catalog_type_hilo START WITH 1 INCREMENT BY 10 NO MINVALUE NO MAXVALUE NO CYCLE;

CREATE TABLE "CatalogBrand" (
    "Id" integer NOT NULL,
    "Brand" character varying(100) NOT NULL,
    CONSTRAINT "PK_CatalogBrand" PRIMARY KEY ("Id")
);

CREATE TABLE "CatalogType" (
    "Id" integer NOT NULL,
    "Type" character varying(100) NOT NULL,
    CONSTRAINT "PK_CatalogType" PRIMARY KEY ("Id")
);

CREATE TABLE "Catalog" (
    "Id" integer NOT NULL,
    "Name" character varying(50) NOT NULL,
    "Description" text,
    "Price" numeric NOT NULL,
    "PictureFileName" text,
    "CatalogTypeId" integer NOT NULL,
    "CatalogBrandId" integer NOT NULL,
    "AvailableStock" integer NOT NULL,
    "RestockThreshold" integer NOT NULL,
    "MaxStockThreshold" integer NOT NULL,
    "OnReorder" boolean NOT NULL,
    CONSTRAINT "PK_Catalog" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Catalog_CatalogBrand_CatalogBrandId" FOREIGN KEY ("CatalogBrandId") REFERENCES "CatalogBrand" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Catalog_CatalogType_CatalogTypeId" FOREIGN KEY ("CatalogTypeId") REFERENCES "CatalogType" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Catalog_CatalogBrandId" ON "Catalog" ("CatalogBrandId");

CREATE INDEX "IX_Catalog_CatalogTypeId" ON "Catalog" ("CatalogTypeId");

COMMIT;

