using System.Net.Http;
using PaymentProcessor.Application.Repositories;
using PaymentProcessor.Infra.Dtos.GetServiceHealth;

namespace PaymentProcessor.Infra.Repositories;

// This interface only exists to be used as a specific repository for the dependency injection, so we can have multiple implementations of IPaymentRepository and choose which one to use in the composition root.
public interface IDefaultHttpPaymentRepository : IHttpPaymentRepository
{
}

public class DefaultHttpPaymentRepository : BaseHttpPaymentRepository, IDefaultHttpPaymentRepository
{
    public DefaultHttpPaymentRepository(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override string GetBaseUrl()
    {
        return Environment.GetEnvironmentVariable("DEFAULT_PAYMENT_PROCESSOR_URL") ?? string.Empty;
    }
}