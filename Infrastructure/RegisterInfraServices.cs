

using BussinessLogic;
using BussinessLogic.Abstractions;
using Infrastructure.OwsHelper;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class RegisterInfraServices
{
    public static void useInfraServices(this IServiceCollection services, Action<InfraConfigutations> options)
    {
        services.AddScoped(p => new InfraConfigutations(options));
        services.AddScoped<ITokenRequest, TokenService>();
        services.AddScoped<IAdenPayment, PaymentService>();
        services.AddScoped<IOperaService, OperaServices>();

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
    public string? ApiKey { get; set; }
    public string? MerchantAccount { get; set; }
    public string? WSSEUserName { get; set; }
    public string? WSSEPassword { get; set; }
    public bool OperaCloudEnabled { get; set; }
    public bool OPIEnabled { get; set; }
    public string? HotelDomain { get; set; }
    public string? KioskID { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? SystemType { get; set; }
    public string? Language { get; set; }
    public string? LegNumber { get; set; }
    public string? ChainCode { get; set; }
    public string? DestinationEntityID { get; set; }
    public string? PreAuthUDF { get; set; }
    public string? PreAuthAmntUDF { get; set; }
    public string? DestinationSystemType { get; set; }
    public string? GuranteeTypeCode { get; set; }
    public string? NameOWSURL { get; set; }
    public string? InformationOWSURL { get; set; }

    public string? ReservatinOWSURL { get; set; }

    public string? ReservationAdvancedOWSURL { get; set; }
    public string? LocalAPIURL { get; set; }


}

