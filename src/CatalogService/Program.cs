using CatalogService.CompiledModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<CatalogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("CatalogDb") ??
        throw new InvalidDataException("Missing connection string CatalogDb");

    options.UseNpgsql(connectionString);
    options.EnableThreadSafetyChecks(enableChecks: false);
    options.UseModel(CatalogDbContextModel.Instance);
});

//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapCatalogApi();

app.MapControllers();

await app.Services.InitializeDatabaseAsync();

app.Run();
