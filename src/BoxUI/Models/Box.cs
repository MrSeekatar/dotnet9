namespace BoxUI.Models;

internal record Box(string Name, Guid? BoxId, string Description, DateTime CreatedOn, bool Active = true);