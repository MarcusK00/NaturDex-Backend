using Microsoft.EntityFrameworkCore;
using NaturDex.Core.Interfaces;
using NaturDex.Core.Models;
using NaturDex.Core.Repositories;
using System;
using NaturDex.Core.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<NaturDexDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IRepository<Animal>, AnimalRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//API ENDPOINTS
app.MapGet("/api/v1/animals/{id}", async (int id, IRepository<Animal> repo) =>
{
    try
    {
        if (id <= 0)
            return Results.BadRequest("Invalid animal ID");

        var animal = await repo.GetByIdAsync(id);

        if (animal is null)
            return Results.NotFound($"Animal with ID {id} not found");

        return Results.Ok(animal);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex);
    }
});

app.MapGet("/api/v1/animals", async (IRepository<Animal> repo) =>
{
    try
    {

        var animals = await repo.GetAllAsync();
        
        if (animals == null || animals.Count == 0)
            return Results.NotFound("No animals found");

        return Results.Ok(animals);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex);
    }
});

app.MapPost("/api/v1/animals", async (Animal animal, IRepository<Animal> repo) =>
{
    try
    {
        if (animal is null)
            return Results.BadRequest("Animal is required");

        if (string.IsNullOrWhiteSpace(animal.Species))
            return Results.BadRequest("Animal name is required");

        var createdAnimal = await repo.CreateAsync(animal);
        return Results.Created($"/api/v1/animals/{createdAnimal.Id}", createdAnimal);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex);
    }
});

app.MapPut("/api/v1/animals/{id}", async (int id, Animal animal, IRepository<Animal> repo) =>
{
    try
    {
        if (animal is null) 
            return Results.BadRequest("Animal is required");

        animal.Id = id;
        var updatedAnimal = await repo.UpdateAsync(animal);

        return Results.Ok(updatedAnimal); 
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex);
    }
});

app.MapDelete("/api/v1/animals/{id}", async (int id, IRepository<Animal> repo) =>
{
    try
    {
        if (id <= 0) 
            return Results.BadRequest("Id must be over 0");

        await repo.DeleteAsync(id);

        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex);
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();
