using Domain.Response;
using Microsoft.Extensions.DependencyInjection;
using BussinessLogic.Abstractions;



namespace DataValidation;

public static class RegisterValidationservices
{

     public static void useValidationServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentValidation, PaymentValidation>();

    }

}
