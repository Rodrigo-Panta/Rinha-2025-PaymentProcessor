namespace PaymentProcessor.Infra.Repositories;

using System;
using System.Net;
using PaymentProcessor.Application.Queries;
using PaymentProcessor.Application.Repositories;
using PaymentProcessor.Domain.Entities;
using PaymentProcessor.Application.Dtos.GetPaymentSummary;
using PaymentProcessor.Infra.Dtos.GetPaymentSummary;
using Microsoft.Extensions.Logging;

public class FacadePaymentRepository : IPaymentRepository
{
    private readonly IDefaultHttpPaymentRepository defaultRepository;
    private readonly IFallbackHttpPaymentRepository fallbackRepository;

    private readonly ILogger<FacadePaymentRepository> _logger;

    public FacadePaymentRepository(IDefaultHttpPaymentRepository defaultRepository, IFallbackHttpPaymentRepository fallbackRepository, ILogger<FacadePaymentRepository> logger)
    {
        this.defaultRepository = defaultRepository;
        this.fallbackRepository = fallbackRepository;
        _logger = logger;
    }

    public async Task Add(Payment entity)
    {
        bool defaultIsHealthy;
        int? defaultResponseTime = null;
        bool fallbackIsHealthy;
        int? fallbackResponseTime = null;

        try
        {
            var defaultRepositoryHealth = await defaultRepository.GetHealth();
            defaultIsHealthy = defaultRepositoryHealth.Status == HttpStatusCode.OK && !defaultRepositoryHealth.IsFailing;
            defaultResponseTime = defaultRepositoryHealth.minResponseTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health of default repository");
            defaultIsHealthy = false;
        }


        if (defaultIsHealthy && defaultResponseTime.HasValue && defaultResponseTime.Value < 1000)
        {
            var success = await TryAddToRepository(defaultRepository, entity, "default");
            if (success) return;
        }

        try
        {
            var fallbackRepositoryHealth = await fallbackRepository.GetHealth();
            fallbackIsHealthy = fallbackRepositoryHealth.Status == HttpStatusCode.OK && !fallbackRepositoryHealth.IsFailing;
            fallbackResponseTime = fallbackRepositoryHealth.minResponseTime;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking health of fallback repository");
            fallbackIsHealthy = false;
        }


        if (!defaultIsHealthy && fallbackIsHealthy)
        {
            var success = await TryAddToRepository(fallbackRepository, entity, "fallback");
            if (success) return;
        }

        if (defaultIsHealthy && fallbackIsHealthy && defaultResponseTime < fallbackResponseTime)
        {
            var success = await TryAddToRepository(defaultRepository, entity, "default (healthy but fallback is slower)");
            if (success) return;
        }

        if (fallbackIsHealthy)
        {
            var success = await TryAddToRepository(defaultRepository, entity, "default (healthy but fallback is slower)");
            if (success) return;
        }

        throw new HttpRequestException("All payment repositories are unavailable", new Exception(), HttpStatusCode.ServiceUnavailable);
    }

    public async Task<Payment?> GetByCorrelationId(Guid correlationId)
    {
        try
        {
            var PaymentFromDefault = await defaultRepository.GetByCorrelationId(correlationId);
            if (PaymentFromDefault != null) return PaymentFromDefault;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HttpRequestException occurred while fetching payment by correlation ID from default repository");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching payment by correlation ID from default repository");
        }

        try
        {
            return await fallbackRepository.GetByCorrelationId(correlationId);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HttpRequestException occurred while fetching payment by correlation ID from fallback repository");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while fetching payment by correlation ID from fallback repository");
            throw new Exception("An unexpected error occurred while fetching payment data", ex);
        }
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