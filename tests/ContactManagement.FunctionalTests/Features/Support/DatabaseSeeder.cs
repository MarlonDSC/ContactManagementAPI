using ContactManagement.Domain.Entities;
using ContactManagement.Domain.ValueObjects;
using ContactManagement.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManagement.FunctionalTests.Features.Support
{
    /// <summary>
    /// Helper class for seeding test data into the in-memory database
    /// </summary>
    // public static class DatabaseSeeder
    // {
    //     /// <summary>
    //     /// Seeds the database with test contacts
    //     /// </summary>
    //     /// <param name="serviceProvider">The service provider to get the DbContext from</param>
    //     public static void SeedContacts(IServiceProvider serviceProvider)
    //     {
    //         using var scope = serviceProvider.CreateScope();
    //         var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
    //         // Clear existing data
    //         dbContext.Contacts.RemoveRange(dbContext.Contacts);
    //         dbContext.SaveChanges();
            
    //         // Add test contacts
    //         var contacts = new List<Contact>
    //         {
    //             new Contact(
    //                 "John", "Doe",
    //                 "john.doe@example.com",
    //                 "+1234567890"
    //             ),
    //             new Contact(
    //                 "Jane", "Smith",
    //                 "jane.smith@example.com",
    //                 "+0987654321"
    //             ),
    //             new Contact(
    //                 "Alice", "Johnson",
    //                 "alice.johnson@example.com",
    //                 "+1122334455"
    //             )
    //         };
            
    //         dbContext.Contacts.AddRange(contacts);
    //         dbContext.SaveChanges();
    //     }
        
    //     /// <summary>
    //     /// Seeds the database with test funds
    //     /// </summary>
    //     /// <param name="serviceProvider">The service provider to get the DbContext from</param>
    //     public static void SeedFunds(IServiceProvider serviceProvider)
    //     {
    //         using var scope = serviceProvider.CreateScope();
    //         var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
    //         // Clear existing data
    //         dbContext.Contacts.RemoveRange(dbContext.Contacts);
    //         dbContext.SaveChanges();
            
    //         // Add test funds
    //         var funds = new List<Fund>
    //         {
    //             new Fund { Name = "Growth Fund", Description = "A high-growth investment fund" },
    //             new Fund { Name = "Income Fund", Description = "A stable income investment fund" },
    //             new Fund { Name = "Balanced Fund", Description = "A balanced investment strategy" }
    //         };
            
    //         dbContext.Funds.AddRange(funds);
    //         dbContext.SaveChanges();
    //     }
        
    //     /// <summary>
    //     /// Seeds the database with test fund contacts
    //     /// </summary>
    //     /// <param name="serviceProvider">The service provider to get the DbContext from</param>
    //     public static void SeedFundContacts(IServiceProvider serviceProvider)
    //     {
    //         using var scope = serviceProvider.CreateScope();
    //         var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
    //         // Get existing contacts and funds
    //         var contacts = dbContext.Contacts.ToList();
    //         var funds = dbContext.Funds.ToList();
            
    //         if (!contacts.Any() || !funds.Any())
    //         {
    //             return; // Nothing to associate
    //         }
            
    //         // Clear existing data
    //         dbContext.FundContacts.RemoveRange(dbContext.FundContacts);
    //         dbContext.SaveChanges();
            
    //         // Create some fund contact associations
    //         var fundContacts = new List<FundContact>
    //         {
    //             new FundContact { ContactId = contacts[0].Id, FundId = funds[0].Id },
    //             new FundContact { ContactId = contacts[1].Id, FundId = funds[0].Id },
    //             new FundContact { ContactId = contacts[0].Id, FundId = funds[1].Id }
    //         };
            
    //         dbContext.FundContacts.AddRange(fundContacts);
    //         dbContext.SaveChanges();
    //     }
        
    //     /// <summary>
    //     /// Seeds all test data into the database
    //     /// </summary>
    //     /// <param name="serviceProvider">The service provider to get the DbContext from</param>
    //     public static void SeedAllTestData(IServiceProvider serviceProvider)
    //     {
    //         SeedContacts(serviceProvider);
    //         SeedFunds(serviceProvider);
    //         SeedFundContacts(serviceProvider);
    //     }
    // }
}
