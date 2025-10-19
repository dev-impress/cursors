namespace Receipts.Domain.Entities;

public class Receipt
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public DateTime IssuedAt { get; private set; }
    public string StoreName { get; private set; }
    public string Currency { get; private set; }
    public decimal Total { get; private set; }

    // Raw payload provided by user (JSON/text)
    public string Payload { get; private set; }

    private Receipt() { StoreName = string.Empty; Currency = "USD"; Payload = string.Empty; }

    public Receipt(DateTime issuedAt, string storeName, string currency, decimal total, string payload)
    {
        if (string.IsNullOrWhiteSpace(storeName)) throw new ArgumentException("StoreName required", nameof(storeName));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency required", nameof(currency));
        if (string.IsNullOrWhiteSpace(payload)) throw new ArgumentException("Payload required", nameof(payload));
        if (total < 0) throw new ArgumentOutOfRangeException(nameof(total));

        IssuedAt = issuedAt;
        StoreName = storeName.Trim();
        Currency = currency.Trim().ToUpperInvariant();
        Total = total;
        Payload = payload;
    }
}
