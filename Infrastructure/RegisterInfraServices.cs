namespace Infrastructure;

using BussinessLogic;
using Microsoft.Extensions.DependencyInjection;



public static class RegisterInfraServices
{
    public static void useInfraServices(this IServiceCollection services, Action<InfraConfigutations> options)
    {
        services.AddScoped(p => new InfraConfigutations(options));
        services.AddScoped<ITokenRequest, TokenService>();
        services.AddScoped<IAdenPayment, PaymentService>();

    }


}

public class InfraConfigutations
{
    public PaymentSettings? PaymentSettings { get; set; }
    public InfraConfigutations(Action<InfraConfigutations> options)
    {
        options(this);
    }
}



public class PaymentSettings
{

    public string? AdyenPaymentURL { get; set; }
    public string? AdyenPaymentCurrency { get; set; }
    public string? AccessTokenURL { get; set; }
}

