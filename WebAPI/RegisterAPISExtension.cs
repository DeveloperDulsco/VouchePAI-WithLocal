using Microsoft.AspNetCore.Builder;
using Middlewares;

namespace WebAPI;

public static class RegisterAPISExtension
{
    public static void usePaymentAPIS(this WebApplication app)
    {


        app.UseHttpsRedirection();
        // app.validateToken();
        app.AddApiEndPoints(typeof(WebAPI.RegisterAPISExtension).Assembly);

    }

}
