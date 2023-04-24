using Azure.Storage.Blobs;
using InzynierkaAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace InzynierkaAPI.Services
{
    public class PlikiService : IPlikiService
    {
        public async Task<IResult> DownloadFile(Guid guid, DataContext db, BlobServiceClient blobServiceClient)
        {
            var plik = await db.Plik.FindAsync(guid);
            var downloadStream = (await blobServiceClient.GetBlobContainerClient("abc").GetBlobClient(plik.Url).DownloadStreamingAsync()).Value.Content;

            return Results.File(downloadStream, fileDownloadName: plik.Nazwa);
        }

        public async Task<IResult> UploadFile(HttpRequest request, DataContext db, BlobServiceClient blobServiceClient)
        {
            var uploadPlik = request.Form.Files.Single();
            var przetargId = int.Parse(request.Form["Id"].Single());
            var nazwaPliku = request.Form["nazwa"].Single();
            int kategoriaId = int.Parse(request.Form["kategoriaId"].Single());

            var foundModel = await db.Przetarg.FindAsync(przetargId);
            string plikId = Guid.NewGuid().ToString();
            await blobServiceClient.GetBlobContainerClient("abc").UploadBlobAsync(plikId, uploadPlik.OpenReadStream());

            var plik = new Plik { Url = plikId, Nazwa = nazwaPliku, KategoriaPlikId = kategoriaId };

            foundModel.Pliki ??= new List<Plik>();

            foundModel.Pliki.Add(plik);
            await db.SaveChangesAsync();
            return Results.StatusCode(200);
        }
        public  async Task DeleteBlobAsync(Plik plik, BlobServiceClient blobServiceClient, DataContext db = null)
        {
            await blobServiceClient.GetBlobContainerClient("abc").DeleteBlobAsync(plik.Url);
            if (db != null)
            {
                db.Plik.Remove(plik);
                await db.SaveChangesAsync();
            }
        }


    }
}
