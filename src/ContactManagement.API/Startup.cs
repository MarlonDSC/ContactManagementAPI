using ContactManagement.API.Exceptions;
using ContactManagement.Application.Extensions;
using ContactManagement.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ContactManagement.API
{
    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Add Swagger services
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Contact Management API",
                    Version = "v1",
                    Description = "API for managing contacts"
                });
            });

            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);

            // Add authorization services
            services.AddAuthentication();
            services.AddAuthorization();

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService("ContactManagement"))
                .WithTracing(tracing =>
                {
                    tracing
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation();
                })
                .UseOtlpExporter();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contact Management API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}