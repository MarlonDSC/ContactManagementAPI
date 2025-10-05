using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ContactManagement.API;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Reqnroll;

namespace ContactManagement.FunctionalTests.Features.Support
{
    /// <summary>
    /// Test context that manages the API test server and HTTP client.
    /// This class is designed to be used with Reqnroll for BDD testing.
    /// </summary>
    public class TestContext : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private bool _disposed;
        
        public HttpClient Client { get; private set; }
        public HttpResponseMessage? LastResponse { get; set; }
        public JsonSerializerOptions JsonOptions { get; }
        
        public TestContext()
        {
            // Initialize the WebApplicationFactory with custom configuration for testing
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Configure services for testing
                        // You can customize services for testing here, such as:
                        // - Replacing real database with in-memory database
                        // - Mocking external services
                        // - Adding test-specific services
                        
                        // Configure logging for tests
                        services.AddLogging(loggingBuilder =>
                        {
                            loggingBuilder.AddConsole();
                            loggingBuilder.SetMinimumLevel(LogLevel.Warning);
                        });
                    });
                });
                
            Client = _factory.CreateClient();
            
            // Configure JSON options to match the API
            JsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }
        
        public async Task<HttpResponseMessage> SendJsonRequest<T>(HttpMethod method, string uri, T content)
        {
            var request = new HttpRequestMessage(method, uri);
            
            if (content is not null)
            {
                var json = JsonSerializer.Serialize(content, JsonOptions);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }
            
            LastResponse = await Client.SendAsync(request);
            return LastResponse;
        }
        
        public async Task<T?> ReadResponseAs<T>() where T : class
        {
            if (LastResponse == null)
                return null;
                
            return await LastResponse.Content.ReadFromJsonAsync<T>(JsonOptions);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
                
            if (disposing)
            {
                Client?.Dispose();
                _factory?.Dispose();
            }
            
            _disposed = true;
        }
    }
}
