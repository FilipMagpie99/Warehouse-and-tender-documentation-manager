using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
namespace InzynierkaAPI.Controllers;

public static class UserEndpoints
{
    public static void MapUserEndpoints (this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/api/User", async (DataContext db) =>
        {
            return await db.User.ToListAsync();
        })
        .WithName("GetAllUsers");

        routes.MapGet("/api/User/{id}", async (Guid Id, DataContext db) =>
        {
            return await db.User.FindAsync(Id)
                is User model
                    ? Results.Ok(model)
                    : Results.NotFound();
        })
        .WithName("GetUserById");

        routes.MapPut("/api/User/{id}", async (Guid Id, User user, DataContext db) =>
        {
            var foundModel = await db.User.FindAsync(Id);

            if (foundModel is null)
            {
                return Results.NotFound();
            }
            //update model properties here

            await db.SaveChangesAsync();

            return Results.NoContent();
        })
        .WithName("UpdateUser");

        routes.MapPost("/api/User/", async (User user, DataContext db) =>
        {
            db.User.Add(user);
            await db.SaveChangesAsync();
            return Results.Created($"/Users/{user.Id}", user);
        })
        .WithName("CreateUser");

        routes.MapDelete("/api/User/{id}", async (Guid Id, DataContext db) =>
        {
            if (await db.User.FindAsync(Id) is User user)
            {
                db.User.Remove(user);
                await db.SaveChangesAsync();
                return Results.Ok(user);
            }

            return Results.NotFound();
        })
        .WithName("DeleteUser");

		routes.MapPost("/api/Powiadomienia/{id}", async (int id , DataContext db) =>
		{
			var czyPowiadomienia = await db.Ustawienia.FindAsync("Powiadomienia");
			if (czyPowiadomienia == null)
			{
				db.Ustawienia.Add(new Ustawienia
				{
					Id = "Powiadomienia",
					Wartosc = true
				});
			}
			czyPowiadomienia.Wartosc = id == 1;
            await db.SaveChangesAsync();
            return Results.Accepted();
		})
        .WithName("Powiadomienia");
	}
}
