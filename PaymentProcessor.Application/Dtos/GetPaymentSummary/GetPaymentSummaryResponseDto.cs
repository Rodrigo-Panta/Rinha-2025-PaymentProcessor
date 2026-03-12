namespace PaymentProcessor.Application.Dtos.GetPaymentSummary;


public record GetPaymentSummaryResultDto
{
    public required IDictionary<string, PaymentSummaryDto> map { get; init; }
}

public record PaymentSummaryDto
{
    public required int TotalRequests { get; init; }

    public required double TotalAmount { get; init; }

    public required double TotalFee { get; init; }

    public required double FeePerTransaction { get; init; }
}

