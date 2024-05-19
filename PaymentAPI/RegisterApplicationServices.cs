using WebAPI;
using DataAccessLayer;
using BussinessLogic;
using Infrastructure;
using System.Configuration;
using Domain;

namespace PaymentAPI;


public static class RegisterApplicationServices
{
    internal static IServiceCollection useApplicationServices(this WebApplicationBuilder builder)
    {

        var services = builder.Services;
        var configuration = builder.Configuration;

        string? _connectionString = configuration.GetConnectionString(ApplicationGenericConstants.DBCON_PARAM);
        if (string.IsNullOrWhiteSpace(_connectionString)) throw new ConfigurationErrorsException(ApplicationGenericConstants.MISSING_CONNECTION_STRING);


        PaymentSettings? paymentSettings = new PaymentSettings();
        paymentSettings = configuration.GetSection("PaymentSettings").Get<PaymentSettings>();
        if (paymentSettings is null) throw new ConfigurationErrorsException(ApplicationGenericConstants.MISSING_PAYMENT_SETTINGS);



        services.useAPIServices(APIconfig => APIconfig.Name = "PaymentAPI"); //to be changed
        services.useDALServices(DALconfig => DALconfig._connectionString = _connectionString);
        services.useBLServices(blconfig => blconfig.Name = "PaymentAPI"); //to be changed

        services.useInfraServices(Infraconfig => Infraconfig.PaymentSettings = paymentSettings);

        return services;


    }

}
