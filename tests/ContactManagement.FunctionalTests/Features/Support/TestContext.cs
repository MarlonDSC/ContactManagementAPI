using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ContactManagement.FunctionalTests.Features.Support
{
    /// <summary>
    /// Test context that manages the API test server and HTTP client.
    /// This class is designed to be used with Reqnroll for BDD testing.
    /// </summary>
    public class TestContext : IDisposable
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private bool _disposed;

        public HttpClient Client { get; private set; }
        public HttpResponseMessage? LastResponse { get; set; }
        public JsonSerializerOptions JsonOptions { get; }
        public IServiceProvider Services => _factory.Services;

        public TestContext()
        {
            // Initialize the custom WebApplicationFactory for testing
            _factory = new IntegrationTestWebAppFactory();
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
