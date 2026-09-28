using FluentValidation;

namespace Valetax.Infrastructure.Pagination;

public sealed class PageDtoValidator : AbstractValidator<PageDto>
{
    public PageDtoValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PageDto.MaxPageSize);
    }
}