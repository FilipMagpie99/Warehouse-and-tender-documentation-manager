using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Xunit.Sdk;

namespace InzynierkaAPI.Controllers;

public static class WystawcaPrzetarguEndpoints
{
	private static int itemsOnPage { get; set; }

	public static void MapWystawcaPrzetarguEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/WystawcaPrzetarguToList", async (DataContext db) =>
        {
            return await db.WystawcaPrzetargu.ToListAsync();
        })
        .WithName("GetAllWystawcaPrzetarguToList");

        routes.MapGet("/api/WystawcaPrzetargu",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int? page,DataContext db) =>
        {
			if (itemsOnPage == 0)
			{
				itemsOnPage = 5;
			}
			IQueryable<WystawcaPrzetargu> wystawcy = db.WystawcaPrzetargu;
			if (page != null)
				wystawcy = wystawcy.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);

			var ret = await wystawcy.ToListAsync();
			return ret;
        })
        .WithName("GetAllWystawcaPrzetargus");

        routes.MapGet("/api/ChangeNumberOfItemsW/{items}", async (int items, DataContext db) =>
        {

            itemsOnPage = items;
            await Task.CompletedTask;
            return Results.Ok();
        })
        .WithName("ChangeNumberOfItemsW");


        routes.MapGet("/api/FindWystawca",
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int? page, string nazwaWystawcy, string nip, DataContext db) =>
   {
       var wystawcy = db.WystawcaPrzetargu.AsQueryable();

       if (!string.IsNullOrEmpty(nazwaWystawcy))
       {
           if (page != null)
           {

               wystawcy = wystawcy.Where(x => x.Nazwa.ToLower().Contains(nazwaWystawcy.ToLower()) || x.Miejscowosc.ToLower().Contains(nazwaWystawcy.ToLower()));
               wystawcy = wystawcy.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);
           }

       }
       if (!string.IsNullOrEmpty(nip))
       {
           if (page != null)
           {
               wystawcy = wystawcy.Where(x => x.Nip == nip || x.Krs == nip);
               wystawcy = wystawcy.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);
           }

       }
       return await wystawcy.ToListAsync();
   })
        .WithName("FindWystawca")
        .Produces<List<WystawcaPrzetargu>>(StatusCodes.Status200OK);

        routes.MapGet("/api/WystawcyLastPage", async (DataContext db) =>
		{
			return await db.WystawcaPrzetargu.ToListAsync();
		})
.WithName("GetWystawcyForLastPage");

		routes.MapGet("/api/WystawcaPrzetargu/{id}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (int Id, DataContext db) =>
        {
            return await db.WystawcaPrzetargu.SingleOrDefaultAsync(x=>x.Id==Id)
                is WystawcaPrzetargu model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetWystawcaPrzetarguById");

        routes.MapGet("/api/WystawcaKrs/{krs}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (string krs, DataContext db) =>
        {
            try {
                if(!long.TryParse(krs, out _))
                {
                    return Results.BadRequest(error:"Nie znaleziono firmy.");
                }
                return Results.Ok(await new HttpClient().GetFromJsonAsync<KrsApi>(string.Format("https://api-krs.ms.gov.pl/api/krs/OdpisAktualny/{0}?rejestr=P&format=json", krs)));
            }
            catch
            {
                return Results.BadRequest(error:"Nie znaleziono firmy.");
            }
        })
        .WithName("GetWystawcaKRS");

        routes.MapPut("/api/WystawcaPrzetargu/{id}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (int Id, WystawcaPrzetargu wystawcaPrzetargu, DataContext db) =>
        {
            var foundModel = await db.WystawcaPrzetargu.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }
            //update model properties here

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateWystawcaPrzetargu");

        routes.MapPost("/api/WystawcaPrzetargu/",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (WystawcaPrzetargu wystawcaPrzetargu, DataContext db) =>
        {
        var foundModel = db.WystawcaPrzetargu.Any(x => x.Krs == wystawcaPrzetargu.Krs);
            if (!foundModel)
            {
            db.WystawcaPrzetargu.Add(wystawcaPrzetargu);
            await db.SaveChangesAsync();
            return Results.Created($"/WystawcaPrzetargus/{wystawcaPrzetargu.Id}", wystawcaPrzetargu);
            }
            else
            {
            return Results.BadRequest();
            }
        })
        .WithName("CreateWystawcaPrzetargu");

        routes.MapDelete("/api/WystawcaPrzetargu/{id}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (int Id, DataContext db) =>
        {
            if (await db.WystawcaPrzetargu.FindAsync(Id) is WystawcaPrzetargu wystawcaPrzetargu)
            {
                db.WystawcaPrzetargu.Remove(wystawcaPrzetargu);
                await db.SaveChangesAsync();
                return Results.Ok(wystawcaPrzetargu);
            }

            return Results.NotFound();
        })
        .WithName("DeleteWystawcaPrzetargu");
    }
}
