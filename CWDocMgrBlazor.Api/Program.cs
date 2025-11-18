using CWDocMgrBlazor.Api.Data;
using CWDocMgrBlazor.Api.Models;
using CWDocMgrBlazor.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Force Production environment if not explicitly set
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")))
{
    builder.Environment.EnvironmentName = "Production";
}

// Configure Serilog - use relative path for production
string logFolder = builder.Configuration["LogFolder"] ?? "Logs";

if (!Path.IsPathRooted(logFolder))
{
    logFolder = Path.Combine(builder.Environment.ContentRootPath, logFolder);
}

Directory.CreateDirectory(logFolder);
string logFileName = Path.Combine(logFolder, "log-.txt");

// Set log level based on environment
var logLevel = builder.Environment.IsDevelopment()
    ? Serilog.Events.LogEventLevel.Debug
    : Serilog.Events.LogEventLevel.Warning;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(logLevel)
    .WriteTo.File(logFileName, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Register services
builder.Services.AddScoped<PathService>();

builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration["AllowedOrigins"] ?? "*";
        if (allowedOrigins == "*")
        {
            policy.AllowAnyOrigin();
        }
        else
        {
            policy.WithOrigins(allowedOrigins.Split(','));
        }
        policy.AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Only add OpenAPI in Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
}

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
