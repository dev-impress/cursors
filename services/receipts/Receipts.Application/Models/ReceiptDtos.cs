namespace Receipts.Application.Models;

public record ReceiptCreateRequest(DateTime IssuedAt, string StoreName, string Currency, decimal Total, string Payload);
public record ReceiptResponse(Guid Id, DateTime IssuedAt, string StoreName, string Currency, decimal Total, string Payload);
