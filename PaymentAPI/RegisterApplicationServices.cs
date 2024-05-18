using WebAPI;
using DataAccessLayer;
using BussinessLogic;
using Infrastructure;

namespace PaymentAPI;


public static class RegisterApplicationServices
{
    internal static IServiceCollection useApplicationServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        services.useAPIServices(APIconfig => APIconfig.Name = "PaymentAPI");
        services.useDALServices(DALconfig => DALconfig._connectionString = "Data Source=94.203.133.74,1433;Initial Catalog=QC_SaavyPayDB;user id=sbs_administrator;password=P@ssw0rd@2020");
        services.useBLServices(blconfig => blconfig.Name = "PaymentAPI");

        services.useInfraServices(Infraconfig => Infraconfig.PaymentSettings = new PaymentSettings
        {
            AdyenPaymentURL = "https://pal-test.adyen.com/pal/servlet/Payment/v52/capture",
            AdyenPaymentCurrency = "SGD",
            AccessTokenURL = "https://login.microsoftonline.com/a53a7a70-988d-4539-b456-708670a75463/oauth2/v2.0/token"
        });
        return services;


    }

}
