using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Middlewares;



namespace WebAPI.Payments;

    internal class  PaymentApiEndPoint : IEndPointDefinition
    {
        public void RegisterEndPoints(WebApplication application)
        {
            var PaymentGroup = application.MapGroup("v1/Payments").WithOpenApi();
            PaymentAPI(PaymentGroup);
           
        }

        private static RouteGroupBuilder PaymentAPI(RouteGroupBuilder payment) {
            payment.MapGet("/ping", getAsync).Produces<Ok>();
            return payment;

        }

        private static async Task<IResult?> getAsync()
        {
           ServiceResult serviceResult=new ServiceResult();
           return ReturnResultBaseClass.returnResult<string> (await serviceResult.GetServiceResponseAsync<string>("pong",ApplicationGenericConstants.SUCCESS,ApiResponseCodes.SUCCESS, 200,null));
        }
       
    
    }

