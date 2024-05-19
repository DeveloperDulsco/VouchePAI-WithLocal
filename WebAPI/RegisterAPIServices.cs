using BussinessLogic;

using Microsoft.Extensions.DependencyInjection;

namespace WebAPI;

public static class RegisterAPIServices
{
    public static void useAPIServices(this IServiceCollection services, Action<APIConfigurations> options)
    {
        services.AddScoped(p => new APIConfigurations(options));
        services.AddScoped<PaymentBL>();
    }


}



public class APIConfigurations
{
    public string? Name { get; set; }
    public APIConfigurations(Action<APIConfigurations> options)
    {
        options(this);
    }
}

