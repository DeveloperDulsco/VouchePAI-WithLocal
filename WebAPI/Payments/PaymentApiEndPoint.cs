using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Middlewares;


namespace WebAPI.Payments
{
    internal class  PaymentApiEndPoint : IEndPointDefinintion
    {
        public void RegisterEndPoints(WebApplication application)
        {
            var PaymentGroup = application.MapGroup("v1/Payments").WithOpenApi();
            throw new NotImplementedException();
        }

        private static RouteGroupBuilder PaymentAPI(RouteGroupBuilder payment) {
            payment.MapGet("/Ping", getAsync).Produces<Ok>();
            return payment;

        }
        private static async Task<IResult?> getAsync()
        {

            return null;
        }

    
    }
}
