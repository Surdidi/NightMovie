using Blazored.LocalStorage;
using NightMovie.Web;
using NightMovie.Web.Components;
using NightMovie.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();
builder.Services.AddAuthorization();
builder.Services.AddHttpClient<WeatherApiClient>(client => client.BaseAddress = new("http://apiservice"));
builder.Services.AddHttpClient<NightMovieApiClient>(client => client.BaseAddress = new("https://nightmovie-api"));
builder.Services.AddHttpClient<TMDBClient>(client => client.BaseAddress = new("https://api.themoviedb.org/3/movie"));
builder.Services.AddScoped<AuthService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazorBootstrap();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
