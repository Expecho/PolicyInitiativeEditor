namespace PolicyInitiativeEditor.Client.Models
{
    public record Policy(string Id, string Name, string Type, string Category, string Description, IEnumerable<Parameter> Parameters);

    public record Parameter(string Name, string Type, string? DefaultValue, string Description, string AllowedValues);

    public record Tenant(string Id, string DisplayName, string DefaultDomain)
    {
        public override string ToString()
        {
            return $"{DisplayName} - {DefaultDomain}";
        }
    }

}