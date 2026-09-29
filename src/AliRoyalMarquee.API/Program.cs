using AliRoyalMarquee.Application;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Infrastructure;
using AliRoyalMarquee.Infrastructure.Persistence;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using AliRoyalMarquee.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
// Configure CORS
var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"] ?? "https://ali-royal-marquee.vercel.app";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins.Split(','))
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // needed if sending cookies/tokens
    });
});

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger - Removed due to .NET 10 compatibility issues with Swashbuckle 6.6.2

// Infrastructure - Database
var envDbString = builder.Configuration["DB_CONNECTION_STRING"] ?? builder.Configuration["DATABASE_URL"];
var connectionString = !string.IsNullOrEmpty(envDbString) 
    ? envDbString 
    : builder.Configuration.GetConnectionString("DefaultConnection");
    
if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("localhost:5434"))
{
    if (builder.Environment.IsProduction()) 
    {
        throw new InvalidOperationException("No production database connection string found! Please set DB_CONNECTION_STRING in Railway.");
    }
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<AliRoyalMarquee.Application.Common.Interfaces.ITokenService, AliRoyalMarquee.Infrastructure.Services.TokenService>();
builder.Services.AddScoped<AliRoyalMarquee.Application.Enquiries.Interfaces.IQuotationPdfGenerator, AliRoyalMarquee.Infrastructure.Pdf.QuotationPdfGenerator>();
builder.Services.AddScoped<AliRoyalMarquee.Application.Enquiries.Interfaces.IEnquiriesReportPdfGenerator, AliRoyalMarquee.Infrastructure.Pdf.EnquiriesReportPdfGenerator>();

// Auth Configuration
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    // Policies
    options.AddPolicy("Enquiries.Export", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "Permission" && c.Value == "Enquiries.Export") ||
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("Owner"));
    });

    options.AddPolicy("Enquiries.ManageQuotations", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "Permission" && c.Value == "Enquiries.ManageQuotations") ||
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("Owner"));
    });
});

// Application
builder.Services.AddApplication();

// Add ProblemDetails for global error handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<AliRoyalMarquee.API.ExceptionHandlers.GlobalExceptionHandler>();

builder.Services.AddSignalR();

// Add Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "AliRoyalMarquee_";
});

// Add Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(o => o.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();

var app = builder.Build();

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Run Seeder
using (var scope = app.Services.CreateScope())
{
    await AppDbSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

// Use CORS before Auth
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<AliRoyalMarquee.API.Hubs.NotificationHub>("/hubs/notifications");

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter() }
});

// Ensure app binds to the PORT provided by Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
