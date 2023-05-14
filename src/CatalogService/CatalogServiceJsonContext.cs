using System.Text.Json.Serialization;

namespace CatalogService;

// https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation

[JsonSerializable(typeof(Catalog))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class CatalogServiceJsonContext : JsonSerializerContext { }
