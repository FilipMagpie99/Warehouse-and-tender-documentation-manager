using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
namespace InzynierkaAPI.Controllers;

public static class KategoriaPlikEndpoints
{
    public static void MapKategoriaPlikEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/KategoriaPlik", async (DataContext db) =>
        {
            return await db.KategoriaPlik.ToListAsync();
        })
        .WithName("GetAllKategoriaPliks");

        routes.MapGet("/api/KategoriaPlik/{id}", async (int Id, DataContext db) =>
        {
            return await db.KategoriaPlik.FindAsync(Id)
                is KategoriaPlik model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetKategoriaPlikById");

        routes.MapPut("/api/KategoriaPlik/{id}", async (int Id, KategoriaPlik kategoriaPlik, DataContext db) =>
        {
            var foundModel = await db.KategoriaPlik.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }
            //update model properties here

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateKategoriaPlik");

        routes.MapPost("/api/KategoriaPlik/", async (KategoriaPlik kategoriaPlik, DataContext db) =>
        {
            db.KategoriaPlik.Add(kategoriaPlik);
            await db.SaveChangesAsync();
            return Results.Created($"/KategoriaPliks/{kategoriaPlik.Id}", kategoriaPlik);
        })
        .WithName("CreateKategoriaPlik");

        routes.MapDelete("/api/KategoriaPlik/{id}", async (int id, DataContext db) =>
        {
            if (await db.KategoriaPlik.FindAsync(id) is KategoriaPlik kategoriaPlik)
            {
                db.KategoriaPlik.Remove(kategoriaPlik);
                await db.SaveChangesAsync();
                return Results.Ok(kategoriaPlik);
            }

            return Results.NotFound();
        })
        .WithName("DeleteKategoriaPlik");
    }
}
