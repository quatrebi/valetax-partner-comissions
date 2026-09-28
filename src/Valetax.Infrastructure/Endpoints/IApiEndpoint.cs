using Microsoft.AspNetCore.Routing;

namespace Valetax.Infrastructure.Endpoints;

public interface IApiEndpoint
{
    public void UseMetadata(IEndpointRouteBuilder routeBuilder);
}

public interface IApiEndpoint<TRequest> : IApiEndpoint;