using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
namespace InzynierkaAPI.Controllers;

public static class MagazynEndpoints
{
    public static void MapMagazynEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/Magazyn", async (DataContext db) =>
        {
            return await db.Magazyn.ToListAsync();
        })
        .WithName("GetAllMagazyns");

        routes.MapGet("/api/Magazyn/{id}", async (int Id, DataContext db) =>
        {
            return await db.Magazyn.FindAsync(Id)
                is Magazyn model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetMagazynById");

        routes.MapPut("/api/Magazyn/{id}", async (int Id, Magazyn magazyn, DataContext db) =>
        {
            var foundModel = await db.Magazyn.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }
            //update model properties here
            var last = magazyn.Strefa.Last();

			foundModel.Strefa.Add(last);
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateMagazyn");

        routes.MapPost("/api/Magazyn/", async (Magazyn magazyn, DataContext db) =>
        {
            db.Magazyn.Add(magazyn);
            await db.SaveChangesAsync();
            return Results.Created($"/Magazyns/{magazyn.Id}", magazyn);
        })
        .WithName("CreateMagazyn");

        routes.MapDelete("/api/Magazyn/{id}", async (int Id, DataContext db) =>
        {
            if (await db.Magazyn.FindAsync(Id) is Magazyn magazyn)
            {
                db.Magazyn.Remove(magazyn);
                await db.SaveChangesAsync();
                return Results.Ok(magazyn);
            }

            return Results.NotFound();
        })
        .WithName("DeleteMagazyn");
    }
}
