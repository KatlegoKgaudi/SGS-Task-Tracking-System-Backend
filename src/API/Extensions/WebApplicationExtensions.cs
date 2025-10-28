using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;

namespace SGS.TaskTracker.API.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            ConfigureDevelopmentPipeline(app);
            ConfigureProductionPipeline(app);
            MapEndpoints(app);

            return app;
        }

        private static void ConfigureDevelopmentPipeline(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); 

                app.UseSwaggerUI(options =>
                {
                    options.DocumentTitle = "SGS TaskTracker API - .NET Aspire";
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });
            }
        }

        private static void ConfigureProductionPipeline(WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseCors("AllowAngularApp");
            app.UseAuthentication();
            app.UseAuthorization();
        }

        private static void MapEndpoints(WebApplication app)
        {
            app.MapControllers();
            app.MapHealthCheck();
            app.MapAspireEndpoints();
        }

        public static WebApplication MapHealthCheck(this WebApplication app)
        {
            app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
                .WithName("HealthCheck")
                .WithSummary("Health check endpoint")
                .WithDescription("Returns the health status of the API")
                .WithOpenApi(); 

            return app;
        }

        public static WebApplication MapAspireEndpoints(this WebApplication app)
        {
            app.MapGet("/", () => "SGS TaskTracker API is running with .NET Aspire OpenAPI")
                .WithTags("Info")
                .WithOpenApi(operation =>
                {
                    operation.Summary = "API Root";
                    operation.Description = "Returns basic API information";
                    return operation;
                });

            return app;
        }
    }
}