namespace PaymentProcessor.Infra.Repositories;

using System;
using System.Net;
using PaymentProcessor.Application.Queries;
using PaymentProcessor.Application.Repositories;
using PaymentProcessor.Domain.Entities;
using PaymentProcessor.Application.Dtos.GetPaymentSummary;
using PaymentProcessor.Infra.Dtos.GetPaymentSummary;

public class FacadePaymentRepository : IPaymentRepository
{
    private readonly IDefaultHttpPaymentRepository defaultRepository;
    private readonly IFallbackHttpPaymentRepository fallbackRepository;

    public FacadePaymentRepository(IDefaultHttpPaymentRepository defaultRepository, IFallbackHttpPaymentRepository fallbackRepository)
    {
        this.defaultRepository = defaultRepository;
        this.fallbackRepository = fallbackRepository;
    }

    public async Task Add(Payment entity)
    {
        var defaultRepositoryHealth = await defaultRepository.GetHealth();
        var defaultIsHealthy = defaultRepositoryHealth.Status == HttpStatusCode.OK && !defaultRepositoryHealth.IsFailing;


        if (defaultIsHealthy && defaultRepositoryHealth.minResponseTime < 1000)
        {
            var success = await TryAddToRepository(defaultRepository, entity, "default");
            if (success) return;
        }

        var fallbackRepositoryHealth = await fallbackRepository.GetHealth();
        var fallbackIsHealthy = fallbackRepositoryHealth.Status == HttpStatusCode.OK && !fallbackRepositoryHealth.IsFailing;

        if (!defaultIsHealthy && fallbackIsHealthy)
        {
            var success = await TryAddToRepository(fallbackRepository, entity, "fallback");
            if (success) return;
        }

        if (defaultIsHealthy && fallbackIsHealthy && defaultRepositoryHealth.minResponseTime < fallbackRepositoryHealth.minResponseTime)
        {
            var success = await TryAddToRepository(defaultRepository, entity, "default (healthy but fallback is slower)");
            if (success) return;
        }

    }

    public async Task<Payment?> GetByCorrelationId(Guid correlationId)
    {
        var PaymentFromDefault = await defaultRepository.GetByCorrelationId(correlationId);
        if (PaymentFromDefault != null) return PaymentFromDefault;
        return await fallbackRepository.GetByCorrelationId(correlationId);
    }

    public async Task<GetPaymentSummaryResultDto> GetSummary(GetPaymentSummaryQuery query)
    {
        var defaultSummaryTask = defaultRepository.GetSummary(new GetPaymentSummaryRequestDto
        {
            From = query.From,
            To = query.To
        });

        var fallbackSummaryTask = fallbackRepository.GetSummary(new GetPaymentSummaryRequestDto
        {
            From = query.From,
            To = query.To
        });

        var results = await Task.WhenAll(defaultSummaryTask, fallbackSummaryTask);

        var resultDto = new GetPaymentSummaryResultDto
        {
            map = new Dictionary<string, PaymentSummaryDto>
            {
                {
                    "default",
                    new PaymentSummaryDto{
                     TotalRequests = results[0].TotalRequests,
                     TotalAmount = results[0].TotalAmount,
                     TotalFee = results[0].TotalFee,
                     FeePerTransaction = results[0].FeePerTransaction
                    }
                },
                {
                    "fallback",
                    new PaymentSummaryDto{
                     TotalRequests = results[1].TotalRequests,
                     TotalAmount = results[1].TotalAmount,
                     TotalFee = results[1].TotalFee,
                     FeePerTransaction = results[1].FeePerTransaction
                    }
                }
            }
        };

        return resultDto;
    }

    private async Task<bool> TryAddToRepository(IHttpPaymentRepository repository, Payment entity, string repositoryName)
    {
        try
        {
            await repository.Add(entity);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding payment to repository {repositoryName}: {ex.Message}");
            return false;
        }
    }
}