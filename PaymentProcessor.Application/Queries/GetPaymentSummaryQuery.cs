namespace PaymentProcessor.Application.Queries;

public record GetPaymentSummaryQuery
{
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}