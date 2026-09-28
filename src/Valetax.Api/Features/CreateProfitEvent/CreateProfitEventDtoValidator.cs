using FluentValidation;

namespace Valetax.Api.Features.CreateProfitEvent;

public sealed class CreateProfitEventDtoValidator : AbstractValidator<CreateProfitEventDto>
{
    public CreateProfitEventDtoValidator()
    {
        RuleFor(x => x.ExternalId)
            .NotEmpty();

        RuleFor(x => x.PartnerExternalId)
            .NotEmpty();

        RuleFor(x => x.Profit)
            .PrecisionScale(Constants.Money.Precision, Constants.Money.Scale, ignoreTrailingZeros: true);
    }
}