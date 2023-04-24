using InzynierkaAPI.Data;
using InzynierkaAPI.Models;
using InzynierkaAPI.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace InzynierkaAPITests
{

	public class ProducentControllerTests
	{

		public ProducentControllerTests()
		{
			[Fact]
			 async Task GetProduktById_ReturnsProdukt_WhenIdExists()
			{
				// Arrange
				var mockDb = new Mock<DataContext>();
				var mockProdukt = new Produkt { Id = 1, Nazwa = "Test Produkt", JednostkaMiary="testjednostka", TypProduktu="testtyp",ProducentId=1};
				var mockDbSet = new Mock<DbSet<Produkt>>();
				mockDbSet.As<IAsyncEnumerable<Produkt>>().Setup(x => x.GetEnumerator()).Returns(new AsyncEnumerator<Produkt>(new List<Produkt> { mockProdukt }.GetEnumerator()));
				mockDbSet.As<IQueryable<Produkt>>().Setup(x => x.Provider).Returns(new AsyncQueryProvider<Produkt>(new List<Produkt> { mockProdukt }.AsQueryable().Provider));
				mockDbSet.As<IQueryable<Produkt>>().Setup(x => x.Expression).Returns(new List<Produkt> { mockProdukt }.AsQueryable().Expression);
				mockDbSet.As<IQueryable<Produkt>>().Setup(x => x.ElementType).Returns(new List<Produkt> { mockProdukt }.AsQueryable().ElementType);
				mockDbSet.Setup(x => x.FindAsync(1)).ReturnsAsync(mockProdukt);

				var mockDb = new Mock<DataContext>();
				mockDb.Setup(x => x.Set<Produkt>()).Returns(mockDbSet.Object);

				var server = new TestServer(new WebHostBuilder()
					.UseStartup<IStartup>()
					.ConfigureServices(services =>
					{
						services.AddScoped(x => mockDb.Object);
					}));
				var client = server.CreateClient();
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test_token");

				// Act
				var response = await client.GetAsync("/api/Produkt/1");
				var responseText = await response.Content.ReadAsStringAsync();
				var produktResult = System.Text.Json.JsonSerializer.Deserialize<Produkt>(responseText);

				// Assert
				Assert.Equal(HttpStatusCode.OK, response.StatusCode);
				Assert.Equal(1, produktResult.Id);
				Assert.Equal("Test Produkt", produktResult.Nazwa);
			}


		}


	}

}