using Microsoft.Extensions.DependencyInjection;
using BussinessLogic.Abstractions;
using DataAccessLayer.Repository;


namespace DataAccessLayer;

public static class RegisterDALServices
{
    public static void useDALServices(this IServiceCollection services, Action<DALConfigurations> options)
    {

        services.AddScoped<IPayment, PaymentRepository>();
        services.AddScoped(p => new DALConfigurations(options));


    }

}


public class DALConfigurations
{
    public string? _connectionString { get; set; }
    public DALConfigurations(Action<DALConfigurations> options)
    {
        options(this);
    }
}

