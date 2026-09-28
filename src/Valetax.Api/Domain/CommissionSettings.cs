namespace Valetax.Api.Domain;

public sealed class CommissionSettings
{
    public int Id { get; private set; }
    public CommissionSchemeType SchemaType { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public void SetSchemaType(CommissionSchemeType schemaType)
    {
        SchemaType = schemaType;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}