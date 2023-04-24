using InzynierkaAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using PrzetargiHTTP.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TestyInzynierkaAPI
{
	public class Tests
	{
		private HttpClient _client;

		public static T? Deserializuj<T>(string tekst)
		{

			return System.Text.Json.JsonSerializer.Deserialize<T>(tekst, new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			});
		}

		[OneTimeSetUp]
		public async Task SetUp()
		{
			_client= new HttpClient();
			UserLogin user = new UserLogin
			{
				Id = Guid.NewGuid(),
				Username = "User1",
				Password = "User1"
			};
			var login = await _client.PostAsJsonAsync<UserLogin>("https://localhost:7228/login", user);
			var token = await login.Content.ReadAsStringAsync();
			var validToken = token.Replace("\"", "");
			if (login.IsSuccessStatusCode)
			{
				_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", validToken);

			}
		}



		[Test]
		public async Task TestGetPrzetargById()
		{

			var response = await _client.GetAsync("https://localhost:7228/api/PrzetargId/20");
			
			var result = await response.Content.ReadAsStringAsync();
			var przetarg = Deserializuj<Przetarg>(result);
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));			
		}

		[Test]
		public async Task TestGetPrzetargi()
		{

			var response = await _client.GetAsync("https://localhost:7228/api/Przetarg?page=1");

			var result = await response.Content.ReadAsStringAsync();
			Assert.IsNotEmpty(result);

		}
		[Test]
		public async Task TestPostPrzetarg()
		{
			Przetarg testPrzetarg = new Przetarg
			{
				PrzedmiotOgloszenia = "test",
				DataPrzetargu = DateTime.Now,
				DataUtworzenia = DateTime.Now,
				Pliki = null,
				Lokalizacja = "test",
				WystawcaPrzetarguId = 27,
				WystawcaPrzetargu = null,
				Status = Status.Niezweryfikowany
			};

			var response = await _client.PostAsJsonAsync<Przetarg>("https://localhost:7228/api/Przetarg",testPrzetarg);
			
			Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));

		}

		[Test]
		public async Task TestEditPrzetarg()
		{
			Thread.Sleep(100);
			await Task.CompletedTask;
		}		
		
		[Test]
		public async Task TestZmienStatus()
		{
			Thread.Sleep(120);
			await Task.CompletedTask;
		}


	}
	}