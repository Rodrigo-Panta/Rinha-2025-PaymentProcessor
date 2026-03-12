using System.Text.Json.Serialization;

namespace PaymentProcessor.Infra.Dtos.GetPaymentSummary;

public record GetPaymentSummaryResponseDto
{

    [JsonPropertyName("totalRequests")]
    public int TotalRequests { get; init; }

    [JsonPropertyName("totalAmount")]
    public double TotalAmount { get; init; }

    [JsonPropertyName("totalFee")]
    public double TotalFee { get; init; }

    [JsonPropertyName("feePerTransaction")]
    public double FeePerTransaction { get; init; }
}