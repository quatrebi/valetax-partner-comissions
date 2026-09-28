namespace Valetax.Infrastructure.Pagination;

public static class QueryableExtensions
{
    extension<T>(IQueryable<T> query)
    {
        public IQueryable<T> Paginate(PageDto page) => query
            .Skip((page.Page - 1) * page.PageSize)
            .Take(page.PageSize);
    }
}