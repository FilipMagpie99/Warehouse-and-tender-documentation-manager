using Microsoft.EntityFrameworkCore;
using InzynierkaAPI;
using InzynierkaAPI.Data;
namespace InzynierkaAPI.Controllers;

public static class PlikEndpoints
{
    public static void MapPlikEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Plik", async (DataContext db) =>
        {
            return await db.Plik.ToListAsync();
        })
        .WithName("GetAllPliks")
        .Produces<List<Plik>>(StatusCodes.Status200OK);

        routes.MapGet("/api/Plik/{id}", async (Guid Id, DataContext db) =>
        {
            return await db.Plik.FindAsync(Id)
                is Plik model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetPlikById")
        .Produces<Plik>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routes.MapPut("/api/Plik/{id}", async (Guid Id, Plik plik, DataContext db) =>
        {
            var foundModel = await db.Plik.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }
            //update model properties here

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdatePlik")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        routes.MapPost("/api/Plik/", async (Plik plik, DataContext db) =>
        {
            db.Plik.Add(plik);

            await db.SaveChangesAsync();
            return Results.Created($"/Pliks/{plik.Id}", plik);
        })
        .WithName("CreatePlik")
        .Produces<Plik>(StatusCodes.Status201Created);

        routes.MapDelete("/api/Plik/{id}", async (Guid Id, DataContext db) =>
        {
            if (await db.Plik.FindAsync(Id) is Plik plik)
            {
                db.Plik.Remove(plik);
                await db.SaveChangesAsync();
                return Results.Ok(plik);
            }

            return Results.NotFound();
        })
        .WithName("DeletePlik")
        .Produces<Plik>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
