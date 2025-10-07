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
                // For in-memory database, the most reliable approach is to recreate the database
                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                
                Console.WriteLine("Database recreated for clean test environment");
                
                // Double-check that all tables are empty
                var fundContactCount = dbContext.FundContacts.Count();
                var fundCount = dbContext.Funds.Count();
                var contactCount = dbContext.Contacts.Count();
                
                if (fundContactCount > 0 || fundCount > 0 || contactCount > 0)
                {
                    Console.WriteLine($"Warning: After cleanup, database still contains: {fundContactCount} fund contacts, {fundCount} funds, {contactCount} contacts");
                    
                    // As a fallback, try explicit removal
                    try
                    {
                        // Remove all fund contacts first (due to foreign keys)
                        foreach (var entity in dbContext.FundContacts.ToList())
                        {
                            dbContext.FundContacts.Remove(entity);
                        }
                        dbContext.SaveChanges();
                        
                        // Then remove all funds
                        foreach (var entity in dbContext.Funds.ToList())
                        {
                            dbContext.Funds.Remove(entity);
                        }
                        dbContext.SaveChanges();
                        
                        // Finally remove all contacts
                        foreach (var entity in dbContext.Contacts.ToList())
                        {
                            dbContext.Contacts.Remove(entity);
                        }
                        dbContext.SaveChanges();
                        
                        Console.WriteLine("Explicit entity removal completed");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during explicit entity removal: {ex.Message}");
                    }
                }
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

