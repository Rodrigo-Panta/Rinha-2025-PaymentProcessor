using System.Text.Json.Serialization;

namespace PaymentProcessor.Infra.Dtos.GetPaymentSummary;

public record GetPaymentSummaryRequestDto
{
    [JsonPropertyName("from")]
    public DateTime? From { get; init; }
    [JsonPropertyName("to")]
    public DateTime? To { get; init; }
}