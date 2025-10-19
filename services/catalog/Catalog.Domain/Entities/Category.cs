namespace Catalog.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private Category() { Name = string.Empty; }

    public Category(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        Name = name.Trim();
    }

    public void UpdateDescription(string? description)
    {
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}