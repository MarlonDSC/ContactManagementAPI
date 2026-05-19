using Testcontainers.MsSql;

namespace ContactManagement.FunctionalTests.Features.Support
{
    public static class TestDatabaseContainer
    {
        private static MsSqlContainer? _container;

        public static string ConnectionString =>
            _container?.GetConnectionString()
            ?? throw new InvalidOperationException("Test database container has not been started.");

        public static async Task StartAsync()
        {
            if (_container is not null)
            {
                return;
            }

            _container = new MsSqlBuilder().Build();
            await _container.StartAsync();
        }

        public static async Task StopAsync()
        {
            if (_container is null)
            {
                return;
            }

            await _container.DisposeAsync();
            _container = null;
        }
    }
}
