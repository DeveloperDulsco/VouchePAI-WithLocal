using Microsoft.Extensions.DependencyInjection;
using BussinessLogic.Abstractions;
using DataAccessLayer.Repository;


namespace DataAccessLayer;

public static class RegisterDALServices
{
    public static void useDALServices(this IServiceCollection services, Action<DALConfigutations> options)
    {

        services.AddScoped<IPayment, PaymentRepository>();
        services.AddScoped(p => new DALConfigutations(options));


    }

}


public class DALConfigutations
{
    public string? _connectionString { get; set; }
    public DALConfigutations(Action<DALConfigutations> options)
    {
        options(this);
    }
}

