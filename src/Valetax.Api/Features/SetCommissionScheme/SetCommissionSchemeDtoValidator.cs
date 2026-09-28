using FluentValidation;

namespace Valetax.Api.Features.SetCommissionScheme;

public sealed class SetCommissionSchemeDtoValidator : AbstractValidator<SetCommissionSchemeDto>
{
    public SetCommissionSchemeDtoValidator()
    {
        RuleFor(x => x.SchemaType)
            .IsInEnum();
    }
}