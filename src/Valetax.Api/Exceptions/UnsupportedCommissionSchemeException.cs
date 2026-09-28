using Valetax.Api.Domain;

namespace Valetax.Api.Exceptions;

public sealed class UnsupportedCommissionSchemeException(CommissionSchemeType schemeType)
    : NotImplementedException($"Commission scheme '{schemeType}' is not implemented.");