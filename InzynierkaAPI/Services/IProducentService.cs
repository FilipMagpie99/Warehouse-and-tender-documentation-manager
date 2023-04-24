using Azure.Storage.Blobs;
using InzynierkaAPI.Models;

namespace InzynierkaAPI.Services
{
    public interface IProducentService {
		Task<IResult> CreateProducent(Producent producent, DataContext db);
		Task<IResult> DeleteProducent(int id, DataContext db);
		Task<IResult> UpdateProducent(int id, Producent producent, DataContext db);


	}
}
