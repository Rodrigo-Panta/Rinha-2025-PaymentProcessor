namespace PaymentProcessor.Domain.Entities
{
    public class Payment
    {
        public Payment(Guid correlationId, decimal amount, DateTime createdDate)
        {
            CorrelationId = correlationId;
            Amount = amount;
            CreatedDate = createdDate;
        }

        public Guid CorrelationId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime CreatedDate { get; private set; }
    }
}