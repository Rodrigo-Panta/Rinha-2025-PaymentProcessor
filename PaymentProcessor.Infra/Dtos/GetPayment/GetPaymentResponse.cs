using System.Text.Json.Serialization;

namespace PaymentProcessor.Infra.Dtos.GetPayment;

public record GetPaymentResponseDto
{
    [JsonPropertyName("correlationId")]
    public Guid CorrelationId { get; init; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    [JsonPropertyName("requestedAt")]
    public DateTime RequestedAt { get; init; }
}