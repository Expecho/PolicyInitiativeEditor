namespace PolicyInitiativeEditor.Client.Models
{
    public record Policy(string Id, string Name, string Type, string Category, string Description, IEnumerable<Parameter> Parameters);
}