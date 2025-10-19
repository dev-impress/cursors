namespace Catalog.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public Guid StoreId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public decimal Price { get; private set; }

    // navigation
    public Store Store { get; private set; } = default!;
    public Category? Category { get; private set; }

    private Product() { Name = string.Empty; }

    public Product(string name, Guid storeId, decimal price, Guid? categoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
        Name = name.Trim();
        StoreId = storeId;
        Price = price;
        CategoryId = categoryId;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));
        Name = name.Trim();
    }

    public void ChangePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative");
        Price = price;
    }

    public void AssignCategory(Guid? categoryId)
    {
        CategoryId = categoryId;
    }
}