using PaymentProcessor.Domain.Entities;
using PaymentProcessor.Infra.Dtos.GetPaymentSummary;
using PaymentProcessor.Infra.Dtos.GetServiceHealth;

namespace PaymentProcessor.Infra.Repositories;

public interface IHttpPaymentRepository
{
    Task Add(Payment entity);
    Task<Payment?> GetByCorrelationId(Guid correlationId);
    Task<GetServiceHealthResponseDto> GetHealth();
    Task<GetPaymentSummaryResponseDto> GetSummary(GetPaymentSummaryRequestDto request);
}
