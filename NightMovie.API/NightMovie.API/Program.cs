using LiteDB;
using Microsoft.IdentityModel.Tokens;
using NightMovie.API.Service.AuthentificationService;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options
        .AddPolicy("CorsPolicy",
            policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())); 

builder.Services.AddSingleton<ILiteDatabase>(new LiteDatabase("Data/NightMovieBdd.db"));
builder.Services.AddSingleton<IAuthentificationService, AuthenficationService>();
builder.Services.AddSingleton<ISeanceService, SeanceService>();

string strKey = builder.Configuration.GetValue<string>("Authentification:SignatureKey") ?? throw new InvalidOperationException();
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

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();