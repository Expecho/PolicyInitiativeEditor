namespace PolicyInitiativeEditor.Client.Models
{
    public record Tenant(string Id, string DisplayName, string DefaultDomain)
    {
        public override string ToString()
        {
            return $"{DisplayName} - {DefaultDomain}";
        }
    }

}