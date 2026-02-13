using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using OM_Lab;
using OM_Lab.Components;
using Radzen;
using Serilog;
using System.Data.Common;

var configBuilder = new ConfigurationBuilder();
BuildConfig(configBuilder);
var configuration = configBuilder.Build();
string ServerType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

// Read IsDevelopment from configuration
bool isDevelopment = configuration.GetValue<bool>("IsDevelopment");

var builder = WebApplication.CreateBuilder(args);

if (!isDevelopment)
{
    builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(configuration)
        .Enrich.WithProperty("ServerType", ServerType)
        .Enrich.WithProperty("Program", Util.PROGRAM_NAME));
}

//register the SQL Server library to be used for the MOO library
DbProviderFactories.RegisterFactory(MOO.Data.DBType.SQLServer.ToString(), Microsoft.Data.SqlClient.SqlClientFactory.Instance);
DbProviderFactories.RegisterFactory(MOO.Data.DBType.Oracle.ToString(), Oracle.ManagedDataAccess.Client.OracleClientFactory.Instance);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

if (!isDevelopment)
{
    builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
    builder.Services.AddAuthorization(options =>
    {
        // By default, all incoming requests will be authorized according to the default policy.
        options.FallbackPolicy = options.DefaultPolicy;
    });
    //adds code that will add user roles
    builder.Services.AddScoped<IClaimsTransformation, MOO.Blazor.Security.UserInfoClaims>();
}

builder.Services.AddRadzenComponents();
builder.Services.AddScoped<Radzen.DialogService>();
builder.Services.AddScoped<OM_Lab.Services.ICompTestService, OM_Lab.Services.CompTestService>();
builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = Util.APP_THEME_NAME; // The name of the cookie
    options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});

//This will be used so that the dates will default across each page request
builder.Services.AddScoped<OM_Lab.Data.Models.MetChangeDateVals>();
builder.Services.AddControllers();   //added this line so we can use controllers for the wood shipment export

//create an instance with a list of all possible LIMS Exports (see usage in AddEditLimsBatch.razor file)
builder.Services.AddSingleton<List<OM_Lab.Data.LIMSExports.EqpExport>>(OM_Lab.Data.LIMSExports.EqpExport.GetAllExports(builder.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();  //This forces browser to only work over HTTPS and any subsequent requests, not sure if we want this on.
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

if (!isDevelopment)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();   //added this line so we can use controllers

app.Run();



static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables();
}