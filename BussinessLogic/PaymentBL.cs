using BussinessLogic.Abstractions;
using Domain;
using Domain.Response;
using Domain.Responses;
using Microsoft.Extensions.Primitives;
using System.Net.Http.Json;




namespace BussinessLogic;

public class PaymentBL
{
     private readonly IPayment payment;
     private readonly ITokenRequest tokenRequest;
     private readonly IAdenPayment adenPayment;
     private readonly IOperaService operaService;
     private readonly IPaymentValidation paymentValidation;
    public PaymentBL(IPayment _ipayment, ITokenRequest _itokenRequest, IAdenPayment _iadenPayment,IOperaService _iOperaService,IPaymentValidation _paymentValidation)
    {
        payment = _ipayment;
        tokenRequest = _itokenRequest;
        adenPayment = _iadenPayment;
        operaService = _iOperaService;
        paymentValidation = _paymentValidation;

    }

    public async Task<ServiceResponse<object?>> InsertPayment(RequestModel<PaymentModel> request) //Insert Payment in Vasy Pay database and update in Opera.
    {
        ServiceResult serviceResult = new ServiceResult();
        
        #region Validation
        var result = await  paymentValidation.ValidateInsertPayment(request);
        
        if ( result is null || !result.IsValid )
            return await serviceResult.GetServiceResponseAsync<object?>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);
        
        
        
        #endregion

        
        PaymentModel? paymentDetails = request?.RequestObject;
        var respose = await payment.InsertPayment(request!);

        if (respose is not null)
        {
            if (request?.RequestObject?.transaction == TransactionType.Sale)
            {
                var paymnetresponse = await operaService.MakePayment(new OwsRequestModel()
                {
                    MakePaymentRequest = new MakePaymentRequest()
                    {
                        Amount = Convert.ToDecimal(request?.RequestObject?.paymentHeaders.FirstOrDefault().Amount),
                        PaymentInfo = "Auth code - (" + request?.RequestObject?.paymentHeaders.FirstOrDefault().AuthorisationCode + ")",
                        StationID = OperaConstants.StationIDCheckIn, // This should be hardcoded .
                        WindowNumber = 1,
                        ReservationNameID = request?.RequestObject?.paymentHeaders.FirstOrDefault().ReservationNameID,
                        MaskedCardNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber,
                        PaymentRefernce = "web checkin - (" + request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber + ")",
                        PaymentTypeCode = "WEB", //  need to take from Database.
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
                           FieldName  = OperaConstants.PreAuthUDFFieldName,
                           FieldValue = request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                           },
                           new UDFField()
                            {
                            FieldName  = OperaConstants.PreAuthAmntUDFieldName,
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
                           FieldName  = OperaConstants.PreAuthUDFFieldName,
                           FieldValue = request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                           },
                           new UDFField()
                            {
                            FieldName  = OperaConstants.PreAuthAmntUDFieldName,
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
    public async Task<ServiceResponse<object?>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request)  //Update Paymen Header only in database .
    {
        ServiceResult serviceResult = new ServiceResult();

        #region Validation
        var result = await paymentValidation.ValidateUpdatePayment(request);
       if ( result is null || !result.IsValid )
            return await serviceResult.GetServiceResponseAsync<object?>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);
        #endregion

        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<object?>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        UpdatePaymentModel? updatePayment = request?.RequestObject;
        var respose = await payment.UpdatePaymentHeader(request!);

        if (respose is not null)
            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);

        else
            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
    }
    public async Task<ServiceResponse<IEnumerable<FetchPaymentTransaction?>?>> FetchPaymentDetails(RequestModel<PaymentFetchRequest> request)
    {

        ServiceResult serviceResult = new ServiceResult();

        #region Validation
        var result = await paymentValidation.ValidateFetchPayment(request);
       if ( result is null || !result.IsValid )
            return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction?>>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);
        #endregion

        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction?>>(null, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, null);

        var respose = await payment.FetchPaymentDetails(request!);
        if (respose is not null)
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
        else
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);



    }


    public async Task<ServiceResponse<PaymentResponse?>?> CapturePayment(RequestModel<PaymentRequest> request)  //Update Opera
    {
        ServiceResult serviceResult = new ServiceResult();
        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        #region Validation
        var result = await paymentValidation.ValidateCapturePayment(request);
        if ( result is null || !result.IsValid )
            return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);

        #endregion

        PaymentRequest? paymentRequest = request?.RequestObject;
        var respose = await adenPayment.CapturePayment(paymentRequest);  //Capture the Payment in Adeyan.
        if (respose is not null)
        {
            if(respose?.ResponseData is not null)
            {
                var paymnetresponse = await operaService.MakePayment(new OwsRequestModel()  //Update the Paymnet in Opera
                {
                    MakePaymentRequest = new MakePaymentRequest()
                    {
                        Amount = Convert.ToDecimal(respose?.ResponseData?.Amount),
                        PaymentInfo = "Auth code - (" + respose?.ResponseData?.AuthCode + ")",
                        StationID = OperaConstants.StationIDCheckOut,
                        WindowNumber = 1,
                        ReservationNameID = paymentRequest?.ReservationNameID,
                        MaskedCardNumber = respose?.ResponseData?.MaskCardNumber,
                        PaymentRefernce = "web checkin - (" + respose?.ResponseData?.MaskCardNumber + ")",
                        PaymentTypeCode =  "MC" , //"MC", //  need to take from Database.
                        ApprovalCode = respose?.ResponseData?.ResultCode
                    }
                });
                //Update in Savy Pay Database .
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

