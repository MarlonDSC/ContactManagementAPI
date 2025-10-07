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

        [BeforeScenario(Order = 1)]
        public void InitializeTestContext()
        {
            // Create and register the TestContext
            _testContext = new TestContext();
            _scenarioContext.Set(_testContext);

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
                
                // Clean the database before each test to ensure a clean state
                CleanDatabase(dbContext);
                
                // Seeding can be added here if needed
                
                Console.WriteLine("Database prepared for test");
            }
        }
        
        private static void CleanDatabase(ApplicationDbContext dbContext)
        {
            try
            {
                // Clear the database using a more reliable approach for in-memory database
                // Get entities in memory first, then remove them one by one with exception handling
                
                // Remove all fund contacts
                var fundContacts = dbContext.FundContacts.ToList();
                foreach (var fundContact in fundContacts)
                {
                    try
                    {
                        dbContext.FundContacts.Remove(fundContact);
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex) when (ex is DbUpdateConcurrencyException || ex is InvalidOperationException)
                    {
                        // Entity might have been removed already, continue with next
                        Console.WriteLine($"Skipping already removed fund contact: {ex.Message}");
                    }
                }
                
                // Remove all funds
                var funds = dbContext.Funds.ToList();
                foreach (var fund in funds)
                {
                    try
                    {
                        dbContext.Funds.Remove(fund);
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex) when (ex is DbUpdateConcurrencyException || ex is InvalidOperationException)
                    {
                        // Entity might have been removed already, continue with next
                        Console.WriteLine($"Skipping already removed fund: {ex.Message}");
                    }
                }
                
                // Remove all contacts
                var contacts = dbContext.Contacts.ToList();
                foreach (var contact in contacts)
                {
                    try
                    {
                        dbContext.Contacts.Remove(contact);
                        dbContext.SaveChanges();
                    }
                    catch (Exception ex) when (ex is DbUpdateConcurrencyException || ex is InvalidOperationException)
                    {
                        // Entity might have been removed already, continue with next
                        Console.WriteLine($"Skipping already removed contact: {ex.Message}");
                    }
                }
                
                Console.WriteLine("Database cleaned for test");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during database cleanup: {ex.Message}");
                // Don't throw the exception as we want tests to continue even if cleanup has issues
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
        
        [BeforeFeature("Fund Management")]
        public static void BeforeFundManagementFeature(FeatureContext featureContext)
        {
            // Setup specific to the FundManagement feature
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

