namespace PolicyInitiativeEditor.Shared
{
    public record Policy(string Id, string Name, string Type, string Category, string Description, IEnumerable<Parameter> Parameters);

    public record Parameter(string Name, string Type, string? DefaultValue, string Description, string AllowedValues);

}