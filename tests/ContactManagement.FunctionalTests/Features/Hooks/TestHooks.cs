using ContactManagement.FunctionalTests.Features.Support;
using ContactManagement.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManagement.FunctionalTests.Features.Hooks
{
    [Binding]
    public class TestHooks(ScenarioContext scenarioContext)
    {
        private readonly ScenarioContext _scenarioContext = scenarioContext;
        private TestContext? _testContext;

        [BeforeScenario(Order = 1)]
        public void InitializeTestContext()
        {
            // Create and register the TestContext
            _testContext = new TestContext();
            _scenarioContext.Set(_testContext);

            // No need to await since InitializeAsync is now static and returns a completed task
            Console.WriteLine("TestContext initialized for scenario");
        }
        
        [BeforeScenario(Order = 2)]
        public void SetupTestData()
        {
            // Setup test data needed for all scenarios
            Console.WriteLine("Setting up test data");
            
            // Seed the in-memory database with test data
            if (_testContext != null)
            {
                using var scope = _testContext.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Ensure database is created
                dbContext.Database.EnsureCreated();
                
                // Use the DatabaseSeeder to seed test data if needed
                // DatabaseSeeder.SeedAllTestData(scope.ServiceProvider);
                
                Console.WriteLine("Database seeded with test data");
            }
        }

        [AfterScenario]
        public void CleanupScenario()
        {
            // Dispose the TestContext
            _testContext?.Dispose();
            
            Console.WriteLine("Cleaned up scenario resources");
        }

        [BeforeFeature("ContactManagement")]
        public static void BeforeContactManagementFeature(FeatureContext featureContext)
        {
            // Setup specific to the ContactManagement feature
            Console.WriteLine($"Setting up for feature: {featureContext.FeatureInfo.Title}");
        }
        
        [BeforeFeature("FundContactAssignment")]
        public static void BeforeFundContactAssignmentFeature(FeatureContext featureContext)
        {
            // Setup specific to the FundContactAssignment feature
            Console.WriteLine($"Setting up for feature: {featureContext.FeatureInfo.Title}");
        }

        [AfterFeature]
        public static void AfterFeature(FeatureContext featureContext)
        {
            // Cleanup code that runs after each feature
            Console.WriteLine($"Completed feature: {featureContext.FeatureInfo.Title}");
        }
    }
}

