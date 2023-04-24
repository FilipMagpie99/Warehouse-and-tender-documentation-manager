using Azure.Storage.Blobs;
using InzynierkaAPI.Models;

namespace InzynierkaAPI.Services
{
    public interface IPlikiService { 
        public  Task<IResult> UploadFile(HttpRequest request, DataContext db, BlobServiceClient blobServiceClient);
        public  Task<IResult> DownloadFile(Guid guid, DataContext db, BlobServiceClient blobServiceClient);
        public Task DeleteBlobAsync(Plik plik, BlobServiceClient blobServiceClient, DataContext db = null);

    }
}
