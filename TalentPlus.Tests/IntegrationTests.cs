using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TalentPlus.Infraestructure.Persistence;

namespace TalentPlus.Tests;

// I use WebApplicationFactory to spin up a test server in memory
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

                    
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });
                });
            });
        }

        // INTEGRATION TEST: Public endpoint returns 200 OK
        [Fact]
        public async Task GetDepartments_PublicEndpoint_ReturnsSuccess()
        {
            var client = _factory.CreateClient();

            
            var response = await client.GetAsync("/api/Public/departments");

          
            response.EnsureSuccessStatusCode(); 
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        //  INTEGRATION TEST: Protected endpoint returns 401 without Token
        [Fact]
        public async Task GetEmployees_WithoutToken_ReturnsUnauthorized()
        {
            
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/Employees");
            
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }