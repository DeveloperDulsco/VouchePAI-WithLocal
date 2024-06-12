
using Microsoft.Extensions.DependencyInjection;

namespace BussinessLogic;

public static  class RegisterBLServices
{
    public static void useBLServices(this IServiceCollection services,Action<BLConfigutations> options)
    {
       services.AddScoped(p=> new BLConfigutations(options));
      
    }


}

public class BLConfigutations
{
   public PmsUpdateSettings? settings { get; set; }

   public BLConfigutations(Action<BLConfigutations> options)
   {
    options(this);
   }
}


