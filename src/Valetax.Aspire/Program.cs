var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres").WithDataVolume();
var partnersDatabase = postgres.AddDatabase("PartnersDbContext", "partners");
var walletsDatabase = postgres.AddDatabase("WalletsDbContext", "wallets");
var commissionsDatabase = postgres.AddDatabase("CommissionsDbContext", "commissions");
var kafka = builder.AddKafka("kafka").WithDataVolume();

// "Http" and "Grpc" endpoints come from the Kestrel section of each service's appsettings.json.
var partnersApi = builder.AddProject<Projects.Valetax_Partners_Api>("partners-api", launchProfileName: "http")
    .WithReference(partnersDatabase)
    .WithEnvironment("Kafka__Host", kafka)
    .WaitFor(partnersDatabase)
    .WaitFor(kafka)
    .WithHttpHealthCheck("/health", endpointName: "Http");

var walletsApi = builder.AddProject<Projects.Valetax_Wallets_Api>("wallets-api", launchProfileName: "http")
    .WithReference(walletsDatabase)
    .WithEnvironment("Kafka__Host", kafka)
    .WaitFor(walletsDatabase)
    .WaitFor(kafka)
    .WithHttpHealthCheck("/health", endpointName: "Http");

builder.AddProject<Projects.Valetax_Api>("valetax-api", launchProfileName: "http")
    .WithReference(commissionsDatabase)
    .WithEnvironment("GrpcServices__Partners", partnersApi.GetEndpoint("Grpc"))
    .WithEnvironment("GrpcServices__Wallets", walletsApi.GetEndpoint("Grpc"))
    .WaitFor(commissionsDatabase)
    .WaitFor(partnersApi)
    .WaitFor(walletsApi)
    .WithHttpHealthCheck("/health");

await builder.Build().RunAsync();