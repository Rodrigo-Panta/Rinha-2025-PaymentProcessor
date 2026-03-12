using System.ComponentModel.DataAnnotations;

namespace PaymentProcessor.Application.Commands
{
    public class CreatePaymentCommand
    {
        [Required]
        public Guid CorrelationId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}