using Microsoft.EntityFrameworkCore;
using ContactManagement.Infrastructure.Data;

namespace ContactManagement.API
{
    public class Program
    {
        protected Program() { }

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Apply migrations at startup
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var env = services.GetRequiredService<IWebHostEnvironment>();
                var logger = services.GetRequiredService<ILogger<Program>>();
                
                logger.LogInformation("Running in environment: {Environment}", env.EnvironmentName);
                
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    logger.LogInformation("Applying database migrations...");
                    context.Database.Migrate();
                    logger.LogInformation("Database migrated successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) => 
                {
                    var env = hostingContext.HostingEnvironment;
                    
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    
                    if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
                    {
                        config.AddJsonFile("appsettings.Docker.json", optional: true, reloadOnChange: true);
                    }
                    
                    config.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}


