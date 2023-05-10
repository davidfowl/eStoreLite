using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("CatalogDb") ??
        throw new InvalidDataException("Missing connection string CatalogDb");

    options.UseNpgsql(connectionString);
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

await app.Services.InitializeDatabaseAsync();

app.Run();
