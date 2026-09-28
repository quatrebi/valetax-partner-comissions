using FluentValidation;

namespace Valetax.Partners.Api.Features.SetRefPartner;

public sealed class SetRefPartnerDtoValidator : AbstractValidator<SetRefPartnerDto>
{
    public SetRefPartnerDtoValidator()
    {
        RuleFor(x => x.RefPartnerExternalId)
            .NotEqual(Guid.Empty);
    }
}