using Azure.Storage.Blobs;
using InzynierkaAPI.Models;
using InzynierkaAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace InzynierkaAPI.Controllers;

public static class PrzetargEndpoints
{
    private static int itemsOnPage { get; set; }
    public static void MapPrzetargEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/przetargi",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int? page, DataContext db, BlobServiceClient blobServiceClient) =>
        {
                if (itemsOnPage == 0)
                {
                    itemsOnPage = 5;
                }
                IQueryable<Przetarg> przetargi = db.Przetarg.Include(x => x.Pliki);

                if (page != null)
                    przetargi = przetargi.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);

                return await przetargi.ToListAsync();
        })
        .WithName("przetargi")
        .Produces<List<Przetarg>>(StatusCodes.Status200OK);

        routes.MapGet("/api/przetargi/lastpage",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (DataContext db) =>
        {
            return await db.Przetarg.ToListAsync();
        })
        .WithName("przetargi/lastpage");

        routes.MapGet("/api/przetargi/ilosc/{items}",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int items, DataContext db) =>
        {

            itemsOnPage = items;
            await Task.CompletedTask;
            return Results.Ok();
        })
        .WithName("przetargi/ilosc");

        routes.MapGet("/api/PrzetargWystawca",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int idwystawcy, DataContext db, BlobServiceClient blobServiceClient) =>
        {

                IQueryable<Przetarg> przetargi = db.Przetarg.Where(x => x.WystawcaPrzetarguId == idwystawcy);
                return await przetargi.ToListAsync();
        })
        .WithName("GetPrzetargByWystawcaId")
        .Produces<List<Przetarg>>(StatusCodes.Status200OK);

        routes.MapGet("/api/przetargi/{id}",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int id, DataContext db, BlobServiceClient blobServiceClient) =>
        {
                return await db.Przetarg.Include(x => x.Pliki).SingleOrDefaultAsync(x => x.Id == id)
                    is Przetarg model
                        ? Results.Ok(model)
                        : Results.NotFound();
        })
        .WithName("GetPrzetargById")
        .Produces<Przetarg>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routes.MapPut("/api/przetargi/status/{id}",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int id, Przetarg przetarg, DataContext db) =>
        {
                var foundModel = await db.Przetarg.FindAsync(id);

                if (foundModel is null)
                {
                    return Results.NotFound();
                }
                foundModel.Status = przetarg.Status;
                await db.SaveChangesAsync();

                return Results.NoContent();
        })
        .WithName("przetargi/status")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        routes.MapPut("/api/przetargi/{id}",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int id, Przetarg przetarg, DataContext db) =>
        {
            var foundModel = await db.Przetarg.FindAsync(id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }
            foundModel.Lokalizacja = przetarg.Lokalizacja;
            foundModel.DataPrzetargu = przetarg.DataPrzetargu;
            foundModel.DataUtworzenia = przetarg.DataUtworzenia;
            foundModel.PrzedmiotOgloszenia = przetarg.PrzedmiotOgloszenia;
            foundModel.Notatki = przetarg.Notatki;
            foundModel.Status = przetarg.Status;
            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("przetargi/{id}")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);

        routes.MapPost("/api/przetargi/",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (Przetarg przetarg, DataContext db, BlobServiceClient blobServiceClient) =>
            {
                try
                {
                    przetarg.Status = Status.Niezweryfikowany;
                    przetarg.Notatki = "";
                    db.Przetarg.Add(przetarg);
                    await db.SaveChangesAsync();
                    return Results.Created($"/Przetargs/{przetarg.Id}", przetarg);
                }
                catch (Exception ex)
                {
                    return Results.StatusCode(400);
                }
            })
        .WithName("CreatePrzetarg");

        routes.MapDelete("/api/przetargi/{id}",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int id, IPlikiService fileService, DataContext db, BlobServiceClient blobServiceClient) =>
            {
                if (await db.Przetarg.SingleOrDefaultAsync(x => x.Id == id) is Przetarg przetarg)
                {
                    var pliki = przetarg.Pliki?.ToList() ?? new List<Plik>();
                    db.Przetarg.Remove(przetarg);
                    await db.SaveChangesAsync();
                    foreach (var plik in pliki)
                        await fileService.DeleteBlobAsync(plik, blobServiceClient);
                    return Results.Ok(przetarg);
                }

                return Results.NotFound();
            })
        .WithName("DeletePrzetarg")
        .Produces<Przetarg>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        routes.MapGet("/api/FindPrzetarg",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int? page, int rok, string nazwa, int miesiac, string status, DataContext db, BlobServiceClient blobServiceClient) =>
        {
        var przetargi = db.Przetarg.AsQueryable();
        if (rok != 0)
        {
            if (page != null)
            {
                przetargi = przetargi.Where(x => x.DataPrzetargu.Year == rok);
                przetargi = przetargi.Skip((page.Value - 1) * 5).Take(5);
            }
        }
        if (miesiac != 0)
        {
            if (page != null)
            {
                przetargi = przetargi.Where(x => x.DataPrzetargu.Month == miesiac);
                przetargi = przetargi.Skip((page.Value - 1) * 5).Take(5);
            }

        }
        if (!string.IsNullOrEmpty(nazwa))
        {
            if (page != null)
            {
                przetargi = przetargi.Where(x => x.WystawcaPrzetargu.Nazwa.ToLower().Contains(nazwa.ToLower()) || x.PrzedmiotOgloszenia.ToLower().Contains(nazwa.ToLower()));
                przetargi = przetargi.Skip((page.Value - 1) * 5).Take(5);
            }
        }
        if (!string.IsNullOrEmpty(status))
        {
            if (page != null)
            {

                przetargi = przetargi.Where(x => x.Status == (Status)Enum.Parse(typeof(Status), status));
                przetargi = przetargi.Skip((page.Value - 1) * 5).Take(5);
            }

        }
        return await przetargi.ToListAsync();
        })
        .WithName("FindPrzetarg")
        .Produces<List<Przetarg>>(StatusCodes.Status200OK);

        routes.MapPost("/api/Blob",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (IPlikiService filesService, HttpRequest request, DataContext db, BlobServiceClient blobServiceClient) =>
        {
            return await filesService.UploadFile(request, db, blobServiceClient);
        })
        .Accepts<IFormFile>("multipart/form-data")
        .WithName("PostBlob");

        routes.MapGet("/api/Blob/{guid}",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (IPlikiService filesService, Guid guid, DataContext db, BlobServiceClient blobServiceClient) =>
        {
            return await filesService.DownloadFile(guid, db, blobServiceClient);

        })
        .WithName("GetBlob");

        routes.MapDelete("/api/DeleteBlob/{id}",
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (Guid id, IPlikiService fileService, DataContext db, BlobServiceClient blobServiceClient) =>
        {
            await fileService.DeleteBlobAsync(await db.Plik.FindAsync(id), blobServiceClient, db);
        })
        .WithName("DeleteBlob")
        .Produces<Plik>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
