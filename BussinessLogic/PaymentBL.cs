using BussinessLogic.Abstractions;
using Domain;
using Domain.Response;
using Domain.Responses;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Json;



namespace BussinessLogic;

    public class PaymentBL
    {
        IPayment payment;
        ITokenRequest tokenRequest;
        IAdenPayment adenPayment;
        public PaymentBL(IPayment _ipayment , ITokenRequest _itokenRequest, IAdenPayment _iadenPayment)
        {
                payment = _ipayment;
                tokenRequest = _itokenRequest;
                adenPayment = _iadenPayment;
        }
        
        public async Task<ServiceResponse<object>> InsertPayment(RequestModel<PaymentModel> request)
        {
            ServiceResult serviceResult = new ServiceResult();

            if (request?.RequestObject is null) return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

            PaymentModel? paymentDetails = request?.RequestObject;
            var respose = await payment.InsertPayment(request);

            if (respose is not null)
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);





        }
        public async Task<ServiceResponse<object>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request)
        {
            ServiceResult serviceResult = new ServiceResult();
            if (request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

            UpdatePaymentModel? updatePayment = request?.RequestObject;
            var respose = await payment.UpdatePaymentHeader(request);

            if (respose is not null)
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
        }
        public async Task<ServiceResponse<IEnumerable<FetchPaymentTransaction>>> FetchPaymentDetails(RequestModel<string> request)
        {

            ServiceResult serviceResult = new ServiceResult();
            if (request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction>>(null, "Invalid Payment Request", ApiResponseCodes.FAILURE, 400, null);

            var respose = await payment.FetchPaymentDetails(request);
            if (respose is not null)
                return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);



        }


        public async Task<ServiceResponse<PaymentResponse>>  CapturePayment(RequestModel<PaymentRequest> request)
        {
            ServiceResult serviceResult = new ServiceResult();
            if (request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<PaymentResponse>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

            PaymentRequest? paymentRequest = request?.RequestObject;
            var respose = await adenPayment.CapturePayment(paymentRequest);

            var paymentresponse=payment.CapturePayment(request);



            if (respose is not null)
                return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync<PaymentResponse>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
       
        }

    public async Task<ServiceResponse<T>> GetAccessToken<T>(Dictionary<string, StringValues> request)
    {
        ServiceResult serviceResult = new ServiceResult();
        if (request is null)
            return await serviceResult.GetServiceResponseAsync<T>(default, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        var response = await tokenRequest.GetAccessToken(request);
        if (response is not null)
            return await serviceResult.GetServiceResponseAsync(response?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
        else
            return await serviceResult.GetServiceResponseAsync<T>(default, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
    }
}
