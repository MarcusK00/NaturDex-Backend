using Microsoft.EntityFrameworkCore;
using NaturDex.Core.Interfaces;
using NaturDex.Core.Models;
using NaturDex.Core.Repositories;
using NaturDex.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// Use connection string from appsettings.json or environment variable
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? Environment.GetEnvironmentVariable("DATABASE_URL");

// If the URL is in the postgres:// or postgresql:// format (like Render), convert it to Npgsql format
// If the URL is in the postgres:// or postgresql:// format (like Render), convert it to Npgsql format
if (connectionString != null &&
    (connectionString.StartsWith("postgres://") || connectionString.StartsWith("postgresql://")))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');

    var port = uri.Port > 0 ? uri.Port : 5432; // <-- default to 5432 if port is -1

    var builderNpgsql = new Npgsql.NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = port,
        Username = userInfo[0],
        Password = userInfo[1],
        Database = uri.AbsolutePath.TrimStart('/'),
        SslMode = Npgsql.SslMode.Require,
        TrustServerCertificate = true
    };

    connectionString = builderNpgsql.ToString();
}

// Register DbContext with the converted connection string
builder.Services.AddDbContext<NaturDexDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add repository and OpenAPI services
builder.Services.AddScoped<IRepository<Animal>, AnimalRepository>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Development OpenAPI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// --- API Endpoints ---
app.MapGet("/api/v1/animals/{id}", async (int id, IRepository<Animal> repo) =>
{
    if (id <= 0) return Results.BadRequest("Invalid animal ID");

    var animal = await repo.GetByIdAsync(id);
    return animal is null ? Results.NotFound($"Animal with ID {id} not found") : Results.Ok(animal);
});

app.MapGet("/api/v1/animals", async (IRepository<Animal> repo) =>
{
    var animals = await repo.GetAllAsync();
    return animals is null || animals.Count == 0 ? Results.NotFound("No animals found") : Results.Ok(animals);
});

app.MapPost("/api/v1/animals", async (Animal animal, IRepository<Animal> repo) =>
{
    if (animal is null) return Results.BadRequest("Animal is required");
    if (string.IsNullOrWhiteSpace(animal.Species)) return Results.BadRequest("Animal name is required");

    var createdAnimal = await repo.CreateAsync(animal);
    return Results.Created($"/api/v1/animals/{createdAnimal.Id}", createdAnimal);
});

app.MapPut("/api/v1/animals/{id}", async (int id, Animal animal, IRepository<Animal> repo) =>
{
    if (animal is null) return Results.BadRequest("Animal is required");

    animal.Id = id;
    var updatedAnimal = await repo.UpdateAsync(animal);
    return Results.Ok(updatedAnimal);
});

app.MapDelete("/api/v1/animals/{id}", async (int id, IRepository<Animal> repo) =>
{
    if (id <= 0) return Results.BadRequest("Id must be over 0");

    await repo.DeleteAsync(id);
    return Results.NoContent();
});

app.Run();