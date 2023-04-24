using Azure.Storage.Blobs;
using InzynierkaAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Design;
using Newtonsoft.Json.Linq;
using QRCoder;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Xml.Linq;

namespace InzynierkaAPI.Controllers;

public static class ProduktEndpoints
{
	private static int itemsOnPage { get; set; }


    public static void MapProduktEndpoints(this IEndpointRouteBuilder routes)
	{
		routes.MapGet("/api/ProduktDostawca",
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int iddostawcy, DataContext db, BlobServiceClient blobServiceClient) =>
	{

		IQueryable<Produkt> produkty = db.Produkt.Where(x => x.RaportMagazynu.Any(x=> x.DostawcaId == iddostawcy));
		var ret = await produkty.ToListAsync();


		return ret;
	})
.WithName("ProduktDostawca")
.Produces<List<Produkt>>(StatusCodes.Status200OK);		
		
		routes.MapGet("/api/ProduktProducent",
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int idproducenta, DataContext db, BlobServiceClient blobServiceClient) =>
	{

		IQueryable<Produkt> produkty = db.Produkt.Where(x => x.ProducentId == idproducenta);
		var ret = await produkty.ToListAsync();


		return ret;
	})
.WithName("ProduktProducent")
.Produces<List<Produkt>>(StatusCodes.Status200OK);

        routes.MapGet("/api/Produkt",
            [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
        async (int? page, DataContext db) =>
            {
                if (itemsOnPage == 0)
                {
                    itemsOnPage = 5;
                }
                IQueryable<Produkt> produkty = db.Produkt;
                if (page != null)
                    produkty = produkty.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);

                // Use the "await" keyword to asynchronously retrieve the data from the database
                var ret = await produkty.ToListAsync();
                return ret;
            })
        .WithName("GetAllProdukts");


        routes.MapGet("/api/ProduktyLastPage", async (DataContext db) =>
		{
			return await db.Produkt.ToListAsync();
		})
.WithName("GetProduktyLastPage");
		routes.MapGet("/api/FindProdukt",
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int? page, string nazwaProduktu, string typProduktu, string producent, DataContext db) =>
   {
	   var produkty = db.Produkt.AsQueryable();

	   if (!string.IsNullOrEmpty(nazwaProduktu))
	   {
		   if (page != null)
		   {

			   produkty = produkty.Where(x => x.Nazwa.ToLower().Contains(nazwaProduktu.ToLower()));
			   produkty = produkty.Skip((page.Value - 1) * 5).Take(5);
		   }

	   }
	   if (!string.IsNullOrEmpty(producent))
	   {
		   if (page != null)
		   {
			   produkty = produkty.Where(x => x.Producent.Nazwa.ToLower().Contains(producent.ToLower()));
			   produkty = produkty.Skip((page.Value - 1) * 5).Take(5);
		   }

	   }

	   if (!string.IsNullOrEmpty(typProduktu))
	   {
		   if (page != null)
		   {
			   produkty = produkty.Where(x => x.TypProduktu.ToLower().Contains(typProduktu.ToLower()));
			   produkty = produkty.Skip((page.Value - 1) * 5).Take(5);
		   }
	   }
	   return await produkty.ToListAsync();
   })
		.WithName("FindProdukt")
		.Produces<List<Produkt>>(StatusCodes.Status200OK);
		routes.MapGet("/api/ChangeNumberOfItemsP/{items}", async (int items, DataContext db) =>
		{

			itemsOnPage = items;
			await Task.CompletedTask;
			return Results.Ok();
		})
.WithName("ChangeNumberOfItemsP");
		routes.MapGet("/api/Produkt/{id}",
			[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int Id, DataContext db) =>
		{
			return await db.Produkt.FindAsync(Id)
				is Produkt model
					? Results.Ok(model)
					: Results.NotFound();
		})
		.WithName("GetProduktById");

		routes.MapPut("/api/Produkt/{id}",
			[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int Id, Produkt produkt, DataContext db) =>
		{
			var foundModel = await db.Produkt.FindAsync(Id);

			if (foundModel is null)
			{
				return Results.NotFound();
			}
			//update model properties here

			await db.SaveChangesAsync();

			return Results.NoContent();
		})
		.WithName("UpdateProdukt");

		routes.MapPost("/api/Produkt/",
			[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (Produkt produkt, DataContext db) =>
		{
			db.Produkt.Add(produkt);
			await db.SaveChangesAsync();
			return Results.Created($"/Produkts/{produkt.Id}", produkt);
		})
		.WithName("CreateProdukt");		
		


		routes.MapDelete("/api/Produkt/{id}",
			[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (int Id, DataContext db) =>
		{
			if (await db.Produkt.FindAsync(Id) is Produkt produkt)
			{
				db.Produkt.Remove(produkt);
				await db.SaveChangesAsync();
				return Results.Ok(produkt);
			}

			return Results.NotFound();
		})
		.WithName("DeleteProdukt");

		routes.MapGet("/api/QR/{id}",
			[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		(int id, DataContext db, BlobServiceClient blobServiceClient) =>
	{
		using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
		using (QRCodeData qrCodeData = qrGenerator.CreateQrCode("https://przetargi.azurewebsites.net/getprodukt/" + id.ToString(), QRCodeGenerator.ECCLevel.Q))
		using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
		{
			var qrCodeImage = qrCode.GetGraphic(20);
			return Results.File(qrCodeImage, fileDownloadName: "barcode.png");
		}
	})
		.WithName("GetQR");

		routes.MapPut("/api/ChangeProdukt",
			[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Administrator")]
		async (RaportMagazynu raportMagazynu, DataContext db) =>
			{

				var foundModel = await db.Produkt.FindAsync(raportMagazynu.ProduktId);


				if (foundModel is null)
				{
					return Results.NotFound();
					;
				}
				if (raportMagazynu.TypOperacji == TypOperacji.Wydanie && foundModel.Suma - raportMagazynu.Ilosc < 0)
				{
					return Results.BadRequest("Zabroniona operacja, nie posiadasz wystarczającej ilości wybranego produktu");
					
				}
				if(raportMagazynu.TypOperacji == TypOperacji.Wydanie)
				{
					var produkt = await db.Produkt.FindAsync(raportMagazynu.ProduktId);
					int iloscWStrefie =  produkt.RaportMagazynu.Where(x => x.StrefaId == raportMagazynu.StrefaId).Where(x => x.TypOperacji == TypOperacji.Przyjecie).Select(x => x.Ilosc).Sum()
                                     - produkt.RaportMagazynu.Where(x => x.StrefaId == raportMagazynu.StrefaId).Where(x => x.TypOperacji == TypOperacji.Wydanie).Select(x => x.Ilosc).Sum();
					if(raportMagazynu.Ilosc > iloscWStrefie)
					{
						return Results.BadRequest("Zabroniona operacja, nie posiadasz wystarczającej ilości wybranego produktu");
					}
                }


            // Use the JwtSecurityTokenHandler to read the JWT token from the Program instance
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = Program.token;

            // Get the current claims from the JWT token
            var currentPrincipal = jwtToken.Claims;
				if (currentPrincipal.Any())
				{
					// Get the username claim from the current principal
					var usernameClaim = currentPrincipal.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
					if (usernameClaim != null)
					{
						// Use the value of the username claim to look up the user in the database
						Guid userId = (await db.User.SingleAsync(x => x.Username.Equals(usernameClaim.Value))).Id;
						raportMagazynu.UserId = userId;
						// ...

					}
				}




                    /*				var claim = System.Security.Claims.ClaimsPrincipal.Current.Claims.First().Value;

                                    Guid userId = (await db.User.SingleAsync(x => x.Username.Equals(claim))).Id;
                                    raportMagazynu.UserId = userId;*/
                    db.RaportMagazynu.Add(raportMagazynu);
				await db.SaveChangesAsync();

				return Results.NoContent();
			})
		.WithName("ChangeProdukt")
		.Produces(StatusCodes.Status404NotFound)
		.Produces(StatusCodes.Status204NoContent);


		routes.MapGet("/api/ProduktyByStrefa",
		async (int? page, int idStrefy, DataContext db) =>
			{
				if (itemsOnPage == 0)
				{
					itemsOnPage = 5;
				}
				IQueryable<Produkt> produkty = db.Produkt;

				produkty = produkty.Where(x => x.RaportMagazynu.Any(s => s.StrefaId == idStrefy) &&
					x.RaportMagazynu.Where(o => o.TypOperacji == TypOperacji.Przyjecie && o.StrefaId == idStrefy).Select(x => x.Ilosc).Sum()
					- x.RaportMagazynu.Where(o => o.TypOperacji == TypOperacji.Wydanie && o.StrefaId == idStrefy).Select(x => x.Ilosc).Sum() > 0);

				if (page != null)
					produkty = produkty.Skip((page.Value - 1) * itemsOnPage).Take(itemsOnPage);


				var ret = await produkty.ToListAsync();
				return ret;
			})
		.WithName("ProduktyByStrefa");



	}
}
