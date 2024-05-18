using BussinessLogic;

using Microsoft.Extensions.DependencyInjection;

namespace WebAPI;

public static class RegisterAPIServices
{
    public static void useAPIServices(this IServiceCollection services, Action<APIConfigutations> options)
    {
        services.AddScoped(p => new APIConfigutations(options));
        services.AddScoped<PaymentBL>();
    }


}



public class APIConfigutations
{
    public string? Name { get; set; }
    public APIConfigutations(Action<APIConfigutations> options)
    {
        options(this);
    }
}

