var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.NightMovie_ApiService>("apiservice");
var nightMovieapi = builder.AddProject<Projects.NightMovie_API>("nightmovie-api");


builder.AddProject<Projects.NightMovie_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(nightMovieapi);


builder.Build().Run();
