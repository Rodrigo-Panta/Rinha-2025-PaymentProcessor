using PaymentProcessor.API;
using PaymentProcessor.API.ExceptionFilters;

var builder = WebApplication.CreateBuilder(args);

// DI
builder.Services.AddDependencyInjection();
builder.WebHost.UseUrls("http://0.0.0.0:8080");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
