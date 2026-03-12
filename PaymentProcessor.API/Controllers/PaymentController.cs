using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaymentProcessor.Application.Commands;
using PaymentProcessor.Application.Handlers;
using PaymentProcessor.Application.Queries;

namespace PaymentProcessor.API.Controllers;

[ApiController]
public class PaymentController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<PaymentController> _logger;
    private readonly CreatePaymentHandler createPaymentHandler;
    private readonly GetPaymentSummaryHandler getPaymentSummaryHandler;

    public PaymentController(ILogger<PaymentController> logger, CreatePaymentHandler createPaymentHandler, GetPaymentSummaryHandler getPaymentSummaryHandler)
    {
        _logger = logger;
        this.createPaymentHandler = createPaymentHandler;
        this.getPaymentSummaryHandler = getPaymentSummaryHandler; ;
    }


    [HttpPost]
    [Route("/payments")]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
    {
        await createPaymentHandler.Handle(command);
        return Ok();
    }

    [HttpGet]
    [Route("payments-summary")]
    public async Task<IActionResult> GetPaymentSummary([FromQuery] GetPaymentSummaryQuery query)
    {
        var result = await getPaymentSummaryHandler.Handle(query);
        return Ok(result.map);
    }
}
