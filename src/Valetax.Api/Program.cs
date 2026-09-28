using Valetax.Api.Infrastructure;
using Valetax.Api.Jobs;
using Valetax.Api.Persistence;
using Valetax.Api.Services;
using Valetax.Infrastructure;
using Valetax.Infrastructure.Grpc;
using Valetax.Partners.Api.Grpc;
using Valetax.Wallets.Api.Grpc;

var builder = WebApplication.CreateBuilder(args);

builder.UseValetaxInfrastructure<Program>()
    .UseDbContext<ICommissionsDbContext, CommissionsDbContext>()
    .UseGrpcClient<PartnersService.PartnersServiceClient>("Partners")
    .UseGrpcClient<WalletsService.WalletsServiceClient>("Wallets")
    .UseGrpcClient<WalletPayoutsService.WalletPayoutsServiceClient>("Wallets");

builder.Services.AddSingleton<CommissionsMetrics>();
builder.Services.AddScoped<ICommissionService, CommissionService>();
builder.Services.AddScoped<IPartnersClient, PartnersClient>();
builder.Services.AddScoped<IWalletsClient, WalletsClient>();
builder.Services.AddScoped<ICommissionPayoutService, CommissionPayoutService>();

var app = builder.Build();

app.UseValetax<Program>();
await app.SyncDbMigrationsAsync<CommissionsDbContext>();

await app.RunAsync();