using System.Globalization;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Valetax.Infrastructure.Persistence;

namespace Valetax.Infrastructure.Exceptions;

public sealed class ValetaxExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ValetaxExceptionHandler> logger) : IExceptionHandler
{
    private static readonly TimeSpan DependencyRetryAfter = TimeSpan.FromSeconds(5);

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken ct)
    {
        (int statusCode, string title, string detail, TimeSpan? retryAfter) = exception switch
        {
            ValetaxException e => (e.StatusCode, e.GetType().Name.Replace("Exception", string.Empty), e.Message, e.RetryAfter),
            BadHttpRequestException e => (e.StatusCode, "BadRequest", e.Message, null),
            RpcException { StatusCode: StatusCode.Unavailable or StatusCode.DeadlineExceeded or StatusCode.ResourceExhausted } =>
                (StatusCodes.Status503ServiceUnavailable, "DependencyUnavailable", "A dependent service is temporarily unavailable.", DependencyRetryAfter),
            RpcException =>
                (StatusCodes.Status502BadGateway, "DependencyFailed", "A dependent service returned an error.", null),
            DbUpdateException { IsForeignKeyViolation: true } =>
                (StatusCodes.Status409Conflict, "Conflict", "The operation conflicts with related data.", null),
            _ => (0, string.Empty, string.Empty, (TimeSpan?)null)
        };

        if (statusCode == 0)
            return false;

        if (statusCode >= StatusCodes.Status500InternalServerError)
            logger.LogWarning(exception, "Request failed because of a dependency: {Title}", title);

        httpContext.Response.StatusCode = statusCode;

        if (retryAfter is { } delay)
            httpContext.Response.Headers.RetryAfter = ((int)delay.TotalSeconds).ToString(CultureInfo.InvariantCulture);

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            }
        });
    }
}