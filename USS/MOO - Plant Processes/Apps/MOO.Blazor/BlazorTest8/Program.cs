using BlazorTest8;
using BlazorTest8.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Radzen;
using Serilog;


var configBuilder = new ConfigurationBuilder();
BuildConfig(configBuilder);
string ServerType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";


var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(configBuilder.Build())
                .Enrich.WithProperty("ServerType", ServerType)
                .Enrich.WithProperty("Program", Util.PROGRAM_NAME));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddRadzenComponents();
builder.Services.AddHttpClient();

builder.Services.AddRadzenCookieThemeService(options =>
{
    options.Name = Util.APP_THEME_NAME; // The name of the cookie
    options.Duration = TimeSpan.FromDays(365); // The duration of the cookie
});
//adds code that will add user roles
builder.Services.AddScoped<IClaimsTransformation, MOO.Blazor.Security.UserInfoClaims>();



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

app.UseAuthentication();
app.UseAuthorization();

app.Run();



static void BuildConfig(IConfigurationBuilder builder)
{
    builder.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables();
}