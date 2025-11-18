using CWDocMgrBlazor;
using CWDocMgrBlazor.Components;
using CWDocMgrBlazor.Components.Account;
using CWDocMgrBlazor.Data;
using CWDocMgrBlazor.Services;
using DocMgrLib.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with relative path
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<UserService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();
builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
Log.Information("ConnectionString: " + connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// Only add Swagger in Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<OCRService>();
builder.Services.AddScoped<PathService>();

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(CWDocMgrBlazor.Client._Imports).Assembly);

app.MapControllers();
app.MapAdditionalIdentityEndpoints();

await DbInitializer.Initialize(app.Services, builder.Configuration);

app.Run();
