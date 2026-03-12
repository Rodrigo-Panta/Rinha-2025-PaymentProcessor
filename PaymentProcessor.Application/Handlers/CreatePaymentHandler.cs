using PaymentProcessor.Application.Commands;
using PaymentProcessor.Application.Repositories;
using PaymentProcessor.Domain.Entities;

namespace PaymentProcessor.Application.Handlers
{
    public class CreatePaymentHandler
    {
        private readonly IPaymentRepository _paymentProcessor;
        public CreatePaymentHandler(IPaymentRepository paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        public async Task Handle(CreatePaymentCommand command)
        {

            if (command.Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            var existingPayment = await _paymentProcessor.GetByCorrelationId(command.CorrelationId);

            if (existingPayment != null)
                throw new InvalidOperationException("A payment with the same CorrelationId already exists.");

            var payment = new Payment(
                command.CorrelationId,
                command.Amount,
                DateTime.UtcNow
            );

            await _paymentProcessor.Add(payment);
        }
    }
}