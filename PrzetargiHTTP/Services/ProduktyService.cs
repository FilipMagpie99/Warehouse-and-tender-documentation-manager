using InzynierkaAPI.Models;
using PrzetargiHTTP.Data;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Azure.Core;
using Microsoft.AspNetCore.Components.Forms;
using InzynierkaAPI.Migrations;

namespace PrzetargiHTTP.Data
{
    public class ProduktyService
    {

		public static T? Deserializuj<T>(string tekst)
		{ 
            
            return JsonSerializer.Deserialize<T>(tekst, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<Produkt[]> GetAllProdukty(int page)

        {
			string par = String.Format("?page={0}", page);
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/Produkt"+par);
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Produkt[]>(b);
            }
            else
            {
                return new Produkt[] { };
            }
        }
		public async Task<Produkt[]> GetProduktyForLastPage()
		{
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/ProduktyLastPage");

			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<Produkt[]>(b);
			}
			else
			{
				return new Produkt[] { };
			}
		}
		public async Task ChangeNumberOfItemsD(int items)
		{
			var b = await PrzetargiService.client.GetAsync("https://localhost:7228/api/ChangeNumberOfItemsD/" + items);
		}		
        public async Task ChangeNumberOfItemsPro(int items)
		{
			var b = await PrzetargiService.client.GetAsync("https://localhost:7228/api/ChangeNumberOfItemsPro/" + items);
		}		
        public async Task ChangeNumberOfItemsP(int items)
		{
			var b = await PrzetargiService.client.GetAsync("https://localhost:7228/api/ChangeNumberOfItemsP/" + items);
		}
		public async Task<Produkt[]> GetProduktyByStrefa(int page,string idStrefy)

        {
			string par = String.Format("?page={0}&idStrefy={1}", page,idStrefy);
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/ProduktyByStrefa" + par);
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Produkt[]>(b);
            }
            else
            {
                return new Produkt[] { };
            }
        }

		public async Task<Produkt[]> FindProdukt(int page, string nazwaProduktu, string typProduktu, string producent)
		{
			var message = string.Format("?page={0}&nazwaProduktu={1}&typProduktu={2}&producent={3}", page, nazwaProduktu, typProduktu, producent);
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/FindProdukt" + message);
			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<Produkt[]>(b);
			}
			else
			{
				return new Produkt[] { };
			}
		}
		public async Task<string> AddDostawca(Dostawca dostawca)
        {
           var a = await PrzetargiService.client.PostAsJsonAsync("https://localhost:7228/api/Dostawca", dostawca);
           if(a.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return "Podana firma już istnieje!";
            } 
           return string.Empty;
        } 		
        public async Task AddKategoria(KategoriaPlik kategoriaPlik)
        {
            await PrzetargiService.client.PostAsJsonAsync("https://localhost:7228/api/KategoriaPlik", kategoriaPlik);
        }    		
        public async Task EditMagazyn(Magazyn magazyn)
        {
            await PrzetargiService.client.PutAsJsonAsync("https://localhost:7228/api/Magazyn/" + magazyn.Id, magazyn);
        }        
        public async Task<string> AddProducent(Producent producent)
        {
            var a = await PrzetargiService.client.PostAsJsonAsync("https://localhost:7228/api/Producent", producent);
            if (a.StatusCode == System.Net.HttpStatusCode.BadRequest)
			{
				return "Podana firma już istnieje!";
			}
			return string.Empty;
		}

        public async Task<Producent[]> GetAllProducenci(int page)

        {
			string par = String.Format("?page={0}", page);
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/Producent"+par);
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Producent[]>(b);
            }
            else
            {
                return new Producent[] { };
            }
        }         
        
        public async Task<Producent[]> GetAllProducenciToList()

        {
			
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/ProducentToList");
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Producent[]>(b);
            }
            else
            {
                return new Producent[] { };
            }
        }         
        public async Task<Magazyn[]> GetAllMagazyny()

        {
           
            var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/Magazyn");
            if (a.IsSuccessStatusCode)
            {
                var b = await a.Content.ReadAsStringAsync();
                return Deserializuj<Magazyn[]>(b);
            }
            else
            {
                return new Magazyn[] { };
            }
        }        
        
        public async Task<Dostawca[]> GetAllDostawcy(int page)

        {
			string par = String.Format("?page={0}", page);
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/Dostawca"+par);
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
        public async Task<Dostawca[]> GetAllDostawcyToList()

        {
	
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/DostawcaToList");
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

        public async Task<Produkt> GetProduktById(int id)
        {
            var b = await PrzetargiService.client.GetAsync("https://localhost:7228/api/Produkt/" + id);
            if(b.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            var a = await (b).Content.ReadAsStringAsync();
            return Deserializuj<Produkt>(a);
        }              
        
        public async Task<RaportMagazynu[]> GetRaportMagazynuByProdukt(int idproduktu,int page)
        {
            string par = String.Format("?idproduktu={0}&page={1}", idproduktu, page);
            var b = await PrzetargiService.client.GetAsync("https://localhost:7228/api/RaportMagazynu/"+par);
            var a = await (b).Content.ReadAsStringAsync();
            if (b.IsSuccessStatusCode)
                return Deserializuj<RaportMagazynu[]>(a);
            else
                return new RaportMagazynu[] { };
        }        
        
        public async Task<string> ChangeProdukt(RaportMagazynu raportMagazynu)
        {
            
            var a = await PrzetargiService.client.PutAsJsonAsync("https://localhost:7228/api/ChangeProdukt", raportMagazynu);
            if (a.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                return await a.Content.ReadAsStringAsync();
            }
            return string.Empty;
        }

		public async Task<Dostawca[]> GetDostawcyForLastPage()
		{
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/DostawcyLastPage");

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

		public async Task AddProdukt(Produkt produkt)
        {
            await PrzetargiService.client.PostAsJsonAsync("https://localhost:7228/api/Produkt", produkt);
        }            
        public async Task AddMagazyn(Magazyn magazyn )
        {
            await PrzetargiService.client.PostAsJsonAsync("https://localhost:7228/api/Magazyn", magazyn);
        }        
        
        public async Task AddRaportMagazynu(RaportMagazynu raportMagazynu)
        {
            await PrzetargiService.client.PostAsJsonAsync("https://localhost:7228/api/RaportMagazynu", raportMagazynu);
        }

        public async Task<Stream> GetProduktQrById(int id)
        {

            var b = await PrzetargiService.client.GetAsync("https://localhost:7228/api/QR/" + id);
            var a = (b).Content.ReadAsStream();
            return a;
        }
        public async Task DeleteProdukt(int id)
        {
            await PrzetargiService.client.DeleteAsync("https://localhost:7228/api/Produkt/" + id);
        }        
        public async Task DeleteDostawca(int id)
        {
            await PrzetargiService.client.DeleteAsync("https://localhost:7228/api/Dostawca/" + id);
        }         
        
        public async Task DeleteMagazyn(int id)
        {
            await PrzetargiService.client.DeleteAsync("https://localhost:7228/api/Magazyn/" + id);
        }        
        public async Task DeleteProducent(int id)
        {
            await PrzetargiService.client.DeleteAsync("https://localhost:7228/api/Producent/" + id);
        }

		public async Task<Produkt[]> GetAllProduktyByMagazyn(int id)
		{
			var a = await PrzetargiService.client.GetAsync(string.Format("https://localhost:7228/api/MagazynProdukt?idmagazynu={0}", id));

			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<Produkt[]>(b);
			}
			else
			{
				return new Produkt[] { };
			}
		}

		public async Task<Magazyn> GetMagazynById(int id)
		{
			var b = await PrzetargiService.client.GetAsync("https://localhost:7228/api/Magazyn/" + id);
			var a = await (b).Content.ReadAsStringAsync();
            var c = Deserializuj<Magazyn>(a);

            return c;
		}
		public async Task<Producent[]> FindProducent(int page, string nazwaWystawcy, string nip)
		{
			var message = string.Format("?page={0}&nazwaWystawcy={1}&nip={2}", page, nazwaWystawcy, nip);
			var a = await PrzetargiService.client.GetAsync("https://localhost:7228/api/FindProducent" + message);
			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<Producent[]>(b);
			}
			else
			{
				return new Producent[] { };
			}
		}
		public async Task<Produkt[]> GetAllProduktyByDostawca(int id)
		{
			var a = await PrzetargiService.client.GetAsync(string.Format("https://localhost:7228/api/ProduktDostawca?iddostawcy={0}", id));

			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<Produkt[]>(b);
			}
			else
			{
				return new Produkt[] { };
			}
		}		
        public async Task<Produkt[]> GetAllProduktyByProducent(int id) { 
			var a = await PrzetargiService.client.GetAsync(string.Format("https://localhost:7228/api/ProduktProducent?idproducenta={0}", id));

			if (a.IsSuccessStatusCode)
			{
				var b = await a.Content.ReadAsStringAsync();
				return Deserializuj<Produkt[]>(b);
			}
			else
			{
				return new Produkt[] { };
			}
		}

	}
}
