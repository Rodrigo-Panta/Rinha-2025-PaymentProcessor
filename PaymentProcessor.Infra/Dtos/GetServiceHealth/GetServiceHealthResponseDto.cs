using System.Net;

namespace PaymentProcessor.Infra.Dtos.GetServiceHealth;

public record GetServiceHealthResponseDto
{
    public required HttpStatusCode Status { get; set; }
    public bool IsFailing { get; set; }
    public int minResponseTime { get; set; }
}