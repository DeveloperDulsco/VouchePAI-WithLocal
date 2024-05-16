using BussinessLogic;
using DataAccessLayer.Model;
using Domain;
using Domain.Request;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        payment.MapPost("/", PaymentInsertAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();
        payment.MapPut("/", PaymentUpdateAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();
        payment.MapGet("/", PaymentFetchAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();
        payment.MapPost("/CapturePaymentAsync", CapturePaymentAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();

        return payment;

        }
 
    private static async Task<IResult?> getAsync()
        {
        
           ServiceResult serviceResult=new ServiceResult();
           return ReturnResultBaseClass.returnResult<string> (await serviceResult.GetServiceResponseAsync<string>("pong",ApplicationGenericConstants.SUCCESS,ApiResponseCodes.SUCCESS, 200,null));
        }

    private static async Task<IResult?> PaymentInsertAsync([FromBody]  ServiceRequest<PaymentModel> request)
    {
            
              var response = await new PaymentBL().InsertPayment(new RequestModel<PaymentModel> { RequestObject = request.RequestObject});
              return ReturnResultBaseClass.returnResult(response);
    }
          
    
    private static async Task<IResult?> PaymentUpdateAsync([FromBody] ServiceRequest<UpdatePaymentModel> request)
    {
        
        var response = await new PaymentBL().UpdatePaymentHeader(new RequestModel<UpdatePaymentModel> { RequestObject = request.RequestObject});
        return ReturnResultBaseClass.returnResult(response);
        
    }
    private static async Task<IResult?> PaymentFetchAsync([FromBody] ServiceRequest<string> request)
    {
    
            var response = await new PaymentBL().FetchPaymentDetails(new RequestModel<string> { RequestObject = request.RequestObject});
            return ReturnResultBaseClass.returnResult(response);
    }
    private static async Task<IResult?> CapturePaymentAsync([FromBody] ServiceRequest<PaymentRequest> request)
    {

        var response = await new PaymentBL().CapturePayment(new RequestModel<PaymentRequest> { RequestObject = request.RequestObject });
        return ReturnResultBaseClass.returnResult(response);
    }
    
}

