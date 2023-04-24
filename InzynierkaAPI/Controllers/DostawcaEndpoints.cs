using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InzynierkaAPI.Controllers;

public static class DostawcaEndpoints

{
	private static int itemsOnPage { get; set; }

	public static void MapDostawcaEndpoints (this IEndpointRouteBuilder routes)
    {
		routes.MapGet("/api/DostawcyLastPage", async (DataContext db) =>
		{
			return await db.Dostawca.ToListAsync();
		})
        .WithName("GetDostawcyLastPage");

		routes.MapGet("/api/DostawcaToList", async (DataContext db) =>
        {
            return await db.Dostawca.ToListAsync();
        })
        .WithName("DostawcaToList");

        routes.MapGet("/api/Dostawca",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int? page, DataContext db) =>
			{
				if (itemsOnPage == 0)
				{
					itemsOnPage = 5;
				}
				IQueryable<Dostawca> dostawcy = db.Dostawca;
				if (page != null)
					dostawcy = dostawcy.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);

				var ret = await dostawcy.ToListAsync();
				return ret;
        })
        .WithName("GetAllDostawcas");

        routes.MapGet("/api/Dostawca/{id}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int Id, DataContext db) =>
        {
            return await db.Dostawca.FindAsync(Id)
                is Dostawca model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetDostawcaById");

        routes.MapPut("/api/Dostawca/{id}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int Id, Dostawca dostawca, DataContext db) =>
        {
            var foundModel = await db.Dostawca.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }

            //update model properties here

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateDostawca");

        routes.MapPost("/api/Dostawca/", 
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (Dostawca dostawca, DataContext db) =>
        {
            var foundModel = db.Dostawca.Any(x=>x.Krs == dostawca.Krs);
            if (!foundModel)
            {
                db.Dostawca.Add(dostawca);
                await db.SaveChangesAsync();
                return Results.Created($"/Dostawcas/{dostawca.Id}", dostawca);
            }
            else {
                return Results.BadRequest();
            }
        })
        .WithName("CreateDostawca");

        routes.MapDelete("/api/Dostawca/{id}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (int Id, DataContext db) =>
        {
            if (await db.Dostawca.FindAsync(Id) is Dostawca dostawca)
            {
                db.Dostawca.Remove(dostawca);
                await db.SaveChangesAsync();
                return Results.Ok(dostawca);
            }

            return Results.NotFound();
        })
        .WithName("DeleteDostawca");
		routes.MapGet("/api/ChangeNumberOfItemsD/{items}", async (int items, DataContext db) =>
		{

			itemsOnPage = items;
			await Task.CompletedTask;
			return Results.Ok();
		})
.WithName("ChangeNumberOfItemsD");



		routes.MapGet("/api/FindDostawca",
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int? page, string nazwaWystawcy, string nip, DataContext db) =>
   {
	   var dostawcy = db.Dostawca.AsQueryable();

	   if (!string.IsNullOrEmpty(nazwaWystawcy))
	   {
		   if (page != null)
		   {

			   dostawcy = dostawcy.Where(x => x.Nazwa.ToLower().Contains(nazwaWystawcy.ToLower()) || x.Miejscowosc.ToLower().Contains(nazwaWystawcy.ToLower()));
			   dostawcy = dostawcy.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);
		   }

	   }
	   if (!string.IsNullOrEmpty(nip))
	   {
		   if (page != null)
		   {
			   dostawcy = dostawcy.Where(x => x.Nip == nip || x.Krs == nip);
			   dostawcy = dostawcy.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);
		   }

	   }
	   return await dostawcy.ToListAsync();
   })
		.WithName("FindDostawca")
		.Produces<List<Dostawca>>(StatusCodes.Status200OK);

	}

}
