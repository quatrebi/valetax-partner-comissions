using FluentValidation;

namespace Valetax.Partners.Api.Features.CreatePartner;

public sealed class CreatePartnerDtoValidator : AbstractValidator<CreatePartnerDto>
{
    public CreatePartnerDtoValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotEmpty();

        RuleFor(x => x.RefPartnerExternalId)
            .NotEqual(Guid.Empty);
    }
}