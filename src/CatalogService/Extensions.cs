using Npgsql;

namespace CatalogService;

public static class Extensions
{
    public static IServiceCollection AddCatalogDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var connectionString = configuration.GetConnectionString("CatalogDb") ??
                throw new InvalidDataException("Missing connection string CatalogDb");

            return new NpgsqlDataSourceBuilder(connectionString).Build();
        });

        services.AddSingleton<ICatalogDb, CatalogDb>();
        return services;
    }
}
