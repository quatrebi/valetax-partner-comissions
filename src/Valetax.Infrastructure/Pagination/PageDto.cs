namespace Valetax.Infrastructure.Pagination;

public sealed record PageDto(int Page = 1, int PageSize = 50)
{
    public const int MaxPageSize = 100;
}