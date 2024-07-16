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
    private readonly BLConfigutations bLConfigutations;
    public PaymentBL(IPayment _ipayment, ITokenRequest _itokenRequest, IAdenPayment _iadenPayment, IOperaService _iOperaService, IPaymentValidation _paymentValidation, BLConfigutations _bLConfigutations)
    {
        payment = _ipayment;
        tokenRequest = _itokenRequest;
        adenPayment = _iadenPayment;
        operaService = _iOperaService;
        paymentValidation = _paymentValidation;
        bLConfigutations = _bLConfigutations;


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
        if (respose?.ApiResponseCode == ApiResponseCodes.FAILURE) return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.INSERTFAILURE, ApiResponseCodes.FAILURE,400, null);
        var paymenttypecode = await payment.getOperaPaymentType(request?.RequestObject?.paymentHeaders.FirstOrDefault().CardType);
        if (paymenttypecode == null)
            return await serviceResult.GetServiceResponseAsync<object?>(null, "PAYMENTTYPE CODE MISSING", ApiResponseCodes.FAILURE,400, null);
        if (request?.RequestObject?.paymentHeaders.FirstOrDefault().TransactionType == "Sale")
        {
            var creditcardresponse = await operaService.ModifyReservation(new OwsRequestModel()
            {
                modifyBookingRequest = new ModifyBookingRequest()
                {
                    ReservationNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().ReservationNumber,
                    isUDFFieldSpecified = false,
                    updateCreditCardDetails = true,
                    GarunteeTypeCode = null,//"CC",
                    PaymentMethod = new PaymentMethod()
                    {

                        ExpiryDate = !string.IsNullOrEmpty(request?.RequestObject?.paymentHeaders.FirstOrDefault().ExpiryDate) ? "01/" + request?.RequestObject?.paymentHeaders.FirstOrDefault().ExpiryDate : null,
                        MaskedCardNumber = request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber,
                        PaymentType = paymenttypecode.ResponseData,
                        AprovalCode = !string.IsNullOrEmpty(request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode) ? request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode : request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                    }
                }

            });
            if (creditcardresponse?.ApiResponseCode == ApiResponseCodes.FAILURE)
                return await serviceResult.GetServiceResponseAsync<object>(null, creditcardresponse.Message, creditcardresponse.ApiResponseCode,400,null);
            if (bLConfigutations.settings.IsUDFUpdate)
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
                    return await serviceResult.GetServiceResponseAsync<object>(null, udfresponse.Message, udfresponse.ApiResponseCode,400, null);
            }
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
                    PaymentRefernce = "Saavy - (" + request?.RequestObject?.paymentHeaders.FirstOrDefault().MaskedCardNumber + ")",
                    ApprovalCode = request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode,
                    PaymentTypeCode  = bLConfigutations.settings.TestCard ? paymenttypecode.ResponseData.Split('-')[0]: paymenttypecode.ResponseData,

                }
            });

            if (paymnetresponse?.ApiResponseCode == ApiResponseCodes.FAILURE)

                return await serviceResult.GetServiceResponseAsync<object>(paymnetresponse.ResponseData, paymnetresponse.Message, paymnetresponse.ApiResponseCode,400, null);
            else
                return await serviceResult.GetServiceResponseAsync<object>(paymnetresponse.ResponseData, paymnetresponse.Message, paymnetresponse.ApiResponseCode, paymnetresponse.StatusCode, null);

        }
        else if(request?.RequestObject?.paymentHeaders.FirstOrDefault().TransactionType == "PreAuth")
        {
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
                        PaymentType = paymenttypecode.ResponseData,
                        AprovalCode = !string.IsNullOrEmpty(request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode) ? request?.RequestObject?.paymentHeaders.FirstOrDefault().ApprovalCode : request?.RequestObject?.paymentHeaders.FirstOrDefault().pspReferenceNumber
                    }
                }
            });
            if (creditcardresponse?.ApiResponseCode == ApiResponseCodes.FAILURE)
                return await serviceResult.GetServiceResponseAsync<object>(null, creditcardresponse.Message, creditcardresponse.ApiResponseCode,400, null);
            if (bLConfigutations.settings.IsUDFUpdate)
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

                    return await serviceResult.GetServiceResponseAsync<object>(udfresponse.ResponseData, udfresponse.Message, udfresponse.ApiResponseCode, 400, null);
                else
                    return await serviceResult.GetServiceResponseAsync<object>(udfresponse.ResponseData, udfresponse.Message, udfresponse.ApiResponseCode, udfresponse.StatusCode, null);
            }

            return await serviceResult.GetServiceResponseAsync<object>(creditcardresponse.ResponseData, creditcardresponse?.Message, creditcardresponse.ApiResponseCode, 400, null);
        }

        else
        {

            return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
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
            RequestModel<PaymentFetchRequest> request1 = new RequestModel<PaymentFetchRequest>{ ConnectionString = request?.ConnectionString, RequestObject = new PaymentFetchRequest { ReservationNumber = request?.RequestObject.ReservationNumber } };
            var fetchpayment = await payment.FetchPaymentDetails(request1);
            if (respose?.ResponseData is not null)
            {
                if(fetchpayment?.ResponseData is  null)
                    return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null, "NOT ACTIVE TRANSACTION EXISTS",ApiResponseCodes.FAILURE, 400, null);
                var paymentdata = fetchpayment?.ResponseData.Where(x => x.TransactionID == request?.RequestObject.TransactionId);
                if (paymentdata is  null || paymentdata.Count()==0 )
                    return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null, "NOT ACTIVE TRANSACTION EXISTS", ApiResponseCodes.FAILURE, 400, null);

                var paymenttypecode = await payment.getOperaPaymentType(paymentdata?.FirstOrDefault().CardType);
                if(paymenttypecode == null)
                    return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null, "PAYMENTTYPE CODE MISSING", ApiResponseCodes.FAILURE, 400, null);

                if (paymenttypecode.ResponseData is null)
                    return await serviceResult.GetServiceResponseAsync<PaymentResponse?>(null, "PAYMENTTYPE CODE MISSING", ApiResponseCodes.FAILURE, 400, null);
                var paymnetresponse = await operaService.MakePayment(new OwsRequestModel()  //Update the Paymnet in Opera
                {
                    MakePaymentRequest = new MakePaymentRequest()
                    {
                        Amount = Convert.ToDecimal(request?.RequestObject?.Amount),
                        PaymentInfo = "Auth code - (" + paymentdata?.FirstOrDefault().AuthorisationCode + ")",
                        StationID = OperaConstants.StationIDCheckOut,
                        WindowNumber = 1,
                        ReservationNameID = paymentdata?.FirstOrDefault().ReservationNameID,
                        MaskedCardNumber = paymentdata?.FirstOrDefault().MaskedCardNumber,
                        PaymentRefernce = "saavy - (" + paymentdata?.FirstOrDefault().MaskedCardNumber + ")",
                        PaymentTypeCode = bLConfigutations.settings.TestCard?paymenttypecode.ResponseData.Split('-')[0]: paymenttypecode.ResponseData, //"MC", //  need to take from Database.
                        ApprovalCode = paymentdata?.FirstOrDefault().ResultCode
                    }
                    

                });
                if (paymnetresponse?.ApiResponseCode == ApiResponseCodes.FAILURE)

                    return await serviceResult.GetServiceResponseAsync(respose?.ResponseData, paymnetresponse.Message, paymnetresponse.ApiResponseCode,400, null);

                //Update in Savy Pay Database .
                var updateheader = await payment.UpdatePaymentHeader(new RequestModel<UpdatePaymentModel> { RequestObject = new UpdatePaymentModel { isActive = false, amount = paymentdata.FirstOrDefault().Amount, ReservationNumber = request.RequestObject.ReservationNumber, ResponseMessage = paymentdata?.FirstOrDefault().ResultCode, ResultCode = paymentdata?.FirstOrDefault().ResultCode, transactionID = paymentRequest.TransactionId } });
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

