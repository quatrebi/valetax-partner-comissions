using Valetax.Api.Domain;

namespace Valetax.Api.Services;

public interface ICommissionService
{
    decimal Calculate(decimal profit, int level, CommissionSchemeType schemaType);
}