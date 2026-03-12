namespace PaymentProcessor.Infra.Dtos.CreatePaymentRequest;

public record CreatePaymentRequestDto
{
    public Guid CorrelationId { get; init; }
    public decimal Amount { get; init; }
    public DateTime requestedAt { get; init; }
}