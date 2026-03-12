using PaymentProcessor.Application.Commands;
using PaymentProcessor.Application.Dtos.GetPaymentSummary;
using PaymentProcessor.Application.Queries;
using PaymentProcessor.Application.Repositories;
using PaymentProcessor.Domain.Entities;

namespace PaymentProcessor.Application.Handlers
{
    public class GetPaymentSummaryHandler
    {
        private readonly IPaymentRepository _paymentProcessor;
        public GetPaymentSummaryHandler(IPaymentRepository paymentProcessor)
        {
            _paymentProcessor = paymentProcessor;
        }

        public async Task<GetPaymentSummaryResultDto> Handle(GetPaymentSummaryQuery query)
        {
            return await _paymentProcessor.GetSummary(query);

        }
    }
}