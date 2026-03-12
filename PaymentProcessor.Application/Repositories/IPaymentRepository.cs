using PaymentProcessor.Application.Queries;
using PaymentProcessor.Application.Repositories.Base;
using PaymentProcessor.Domain.Entities;
using PaymentProcessor.Application.Dtos.GetPaymentSummary;

namespace PaymentProcessor.Application.Repositories;

public interface IPaymentRepository : IBaseRepository<Payment>
{
    public Task<GetPaymentSummaryResultDto> GetSummary(GetPaymentSummaryQuery query);
}
