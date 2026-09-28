using Valetax.Infrastructure;
using Valetax.Partners.Api.Features.GetPartnerTree;
using Valetax.Partners.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.UseValetaxInfrastructure<Program>()
    .UseDbContext<IPartnersDbContext, PartnersDbContext>();
builder.Services.AddScoped<IPartnerTreeService, PartnerTreeService>();

var app = builder.Build();

app.UseValetax<Program>();
app.MapGrpcService<GetPartnerTreeGrpcService>();

await app.SyncDbMigrationsAsync<PartnersDbContext>();

await app.RunAsync();