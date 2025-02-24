﻿using BussinessLogic;
using Domain;
using Domain.Request;
using Domain.Response;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Middlewares;




namespace WebAPI.Payments;

internal class PaymentApiEndPoint : IEndPointDefinition
{

    public void RegisterEndPoints(WebApplication application)
    {
        PaymentAPI(application.MapGroup(PaymentAPIConstants.PaymentAPIEndV1Point).WithOpenApi());
        PaymentAPI(application.MapGroup(PaymentAPIConstants.PaymentAPIEndPoint)).WithOpenApi().ExcludeFromDescription();

    }

    private static RouteGroupBuilder PaymentAPI(RouteGroupBuilder payment)
    {
        payment.MapGet("/ping", getAsync).Produces<Ok>();
        payment.MapPost("/", PaymentInsertAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();
        payment.MapPut("/", PaymentUpdateAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();
        payment.MapGet("/", PaymentFetchAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();
        payment.MapPost(PaymentAPIConstants.CapturePaymentEndpoint, CapturePaymentAsync).Produces<Ok>().Produces<NotFound>().Produces<BadRequest>();

        return payment;

    }

    private static async Task<IResult?> getAsync()
    {
        ServiceResult serviceResult = new();
        return ReturnResultBaseClass.returnResult<string?>(await serviceResult.GetServiceResponseAsync<string>("pong", ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null));
    }

    private static async Task<IResult?> PaymentInsertAsync([FromBody] ServiceRequest<PaymentModel> request, PaymentBL paymentBL)
    {
        var response = await paymentBL.InsertPayment(new RequestModel<PaymentModel> { RequestObject = request.RequestObject });
        return ReturnResultBaseClass.returnResult(response);
    }


    private static async Task<IResult?> PaymentUpdateAsync([FromBody] ServiceRequest<UpdatePaymentModel> request, PaymentBL paymentBL)
    {
         var response = await paymentBL.UpdatePaymentHeader(new RequestModel<UpdatePaymentModel> { RequestObject = request.RequestObject });
        return ReturnResultBaseClass.returnResult(response);

    }
    private static async Task<IResult?> PaymentFetchAsync([FromBody] ServiceRequest<PaymentFetchRequest> request, PaymentBL paymentBL)
    {
        var response = await paymentBL.FetchPaymentDetails(new RequestModel<PaymentFetchRequest> { RequestObject = request.RequestObject });
        return ReturnResultBaseClass.returnResult(response);
    }
    private static async Task<IResult?> CapturePaymentAsync([FromBody] ServiceRequest<PaymentRequest> request, PaymentBL paymentBL)
    {
        var response = await paymentBL.CapturePayment(new RequestModel<PaymentRequest> { RequestObject = request.RequestObject });
        return ReturnResultBaseClass.returnResult(response);
    }

}

