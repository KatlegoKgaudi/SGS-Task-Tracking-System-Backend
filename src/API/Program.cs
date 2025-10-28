using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGS.TaskTracker.Application.Common_.Validators;
using SGS.TaskTracker.Application.Services;
using SGS.TaskTracker.Core.Data;
using SGS.TaskTracker.Core.Interfaces;
using SGS.TaskTracker.Infrastructure.Data.Repositories;
using SGS.TaskTracker.Interfaces;
using SGS.TaskTracker.Services;
using SSGTaskTracker.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add OpenAPI services
builder.Services.AddOpenApi(); // This replaces AddSwaggerGen and AddEndpointsApiExplorer

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SSGTaskTracker";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SSGTaskTrackerUsers";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Database Context
builder.Services.AddDbContext<TaskTrackerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repository Registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Service Registrations
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Token Service with configuration
builder.Services.AddScoped<ITokenService>(provider =>
    new TokenService(jwtSecret, jwtIssuer, jwtAudience));

// Background Service
builder.Services.AddHostedService<TaskStatusBackgroundService>();

// Fluent Validation
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

// Add HttpContextAccessor for accessing current user
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Configure OpenAPI document
    app.MapOpenApi(); // This replaces app.UseSwagger()

    // Configure OpenAPI UI - this is the replacement for Swagger UI
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "SSG TaskTracker API Documentation";
        });
    }
}

app.UseHttpsRedirection();

// Use CORS - must be before UseAuthentication and UseAuthorization
app.UseCors("AllowAngularApp");

// Use Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add a health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithSummary("Health check endpoint")
    .WithDescription("Returns the health status of the API");

app.Run();