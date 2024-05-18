using WebAPI;
using DataAccessLayer;
using BussinessLogic;
using Infrastructure;
using System.Configuration;

namespace PaymentAPI;


public static class RegisterApplicationServices
{
    internal static IServiceCollection useApplicationServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        services.useAPIServices(APIconfig => APIconfig.Name = "PaymentAPI");
        services.useDALServices(DALconfig => DALconfig._connectionString = configuration.GetConnectionString("DBConnection"));
        services.useBLServices(blconfig => blconfig.Name = "PaymentAPI");

        services.useInfraServices(Infraconfig =>
        {

            PaymentSettings? paymentSettings = new PaymentSettings();
            paymentSettings = configuration.GetSection("PaymentSettings").Get<PaymentSettings>();
            Infraconfig.PaymentSettings = paymentSettings;

        });

        return services;


    }

}
