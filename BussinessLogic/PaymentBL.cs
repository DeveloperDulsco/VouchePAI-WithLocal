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
    private readonly IPayment payment;
    private readonly ITokenRequest tokenRequest;
    private readonly IAdenPayment adenPayment;
    private readonly IOperaService operaService;
    private readonly IPaymentValidation paymentValidation;
    public PaymentBL(IPayment _ipayment, ITokenRequest _itokenRequest, IAdenPayment _iadenPayment, IOperaService _iOperaService, IPaymentValidation _paymentValidation)
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
        var result = await paymentValidation.ValidateInsertPayment(request);

        if (result is null || !result.IsValid)
            return await serviceResult.GetServiceResponseAsync<object?>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);



        #endregion


        PaymentModel? paymentDetails = request?.RequestObject;
        var respose = await payment.InsertPayment(request!);
        if (respose?.ApiResponseCode == ApiResponseCodes.FAILURE) return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);

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
                    ApprovalCode = request?.RequestObject?.paymentHeaders.FirstOrDefault().OperaPaymentTypeCode,
                    PaymentTypeCode= request?.RequestObject?.paymentHeaders.FirstOrDefault().OperaPaymentTypeCode

                }
            });

            if (paymnetresponse?.ApiResponseCode == ApiResponseCodes.FAILURE)
                return await serviceResult.GetServiceResponseAsync<object>(null, paymnetresponse.Message, paymnetresponse.ApiResponseCode, paymnetresponse.StatusCode, null);

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
          
            if (udfresponse?.ApiResponseCode == ApiResponseCodes.FAILURE)
                return await serviceResult.GetServiceResponseAsync<object>(null, paymnetresponse.Message, paymnetresponse.ApiResponseCode, paymnetresponse.StatusCode, null);
            var creditcardresponse = await operaService.ModifyReservation(new OwsRequestModel()
            {
                modifyBookingRequest = new ModifyBookingRequest()
                {
                    ReservationNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().ReservationNumber,
                    isUDFFieldSpecified = false,
                    updateCreditCardDetails = true,
                    GarunteeTypeCode = "CC",//"CC",
                    PaymentMethod = new PaymentMethod()
                    {
                      
                        ExpiryDate = !string.IsNullOrEmpty(request?.RequestObject?.paymentHeaders.FirstOrDefault().ExpiryDate) ? "01/" + request?.RequestObject?.paymentHeaders.FirstOrDefault().ExpiryDate : null,
                        MaskedCardNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber,
                        PaymentType = request?.RequestObject?.paymentHeaders.FirstOrDefault().OperaPaymentTypeCode,
                        AprovalCode= !string.IsNullOrEmpty(request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode)? request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode: request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                    }
                }

                });
            return await serviceResult.GetServiceResponseAsync<object>(creditcardresponse.ResponseData, creditcardresponse.Message, creditcardresponse.ApiResponseCode, creditcardresponse.StatusCode, null);



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

            if (udfresponse?.ApiResponseCode == ApiResponseCodes.FAILURE)
                return await serviceResult.GetServiceResponseAsync<object>(null, udfresponse.Message, udfresponse.ApiResponseCode, udfresponse.StatusCode, null);
            var creditcardresponse = await operaService.ModifyReservation(new OwsRequestModel()
            {
                modifyBookingRequest = new ModifyBookingRequest()
                {
                    ReservationNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().ReservationNumber,
                    isUDFFieldSpecified = false,
                    updateCreditCardDetails = true,
                    GarunteeTypeCode = "CC",//"CC",
                    PaymentMethod = new PaymentMethod()
                    {
                        ExpiryDate = !string.IsNullOrEmpty(request?.RequestObject?.paymentHeaders.FirstOrDefault().ExpiryDate) ? "01/" + request?.RequestObject?.paymentHeaders.FirstOrDefault().ExpiryDate: null,
                        MaskedCardNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber,
                        PaymentType = request?.RequestObject?.paymentHeaders.FirstOrDefault().OperaPaymentTypeCode,
                         AprovalCode = !string.IsNullOrEmpty(request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode) ? request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode : request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                    }
                }
                });
            return await serviceResult.GetServiceResponseAsync<object>(creditcardresponse.ResponseData, creditcardresponse.Message, creditcardresponse.ApiResponseCode, creditcardresponse.StatusCode, null);
        }








    }
    public async Task<ServiceResponse<object?>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request)  //Update Paymen Header only in database .
    {
        ServiceResult serviceResult = new ServiceResult();

        #region Validation
        var result = await paymentValidation.ValidateUpdatePayment(request);
        if (result is null || !result.IsValid)
            return await serviceResult.GetServiceResponseAsync<object?>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);
        #endregion

        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<object?>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        UpdatePaymentModel? updatePayment = request?.RequestObject;
        var respose = await payment.UpdatePaymentHeader(request!);

        if (respose?.ApiResponseCode == ApiResponseCodes.SUCCESS)
            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);

        else
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, respose.Message, ApiResponseCodes.FAILURE, 400, null);
    }
    public async Task<ServiceResponse<IEnumerable<FetchPaymentTransaction?>?>> FetchPaymentDetails(RequestModel<PaymentFetchRequest> request)
    {

        ServiceResult serviceResult = new ServiceResult();

        #region Validation
        var result = await paymentValidation.ValidateFetchPayment(request);
        if (result is null || !result.IsValid)
            return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction?>>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);
        #endregion

        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction?>>(null, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, null);

        var respose = await payment.FetchPaymentDetails(request!);
        if (respose?.ApiResponseCode == ApiResponseCodes.SUCCESS)
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
        else
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, respose.Message, ApiResponseCodes.FAILURE, 400, null);



    }


    public async Task<ServiceResponse<PaymentResponse?>?> CapturePayment(RequestModel<PaymentRequest> request)  //Update Opera
    {
        ServiceResult serviceResult = new ServiceResult();
        if (request?.RequestObject is null)
            return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

        #region Validation
        var result = await paymentValidation.ValidateCapturePayment(request);
        if (result is null || !result.IsValid)
            return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null!, ApplicationGenericConstants.PAYMENT_VALIDATION, ApiResponseCodes.FAILURE, 400, result.Errors);

        #endregion

        PaymentRequest? paymentRequest = request?.RequestObject;
        var respose = await adenPayment.CapturePayment(paymentRequest);  //Capture the Payment in Adeyan.
        if (respose?.ApiResponseCode == ApiResponseCodes.SUCCESS)
        {
            if (respose?.ResponseData is not null)
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
                        PaymentTypeCode = "MC", //"MC", //  need to take from Database.
                        ApprovalCode = respose?.ResponseData?.ResultCode
                    }
                });
                //Update in Savy Pay Database .
                var updateheader = await payment.UpdatePaymentHeader(new RequestModel<UpdatePaymentModel> { RequestObject = new UpdatePaymentModel { isActive = false, amount = paymentRequest.RequestObject.Amount, ReservationNumber = request.RequestObject.ReservationNumber, ResponseMessage = respose?.ResponseData.ResultCode, ResultCode = respose?.ResponseData.ResultCode, transactionID = paymentRequest.TransactionId } });
                return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            }
            return await serviceResult.GetServiceResponseAsync<PaymentResponse>(null, ApplicationGenericConstants.PAYMENT_PMS_ERROR, ApiResponseCodes.FAILURE, 400, null);
        }
        else
        {
            return await serviceResult.GetServiceResponseAsync<PaymentResponse>(null, ApplicationGenericConstants.ERR_DATA_CAPTURE, ApiResponseCodes.FAILURE, 400, null);

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

