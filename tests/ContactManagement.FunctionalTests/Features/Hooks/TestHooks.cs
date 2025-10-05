using ContactManagement.FunctionalTests.Features.Support;

namespace ContactManagement.FunctionalTests.Features.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;
        private TestContext? _testContext;

        public TestHooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

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
            
            // This could include seeding the database with test data
            // or setting up mock services
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

