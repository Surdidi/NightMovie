using Aspire.Hosting;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.NightMovie_ApiService>("apiservice");


var nightMovieapi = builder.AddProject<Projects.NightMovie_API>("nightmovie-api").WithEnvironment("NightMovieDBConnectionString", builder.Configuration.GetValue<string>("AZURE_SQL_CONNECTIONSTRING")).WithEnvironment("Authentification_SignatureKey", builder.Configuration.GetValue<string>("Authentification_SignatureKey"));


builder.AddProject<Projects.NightMovie_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(nightMovieapi);


builder.Build().Run();
