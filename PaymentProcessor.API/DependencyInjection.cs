using PaymentProcessor.Application.Handlers;
using PaymentProcessor.Application.Repositories;
using PaymentProcessor.Infra.Repositories;

namespace PaymentProcessor.API
{
    public static class DependencyInjection
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {

            services.AddControllers();

            services.AddOpenApi();
            services.AddHttpClient("HttpClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(3);
            });

            // Application
            services.AddScoped<CreatePaymentHandler>();
            services.AddScoped<GetPaymentSummaryHandler>();

            // Infra
            services.AddScoped<IDefaultHttpPaymentRepository, DefaultHttpPaymentRepository>();
            services.AddScoped<IFallbackHttpPaymentRepository, FallbackHttpPaymentRepository>();
            services.AddScoped<IPaymentRepository, FacadePaymentRepository>();
        }
    }
}
