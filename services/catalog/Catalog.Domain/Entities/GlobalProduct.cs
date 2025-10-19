namespace Catalog.Domain.Entities;

public class GlobalProduct
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public Guid? CategoryId { get; private set; }

    // navigation
    public Category? Category { get; private set; }

    private GlobalProduct() { Name = string.Empty; }

    public GlobalProduct(string name, Guid? categoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        Name = name.Trim();
        CategoryId = categoryId;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        Name = name.Trim();
    }

    public void AssignCategory(Guid? categoryId)
    {
        CategoryId = categoryId;
    }
}
