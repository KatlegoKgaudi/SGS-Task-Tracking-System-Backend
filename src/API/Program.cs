using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.OpenApi;
using SGS.TaskTracker.API.Extensions;
using SGS.TaskTracker.Core.Data;
using SGS.TaskTracker.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

//for debugging
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Using Connection String: {connectionString}");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddBackgroundServices();
builder.Services.AddValidation();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("SGS.TaskTracker");

var app = builder.Build();

app.MapGet("/db-health", async (IServiceProvider services) =>
{
    try
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TaskTrackerContext>();
        var canConnect = await context.Database.CanConnectAsync();
        return Results.Ok(new
        {
            status = "Healthy",
            database = "Connected",
            server = "sgs-tasktracker-db"
        });
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

app.ConfigurePipeline();

app.Run();