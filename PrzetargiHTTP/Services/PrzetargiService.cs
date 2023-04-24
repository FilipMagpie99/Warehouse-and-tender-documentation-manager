using InzynierkaAPI.Models;
using PrzetargiHTTP.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Azure.Core;
using Microsoft.AspNetCore.Components.Forms;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace PrzetargiHTTP.Data
{
    public class PrzetargiService
    {

        public static HttpClient client { get; set; } = new HttpClient();

        public Task<AuthenticationState> authenticationState { get; set; }
        public bool isLogged { get; set; }

        public long maxFileSize { get; set; } = 1024 * 1024 * 1024;

        public static T? Deserializuj<T>(string tekst)
        {
            return JsonSerializer.Deserialize<T>(tekst, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<Przetarg[]> GetAllPrzetargi(int page)
        {
            string par = String.Format("?page={0}", page);
            var a = await client.GetAsync("https://localhost:7228/api/przetargi" + par);
            
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Przetarg[]>(b);
            }
            else
            {
                return new Przetarg[] { }; 
            }
        }          
        public async Task<WystawcaPrzetargu[]> GetAllWystawcy(int page)
        {
			string par = String.Format("?page={0}", page);
			var a = await client.GetAsync("https://localhost:7228/api/WystawcaPrzetargu"+par);
            
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<WystawcaPrzetargu[]>(b);
            }
            else
            {
                return new WystawcaPrzetargu[] { }; 
            }
        }          
        
        public async Task<WystawcaPrzetargu[]> GetAllWystawcyToList()
        {
			
			var a = await client.GetAsync("https://localhost:7228/api/WystawcaPrzetarguToList");
            
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<WystawcaPrzetargu[]>(b);
            }
            else
            {
                return new WystawcaPrzetargu[] { }; 
            }
        }         
        public async Task<Przetarg[]> GetPrzetargiForLastPage()
        {
            var a = await client.GetAsync("https://localhost:7228/api/przetargi/lastpage");
            
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Przetarg[]>(b);
            }
            else
            {
                return new Przetarg[] { }; 
            }
        }           
        public async Task<WystawcaPrzetargu[]> GetWystawcyForLastPage()
        {
            var a = await client.GetAsync("https://localhost:7228/api/WystawcyLastPage");
            
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<WystawcaPrzetargu[]>(b);
            }
            else
            {
                return new WystawcaPrzetargu[] { }; 
            }
        }
		public async Task<WystawcaPrzetargu[]> FindWystawca(int page, string nazwaWystawcy, string nip)
		{
			var message = string.Format("?page={0}&nazwaWystawcy={1}&nip={2}", page, nazwaWystawcy, nip);
			var a = await client.GetAsync("https://localhost:7228/api/FindWystawca" + message);
			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<WystawcaPrzetargu[]>(b);
			}
			else
			{
				return new WystawcaPrzetargu[] { };
			}
		}		
        public async Task<Dostawca[]> FindDostawca(int page, string nazwaWystawcy, string nip)
		{
			var message = string.Format("?page={0}&nazwaWystawcy={1}&nip={2}", page, nazwaWystawcy, nip);
			var a = await client.GetAsync("https://localhost:7228/api/FindDostawca" + message);
			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<Dostawca[]>(b);
			}
			else
			{
				return new Dostawca[] { };
			}
		}
		public async Task<KategoriaPlik[]> GetKategoriePliku()
        {
            var response = await client.GetAsync("https://localhost:7228/api/KategoriaPlik");
            
            if (response.IsSuccessStatusCode)
            {
                return Deserializuj<KategoriaPlik[]>(await response.Content.ReadAsStringAsync());
            }
            else
            {
                return new KategoriaPlik[] { }; 
            }
        }        
        public async Task<Przetarg[]> GetAllPrzetargiByWystawca(int id)
        {
            var a = await client.GetAsync(string.Format("https://localhost:7228/api/PrzetargWystawca?idwystawcy={0}", id));
            
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Przetarg[]>(b);
            }
            else
            {
                return new Przetarg[] { }; 
            }
        } 
             
        public async Task<Przetarg[]> FindPrzetargs(int page,int Rok, string Nazwa, int Miesiac,string status)
        {
            var message = string.Format("?page={0}&rok={1}&nazwa={2}&miesiac={3}&status={4}", page, Rok, Nazwa, Miesiac,status);
            var a = await client.GetAsync("https://localhost:7228/api/FindPrzetarg" + message);
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Przetarg[]>(b);
                
            }
            else
            {
                return new Przetarg[] { }; 
            }
        }     

        public async Task<bool> Login(UserLogin user)
        {

            isLogged = false;
            var b = await client.PostAsJsonAsync<UserLogin>("https://localhost:7228/login", user);

            var c = (await b.Content.ReadAsStringAsync()).Replace("\"","");
            if (b.IsSuccessStatusCode)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", c);

                isLogged = true;

                return true;
            }
            return false;

        }
        public IResult Logout()
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
            
            isLogged=false;

            return Results.SignOut();
        }
        public async Task<string> Register(User user)
        {
            var a = await client.PostAsJsonAsync<User>("https://localhost:7228/register", user);
            if (a.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return await a.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }
        public async Task<Przetarg> GetPrzetargById(int id)
        {
            var b = await client.GetAsync("https://localhost:7228/api/przetargi/" + id);
            var a = await (b).Content.ReadAsStringAsync();
            return Deserializuj<Przetarg>(a);
        }         
        
        public async Task ChangeNumberOfItems(int items)
        {
            var b = await client.GetAsync("https://localhost:7228/api/przetargi/ilosc/" + items);
        }             
        public async Task ChangeNumberOfItemsW(int items)
        {
            var b = await client.GetAsync("https://localhost:7228/api/ChangeNumberOfItemsW/" + items);
        }          

        public async Task<WystawcaPrzetargu> GetWystawcaById(int id)
        {
            var b = await client.GetAsync("https://localhost:7228/api/WystawcaPrzetargu/" + id);
            var a = await (b).Content.ReadAsStringAsync();
            return Deserializuj<WystawcaPrzetargu>(a);
        }        
        
        public async Task<Dostawca> GetDostawcaById(int id)
        {
            var b = await client.GetAsync("https://localhost:7228/api/Dostawca/" + id);
            var a = await (b).Content.ReadAsStringAsync();
            return Deserializuj<Dostawca>(a);
        }
                
        public async Task<Producent> GetProducentById(int id)
        {
            var b = await client.GetAsync("https://localhost:7228/api/Producent/" + id);
            var a = await (b).Content.ReadAsStringAsync();
            return Deserializuj<Producent>(a);
        }

        public async Task AddPrzetarg(Przetarg przetarg)
        {
            await client.PostAsJsonAsync("https://localhost:7228/api/przetargi", przetarg);
        }             
        
        public async Task<string> AddWystawcaPrzetargu(WystawcaPrzetargu wystawcaPrzetargu)
        {
            var a = await client.PostAsJsonAsync("https://localhost:7228/api/WystawcaPrzetargu", wystawcaPrzetargu);
            if (a.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return "Podana firma ju¿ istnieje!";
            }
            return string.Empty;
        }         
        
        public async Task ZmienStatusPrzetargu(Przetarg przetarg,int id, Status status)
        {
            przetarg.Status = status;
            await client.PutAsJsonAsync("https://localhost:7228/api/przetargi/status/" + id, przetarg);
        }            
        public async Task EditPrzetarg(int id, Przetarg przetarg)
        {
            await client.PutAsJsonAsync("https://localhost:7228/api/przetargi/" + id, przetarg);
        }        
        public async Task DeletePrzetarg(int id)
        {
            await client.DeleteAsync("https://localhost:7228/api/przetargi/" + id);
        }        
        public async Task<string> DeleteDostawca(int id)
        {
           var a = await client.DeleteAsync("https://localhost:7228/api/Dostawca/" + id);
            if (a.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return "Zabroniona operacja! W celu usuniêcia dostawcy, najpierw usuñ jego produkty.";
            }
            return string.Empty;    
        }

        public async Task<string> DodajPlik(string Id, string nazwa, string kategoriaId, IBrowserFile e)
        {
            try { 
            if (e != null)
            {
                //dodac ze plik jest za duzy
                var a = new StreamContent(e.OpenReadStream(maxAllowedSize: 51200000));
                    if (a.Headers.ContentType != null)
                        a.Headers.ContentType = new MediaTypeHeaderValue(e.ContentType);
                    else
                        a.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                var plik = new MultipartFormDataContent
                {
                    { a, "plik", "plik.bin" },
                    { new StringContent(nazwa), "nazwa" },
                    { new StringContent(Id), "Id" },
                    { new StringContent(kategoriaId), "kategoriaId" }
                };

                var response = await client.PostAsync("https://localhost:7228/api/Blob", plik);
                    if (response.IsSuccessStatusCode)
                        return "Uda³o siê, dodaæ plik.";
                System.Diagnostics.Debug.Print(await response.Content.ReadAsStringAsync());
                return string.Empty;
            }
            else
            {
                await Task.CompletedTask;
                return string.Empty;

            }
            }
            catch (IOException error)
            { 
                return "Wybrany plik przekracza maksymalny rozmiar 50MB." ;
            }
        }
        public async Task<Stream> GetPlikById(Guid id)
        {

            var b = await client.GetAsync("https://localhost:7228/api/Blob/" + id);
            var a  = (b).Content.ReadAsStream();
            return a;
        }

        public async Task<Object> GetFirmaFromKrs(string krs)
        {
            var b = await client.GetAsync("https://localhost:7228/api/WystawcaKrs/" + krs);
            var a = await b.Content.ReadAsStringAsync();
			if (b.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				return await b.Content.ReadAsStringAsync();
			}
			return Deserializuj<KrsApi>(a);
        }

        public async Task DeleteWystawca(int id)
        {
            await client.DeleteAsync("https://localhost:7228/api/WystawcaPrzetargu/" + id);
        }

        public async Task DeleteFile(Guid id)
        {
            await client.DeleteAsync("https://localhost:7228/api/DeleteBlob/" + id);
        }
    }
}