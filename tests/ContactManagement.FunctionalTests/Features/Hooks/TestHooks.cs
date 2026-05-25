using ContactManagement.FunctionalTests.Features.Support;
using ContactManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManagement.FunctionalTests.Features.Hooks
{
    [Binding]
    public class TestHooks(ScenarioContext scenarioContext)
    {
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private TestContext? _testContext;

        [BeforeTestRun]
        public static async Task StartDatabaseContainer()
        {
            await TestDatabaseContainer.StartAsync();

            // Apply migrations once against the shared container.
            using var bootstrapFactory = new IntegrationTestWebAppFactory();
            using var scope = bootstrapFactory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        [AfterTestRun]
        public static async Task StopDatabaseContainer()
        {
            await TestDatabaseContainer.StopAsync();
        }

        [BeforeScenario(Order = 1)]
        public void InitializeTestContext()
        {
            _testContext = new TestContext();
            _scenarioContext.Set(_testContext);
        }

        [BeforeScenario(Order = 2)]
        public async Task SetupTestData()
        {
            if (_testContext is null)
            {
                return;
            }

            using var scope = _testContext.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await CleanDatabaseAsync(dbContext);
        }

        private static async Task CleanDatabaseAsync(ApplicationDbContext dbContext)
        {
            // Order matters because of FK relationships.
            await dbContext.FundContacts.ExecuteDeleteAsync();
            await dbContext.Funds.ExecuteDeleteAsync();
            await dbContext.Contacts.ExecuteDeleteAsync();
        }

        [AfterScenario]
        public void CleanupScenario()
        {
            _testContext?.Dispose();
        }
    }
}
