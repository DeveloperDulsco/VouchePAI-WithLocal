using BussinessLogic.Abstractions;
using Domain;
using Domain.Response;
using Domain.Responses;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Json;



namespace BussinessLogic
{
    public class PaymentBL
    {
        IPayment payment;
        public PaymentBL(IPayment _payment)
        {
             payment=_payment;
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
        public async Task<ServiceResponse<PaymentResponse>> CapturePayment(RequestModel<PaymentRequest> request)
        {
            PaymentResponse paymentResponseObject = new PaymentResponse();
            ServiceResult serviceResult = new ServiceResult();
            if (request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<PaymentResponse>(null, "Invalid Capture Request", ApiResponseCodes.FAILURE, 400, null);

            ResponseModel<PaymentResponse> responseModel = new ResponseModel<PaymentResponse>();

            var response = await payment.CapturePayment(request);
            
            if (responseModel is not null && responseModel.Result)
                return await serviceResult.GetServiceResponseAsync(responseModel.ResponseObject, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync(responseModel?.ResponseObject, responseModel?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);
        }

        public async Task<ServiceResponse<T>> GetAccessToken<T>(RequestModel<Dictionary<string, StringValues>> request)
        {
            ServiceResult serviceResult = new ServiceResult();
            var response = await payment.GetAccessToken<T>(request);
            return response;
           
        }
    }
}
