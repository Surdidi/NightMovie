using Microsoft.IdentityModel.Tokens;
using NightMovie.API.Service.AuthentificationService;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Cors;
using NightMovie.API.Database;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddCors(options => options
        .AddPolicy("AllowAllOrigins",
            policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
{
    builder.WithOrigins("http://192.168.1.44:8100", "http://88.172.131.125:17004").AllowAnyMethod().AllowAnyHeader();
}));

//builder.Services.AddSingleton<ILiteDatabase>(new LiteDatabase("Data/NightMovieBdd.db"));
builder.Services.AddSingleton<IAuthentificationService, AuthenficationService>();

var connection =  Environment.GetEnvironmentVariable("NightMovieDBConnectionString");



builder.Services.AddDbContext<NightMovieContext>(options =>
options.UseSqlServer(connection));
builder.Services.AddScoped<ISeanceService, SeanceService>();


using (var sp = builder.Services.BuildServiceProvider())
{
    try
    {
        var context = sp.GetRequiredService<NightMovieContext>();
        var auth = sp.GetRequiredService<IAuthentificationService>();
        NightMovieDBInitializer.Initialize(context, auth);
    }
    catch (Exception ex)
    {
        var logger = sp.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

string strKey = builder.Configuration.GetValue<string>("Authentification_SignatureKey") ?? throw new InvalidOperationException();
var TokenValidationParameters = new TokenValidationParameters
{
    ValidIssuer = "nightMovie.API",
    ValidAudience = "nightMovie.API",
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(strKey)),
    ClockSkew = TimeSpan.Zero // remove delay of token when expire
};

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.TokenValidationParameters = TokenValidationParameters;
    });

builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("Admin", policy => policy.RequireClaim("isAdmin", "TRUE"));
    cfg.AddPolicy("User", policy => policy.RequireClaim("type", "FALSE"));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NightMovie.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                },
                Array.Empty<string>()
            }
        });
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", new[] { (string)context.Request.Headers["Origin"] });
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.StatusCode = 200;
        return;
    }
    await next();
});

// Configure the HTTP request pipeline.
app.UseRouting();
app.UseCors("ApiCorsPolicy");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.Run();