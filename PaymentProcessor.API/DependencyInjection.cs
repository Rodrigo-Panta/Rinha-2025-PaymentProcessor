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
            services.AddHttpClient();

            // Application
            services.AddTransient<CreatePaymentHandler>();
            services.AddTransient<GetPaymentSummaryHandler>();

            // Infra
            services.AddTransient<IDefaultHttpPaymentRepository, DefaultHttpPaymentRepository>();
            services.AddTransient<IFallbackHttpPaymentRepository, FallbackHttpPaymentRepository>();
            services.AddTransient<IPaymentRepository, FacadePaymentRepository>();
        }
    }
}
