using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using InzynierkaAPI.Services;

namespace InzynierkaAPI.Controllers;

public static class ProducentEndpoints
{
	private static int itemsOnPage { get; set; }

	public static void MapProducentEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/ProducentToList", async (DataContext db) =>
        {
            return await db.Producent.ToListAsync();
        })
        .WithName("GetAllProducenciToList");

		routes.MapGet("/api/ChangeNumberOfItemsPro/{items}", async (int items, DataContext db) =>
		{

			itemsOnPage = items;
			await Task.CompletedTask;
			return Results.Ok();
		})
        .WithName("ChangeNumberOfItemsPro");

		routes.MapGet("/api/FindProducent",
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int? page, string nazwaWystawcy, string nip, DataContext db) =>
{
var dostawcy = db.Producent.AsQueryable();

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
.WithName("FindProducent")
.Produces<List<Producent>>(StatusCodes.Status200OK);

		routes.MapGet("/api/Producent",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (int? page,DataContext db) =>
        {
			if (itemsOnPage == 0)
			{
				itemsOnPage = 5;
			}
			IQueryable<Producent> producenci = db.Producent;
			if (page != null)
				producenci = producenci.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);

			var ret = await producenci.ToListAsync();
			return ret;
        })
        .WithName("GetAllProducents");
        routes.MapGet("/api/Producent/{id}",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (int Id, DataContext db) =>
        {
            return await db.Producent.FindAsync(Id)
                is Producent model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetProducentById");

        routes.MapPut("/api/Producent/{id}", 
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (IProducentService service, int Id, Producent producent, DataContext db) =>
        {
			await service.UpdateProducent(Id,producent, db);

		})
        .WithName("UpdateProducent");

        routes.MapPost("/api/Producent/", 
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (IProducentService service, Producent producent, DataContext db) =>
        { 
            
            await service.CreateProducent( producent,  db);

		})
        .WithName("CreateProducent");

        routes.MapDelete("/api/Producent/{id}", 
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")] 
        async (IProducentService service, int Id, DataContext db) =>
        {
            await service.DeleteProducent(Id,db);
        })
        .WithName("DeleteProducent");
    }
}
