namespace ContactManagement.FunctionalTests.Features.Support
{
    /// <summary>
    /// A class to store database information for tests.
    /// </summary>
    public class DatabaseInformation
    {
        /// <summary>
        /// Gets or sets the database name used for in-memory database.
        /// </summary>
        public string DatabaseName { get; set; } = "TestDb";
        
        /// <summary>
        /// Flag indicating whether the database has been seeded with test data.
        /// </summary>
        public bool IsSeeded { get; set; } = false;
    }
}