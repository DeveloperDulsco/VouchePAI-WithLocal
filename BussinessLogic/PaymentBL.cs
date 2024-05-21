using BussinessLogic.Abstractions;
using Domain;
using Domain.Response;
using Domain.Responses;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Json;
using System.Reflection;



namespace BussinessLogic;

public class PaymentBL
{
    IPayment payment;
    ITokenRequest tokenRequest;
    IAdenPayment adenPayment;
    IOperaService operaService;
    public PaymentBL(IPayment _ipayment, ITokenRequest _itokenRequest, IAdenPayment _iadenPayment,IOperaService _iOperaService)
    {
        payment = _ipayment;
        tokenRequest = _itokenRequest;
        adenPayment = _iadenPayment;
        operaService = _iOperaService;

    }

    public async Task<ServiceResponse<object?>> InsertPayment(RequestModel<PaymentModel> request)
    {
        ServiceResult serviceResult = new ServiceResult();

        if (request?.RequestObject is null) return await serviceResult.GetServiceResponseAsync<object?>(null!, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        PaymentModel? paymentDetails = request?.RequestObject;
        var respose = await payment.InsertPayment(request!);

        if (respose is not null)
        {
            if (request.RequestObject.transaction == TransactionType.Sale)
            {
                var paymnetresponse = await operaService.MakePayment(new OwsRequestModel()
                {
                    MakePaymentRequest = new MakePaymentRequest()
                    {
                        Amount = Convert.ToDecimal(request?.RequestObject?.paymentHeaders.FirstOrDefault().Amount),
                        PaymentInfo = "Auth code - (" + request?.RequestObject?.paymentHeaders.FirstOrDefault().AuthorisationCode + ")",
                        StationID = "MCI",
                        WindowNumber = 1,
                        ReservationNameID = request?.RequestObject?.paymentHeaders.FirstOrDefault().ReservationNameID,
                        MaskedCardNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber,
                        PaymentRefernce = "web checkin - (" + request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber + ")",
                        PaymentTypeCode = "WEB",
                        ApprovalCode = request?.RequestObject?.paymentHeaders.FirstOrDefault().ResultCode
                    }
                });
                var udfresponse = await operaService.ModifyReservation(new OwsRequestModel()
                {
                    modifyBookingRequest = new ModifyBookingRequest()
                    {
                        isUDFFieldSpecified = true,
                        ReservationNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().ReservationNumber,
                        uDFFields = new List<UDFField>()
                         { new UDFField()
                          {
                           FieldName  = "PreAuthUDF",
                           FieldValue = request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                           },
                           new UDFField()
                            {
                            FieldName  = "PreAuthAmntUDF",
                            FieldValue = request?.RequestObject?.paymentHeaders.FirstOrDefault().Amount.ToString()
                            }
                            }
                    }
                });
            }
            else
            {
                var udfresponse = await operaService.ModifyReservation(new OwsRequestModel()
                {
                    modifyBookingRequest = new ModifyBookingRequest()
                    {
                        isUDFFieldSpecified = true,
                        ReservationNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().ReservationNumber,
                        uDFFields = new List<UDFField>()
                         { new UDFField()
                          {
                           FieldName  = "PreAuthUDF",
                           FieldValue = request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                           },
                           new UDFField()
                            {
                            FieldName  = "PreAuthAmntUDF",
                            FieldValue = request?.RequestObject?.paymentHeaders.FirstOrDefault().Amount.ToString()
                            }
                            }
                    }
                });

            }

            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
        }
        else
        {
            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
        }





    }
    public async Task<ServiceResponse<object?>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request)
    {
        ServiceResult serviceResult = new ServiceResult();
        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<object?>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        UpdatePaymentModel? updatePayment = request?.RequestObject;
        var respose = await payment.UpdatePaymentHeader(request!);

        if (respose is not null)
            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);

        else
            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
    }
    public async Task<ServiceResponse<IEnumerable<FetchPaymentTransaction?>?>> FetchPaymentDetails(RequestModel<string> request)
    {

        ServiceResult serviceResult = new ServiceResult();
        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction?>>(null, "Invalid Payment Request", ApiResponseCodes.FAILURE, 400, null);

        var respose = await payment.FetchPaymentDetails(request);
        if (respose is not null)
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
        else
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);



    }


    public async Task<ServiceResponse<PaymentResponse?>?> CapturePayment(RequestModel<PaymentRequest> request)
    {
        ServiceResult serviceResult = new ServiceResult();
        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        PaymentRequest? paymentRequest = request?.RequestObject;
        var respose = await adenPayment.CapturePayment(paymentRequest);
        if (respose is not null)
        {
            if(respose?.ResponseData is not null)
            {
                var paymnetresponse = await operaService.MakePayment(new OwsRequestModel()
                {
                    MakePaymentRequest = new MakePaymentRequest()
                    {
                        Amount = Convert.ToDecimal(respose?.ResponseData?.Amount),
                        PaymentInfo = "Auth code - (" + respose?.ResponseData?.AuthCode + ")",
                        StationID = "MCO",
                        WindowNumber = 1,
                        ReservationNameID = paymentRequest?.ReservationNameID,
                        MaskedCardNumber = respose?.ResponseData?.MaskCardNumber,
                        PaymentRefernce = "web checkin - (" + respose?.ResponseData?.MaskCardNumber + ")",
                        PaymentTypeCode = "MC",
                        ApprovalCode = respose?.ResponseData?.ResultCode
                    }
                });
                var udfresponse = await operaService.ModifyReservation(new OwsRequestModel()
                {
                    modifyBookingRequest = new ModifyBookingRequest()
                    {
                        isUDFFieldSpecified = true,
                        ReservationNumber = paymentRequest?.ReservationNumber,
                        uDFFields = new List<UDFField>()
                         { new UDFField()
                          {
                           FieldName  = "PreAuthUDF",
                           FieldValue = respose?.ResponseData?.PspReference
                           },
                           new UDFField()
                            {
                            FieldName  = "PreAuthAmntUDF",
                            FieldValue = respose?.ResponseData?.Amount.ToString()
                            }
                            }
                    }
                });
                var updateheader = await payment.UpdatePaymentHeader(new RequestModel<UpdatePaymentModel> { RequestObject = new UpdatePaymentModel { isActive = false, amount = paymentRequest.RequestObject.Amount, ReservationNumber = request.RequestObject.ReservationNumber, ResponseMessage = respose?.ResponseData.ResultCode, ResultCode = respose?.ResponseData.ResultCode, transactionID = paymentRequest.TransactionId } });
            }
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
        }
        else
        {
            return await serviceResult.GetServiceResponseAsync<PaymentResponse>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
        }

    }

    public async Task<ServiceResponse<object?>> GetAccessToken(Dictionary<string, StringValues> request)
    {
        var response = await tokenRequest.GetAccessToken(request);
        ServiceResult serviceResult = new ServiceResult();
        if (response?.Content is not null)
        {
            var resp = await response.Content.ReadFromJsonAsync<object>();
            return await serviceResult.GetServiceResponseAsync(resp, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, (int)response.StatusCode, null);
        }
        else
            return await serviceResult.GetServiceResponseAsync<object?>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, (int)response.StatusCode, null);




    }

}
