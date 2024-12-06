namespace PolicyInitiativeBuilder.Models;

public record Tenant(Guid Id, string DisplayName, string DefaultDomain)
{
    public override string ToString()
    {
        return $"{DisplayName} - {DefaultDomain}";
    }

    public static Tenant Empty => new(Guid.Empty, string.Empty, string.Empty);

    public bool IsEmpty => Id == Guid.Empty;
}