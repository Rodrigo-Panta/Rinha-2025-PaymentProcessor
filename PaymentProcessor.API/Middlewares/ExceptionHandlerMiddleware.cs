using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PaymentProcessor.Domain.Exceptions;

namespace PaymentProcessor.API.ExceptionFilters;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private async Task HandleException(HttpContext context, Exception ex)
    {
        var problem = MapException(ex);

        _logger.LogError(ex, "Unhandled exception occurred");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = problem.Status ?? 500;

        var json = JsonSerializer.Serialize(problem);

        await context.Response.WriteAsync(json);
    }

    private ProblemDetails MapException(Exception ex)
    {
        return ex switch
        {
            InvalidInputException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid input",
                Detail = e.Message
            },
            DuplicatedEntityException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Duplicated entity",
                Detail = e.Message
            },
            HttpRequestException e => new ProblemDetails
            {
                Status = ((int?)e.StatusCode) ?? StatusCodes.Status500InternalServerError,
                Title = "Service failure",
                Detail = e.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected server error",
                Detail = "An unexpected error occurred."
            }
        };
    }
}