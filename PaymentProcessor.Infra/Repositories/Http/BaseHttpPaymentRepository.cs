namespace PaymentProcessor.Infra.Repositories;

using System.Net.Http.Json;
using System.Web;
using PaymentProcessor.Application.Queries;
using PaymentProcessor.Application.Repositories;
using PaymentProcessor.Domain.Entities;
using PaymentProcessor.Infra.Dtos.CreatePaymentRequest;
using PaymentProcessor.Infra.Dtos.GetPayment;
using PaymentProcessor.Infra.Dtos.GetPaymentSummary;
using PaymentProcessor.Infra.Dtos.GetServiceHealth;

public abstract class BaseHttpPaymentRepository : IHttpPaymentRepository
{
    private readonly HttpClient _httpClient;


    public BaseHttpPaymentRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(GetBaseUrl());
    }

    protected abstract string GetBaseUrl();

    public async Task Add(Payment entity)
    {
        var request = new CreatePaymentRequestDto
        {
            CorrelationId = entity.CorrelationId,
            Amount = entity.Amount,
            requestedAt = entity.CreatedDate
        };

        var response = await _httpClient
            .PostAsJsonAsync("/payments", request);

        response.EnsureSuccessStatusCode();

    }


    public async Task<Payment?> GetByCorrelationId(Guid correlationId)
    {
        var response = await _httpClient
            .GetAsync($"/payments/{correlationId}");

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<GetPaymentResponseDto>();

        if (body == null) return null;

        return new Payment(
            body.CorrelationId,
            body.Amount,
            body.RequestedAt
        );
    }

    public async Task<GetServiceHealthResponseDto> GetHealth()
    {
        var response = await _httpClient
            .GetAsync("/payments/service-health");

        var body = await response.Content.ReadFromJsonAsync<JsonHealthResponse>();

        var responseDto = new GetServiceHealthResponseDto
        {
            Status = response.StatusCode,
            IsFailing = body?.IsFailing ?? true,
            minResponseTime = body?.minResponseTime ?? int.MaxValue
        };

        return responseDto;
    }

    public async Task<GetPaymentSummaryResponseDto> GetSummary(GetPaymentSummaryRequestDto request)
    {


        var builder = new UriBuilder(_httpClient.BaseAddress!);
        builder.Path = builder.Path.TrimEnd('/') + "/admin/payments-summary";
        var query = HttpUtility.ParseQueryString(builder.Query);

        if (request.From != null)
            query["from"] = request.From?.ToString("o");

        if (request.To != null)
            query["to"] = request.To?.ToString("o");

        builder.Query = query.ToString();
        string url = builder.ToString();
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

        requestMessage.Headers.Add("X-Rinha-Token", 123.ToString());
        try
        {
            var response = await _httpClient
                .SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new Exception("Error fetching payment summary: request failed " + content);
            }

            var body = await response.Content.ReadFromJsonAsync<GetPaymentSummaryResponseDto>();
            if (body == null)
                throw new Exception("Error fetching payment summary: response is null");
            return body;
        }
        catch (Exception ex)
        {
            throw new Exception("Error fetching payment summary", ex);
        }

    }


}

record JsonHealthResponse
{
    public bool IsFailing { get; set; }
    public int minResponseTime { get; set; }
}

