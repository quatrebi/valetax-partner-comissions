using Microsoft.AspNetCore.Http;

namespace Valetax.Infrastructure.Exceptions;

public abstract class ValetaxException(
    string message,
    int statusCode = StatusCodes.Status422UnprocessableEntity,
    TimeSpan? retryAfter = null)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public TimeSpan? RetryAfter { get; } = retryAfter;
}