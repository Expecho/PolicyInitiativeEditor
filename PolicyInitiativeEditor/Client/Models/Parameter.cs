﻿namespace PolicyInitiativeEditor.Client.Models
{
    public record Parameter(string Name, string Type, string? DefaultValue, string Description, string AllowedValues);

}