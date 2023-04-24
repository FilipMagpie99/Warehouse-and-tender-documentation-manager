using Azure.Storage.Blobs;
using InzynierkaAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace InzynierkaAPI.Services
{
	public class ProducentService : IProducentService
	{

		async Task<IResult> IProducentService.CreateProducent(Producent producent, DataContext db)
		{
			var foundModel = db.Producent.Any(x => x.Krs == producent.Krs);
			if (!foundModel)
			{
				db.Producent.Add(producent);
				await db.SaveChangesAsync();
				return Results.Created($"/Producents/{producent.Id}", producent);
			}
			else
			{
				return Results.BadRequest();
			}
		}

		async Task<IResult> IProducentService.DeleteProducent(int id, DataContext db)
		{
			if (await db.Producent.FindAsync(id) is Producent producent)
			{
				db.Producent.Remove(producent);
				await db.SaveChangesAsync();
				return Results.Ok(producent);
			}

			return Results.NotFound();
		}

		async Task<IResult> IProducentService.UpdateProducent(int id, Producent producent, DataContext db)
		{
			var foundModel = await db.Producent.FindAsync(id);

			if (foundModel is null)
			{
				return Results.NotFound();
			}

			// tutaj należy uaktualnić dane encji foundModel na podstawie danych z producent

			await db.SaveChangesAsync();

			return Results.NoContent();
		}
	}
}
