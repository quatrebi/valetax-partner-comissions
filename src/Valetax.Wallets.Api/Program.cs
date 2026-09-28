using Valetax.Infrastructure;
using Valetax.Wallets.Api.Features.GetWallet;
using Valetax.Wallets.Api.Features.PayoutCommission;
using Valetax.Wallets.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.UseValetaxInfrastructure<Program>()
    .UseDbContext<IWalletsDbContext, WalletsDbContext>();

var app = builder.Build();

app.UseValetax<Program>();
app.MapGrpcService<GetWalletGrpcService>();
app.MapGrpcService<PayoutCommissionGrpcService>();

await app.SyncDbMigrationsAsync<WalletsDbContext>();

await app.RunAsync();