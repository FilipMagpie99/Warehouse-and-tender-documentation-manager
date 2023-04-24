using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace InzynierkaAPI.Controllers;

public static class RaportMagazynuEndpoints
{
    public static void MapRaportMagazynuEndpoints (this IEndpointRouteBuilder routes)
    {


        routes.MapGet("/api/RaportMagazynow", async (DataContext db) =>
        {
            var raport = db.RaportMagazynu.AsQueryable();
            //raport = raport.TakeLast(5);

            return await raport.ToListAsync();
        })
        .WithName("GetAllRaportMagazynus");        
        
     

        routes.MapGet("/api/RaportMagazyn/{id}", async (int Id, DataContext db) =>
        {
            return await db.RaportMagazynu.FindAsync(Id)
                is RaportMagazynu model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetRaportMagazynuById");         
        
        routes.MapGet("/api/RaportMagazynu", async (int idproduktu, int? page, DataContext db) =>
        {
			IQueryable<RaportMagazynu> raport = db.RaportMagazynu;
			if (page != null)
				raport = raport.Where(x => x.ProduktId == idproduktu).OrderByDescending(x => x.Id).Skip((page.Value - 1) * 10).Take(10);
			var ret = await raport.ToListAsync();
			return ret;
			//.Skip(raport.Count() - 5).OrderByDescending(x => x.Id)
		})
        .WithName("GetRaportMagazynuByProdukt");        


    }
}
