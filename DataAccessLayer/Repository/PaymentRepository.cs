using DataAccessLayer.Model;
using DataAccessLayer.Helper;
using BussinessLogic.Abstractions;
using Domain;
using Microsoft.Extensions.Primitives;
using Domain.Response;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Collections.Generic;

using System.Net.Http.Json;
using Domain.Responses;

namespace DataAccessLayer.Repository;

public class PaymentRepository : IPayment
{
    DALConfigurations? config;
    public PaymentRepository(DALConfigurations _config)
    {
        config = _config;
    }
    async Task<ServiceResponse<object?>> IPayment.InsertPayment(Domain.Response.RequestModel<Domain.Response.PaymentModel> request)
    {
        Model.ResponseModel<bool> responseModel = new Model.ResponseModel<bool>();
        var spResponse = await new DapperHelper(config?._connectionString!).ExecuteSPAsync("Usp_InsertPaymentTransactions"
                , new { TbPaymentHeaderType = new DBHelper().ToDataTable(request!.RequestObject!.paymentHeaders), TbPaymentAdditionalInfoType = new DBHelper().ToDataTable(request.RequestObject.paymentAdditionalInfos), TbPaymentHistoryType = new DBHelper().ToDataTable(request.RequestObject.paymentHistories) });
        if (spResponse is not null)
        {
            responseModel.Result = true;
            return await new ServiceResult().GetServiceResponseAsync<object>(responseModel, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }
        else
        {
            responseModel.Result = false;
            responseModel.ErrorMessage = ApplicationGenericConstants.FAILD_TO_LOAD;
            return await new ServiceResult().GetServiceResponseAsync<object>(responseModel, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }

    }

    async Task<ServiceResponse<object?>> IPayment.UpdatePaymentHeader(Domain.Response.RequestModel<Domain.Response.UpdatePaymentModel> request)
    {
        Domain.Responses.ResponseModel<bool> responseModel = new Domain.Responses.ResponseModel<bool>();
        var ProfilesDt = await new DapperHelper(config!._connectionString!).ExecuteSPAsync("Usp_UpdatePaymentHeader"
               , new { TransactionID = request!.RequestObject!.transactionID, ResultCode = request.RequestObject.ResultCode, ResponseMessage = request.RequestObject.ResponseMessage, IsActive = request.RequestObject.isActive, TransactionType = request.RequestObject.transactionType, Amount = request.RequestObject.amount, ReservationNumber = request.RequestObject.ReservationNumber });

        if (ProfilesDt != null && ProfilesDt.Any())
        {
            responseModel.Result = true;
            return await new ServiceResult().GetServiceResponseAsync<object>(responseModel, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }
        else
        {
            responseModel.Result = false;
            responseModel.ErrorMessage = ApplicationGenericConstants.FAILD_TO_LOAD;
            return await new ServiceResult().GetServiceResponseAsync<object>(responseModel, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }

    }

    async Task<ServiceResponse<IEnumerable<Domain.Responses.FetchPaymentTransaction?>?>?> IPayment.FetchPaymentDetails(Domain.Response.RequestModel<PaymentFetchRequest> request)
    {
        Domain.Responses.ResponseModel<IEnumerable<Domain.Responses.FetchPaymentTransaction>> responseModel = new Domain.Responses.ResponseModel<IEnumerable<Domain.Responses.FetchPaymentTransaction>>();
        var spResponse = await new DapperHelper(config?._connectionString!).ExecuteSPAsync<Model.FetchPaymentTransaction>("Usp_FetchPaymentTransaction", new { ReservationNameID = request.RequestObject?.ReservationNumber });
        if (spResponse != null && spResponse.Any())
        {

            responseModel.Result = true;

            var payrespone = JsonSerializer.Serialize(spResponse);
            IEnumerable<Domain.Responses.FetchPaymentTransaction>? des = JsonSerializer.Deserialize<IEnumerable<Domain.Responses.FetchPaymentTransaction>>(payrespone);
            return await new ServiceResult().GetServiceResponseAsync<IEnumerable<Domain.Responses.FetchPaymentTransaction?>>(des, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }

        else
        {

            responseModel.Result = false;
            responseModel.ErrorMessage = ApplicationGenericConstants.FAILD_TO_LOAD;

            return await new ServiceResult().GetServiceResponseAsync<IEnumerable<Domain.Responses.FetchPaymentTransaction?>>(null, responseModel.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);
        }
    }

  async Task<ServiceResponse<string>> IPayment.getOperaPaymentType(string AdeyanPaymentType)
    {
        var OperaPaymentType = await new DapperHelper(config!._connectionString!).getOperaPaymentType(AdeyanPaymentType);
        return await new ServiceResult().GetServiceResponseAsync<string>(OperaPaymentType, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);

    }

   


    /*async Task<ServiceResponse<Domain.Responses.PaymentResponse>> IPayment.CapturePayment(Domain.Response.RequestModel<Domain.Response.PaymentRequest> request)
    {
        Domain.Responses.PaymentResponse? paymentResponseObject = new Domain.Responses.PaymentResponse();



            Adyen.Model.Modification.ModificationResult modificationResult = JsonSerializer.Deserialize<Adyen.Model.Modification.ModificationResult>(await response.Content.ReadAsStringAsync());

            if (modificationResult != null && modificationResult.Response == Adyen.Model.Enum.ResponseEnum.CaptureReceived)
            {
                paymentResponseObject.PspReference = modificationResult.PspReference;
                List<Domain.Responses.AdditionalInfo> additionalInfos = new List<Domain.Responses.AdditionalInfo>();
                if (modificationResult.AdditionalData != null)
                {
                    foreach (KeyValuePair<string, string> keyValuePair in modificationResult.AdditionalData)
                    {
                        Domain.Responses.AdditionalInfo additionalInfo = new Domain.Responses.AdditionalInfo();
                        additionalInfo.key = keyValuePair.Key;
                        additionalInfo.value = keyValuePair.Value;
                        switch (additionalInfo.key)
                        {
                            case "refusalReasonRaw":
                                paymentResponseObject.RefusalReason = additionalInfo.value;
                                break;
                            case "expiryDate":
                                paymentResponseObject.CardExpiryDate = additionalInfo.value;
                                break;
                            case "recurring.recurringDetailReference":
                                paymentResponseObject.PaymentToken = additionalInfo.value;
                                break;
                            case "authCode":
                                paymentResponseObject.AuthCode = additionalInfo.value;
                                break;
                            case "paymentMethod":
                                paymentResponseObject.CardType = additionalInfo.value;
                                break;
                            case "fundingSource":
                                paymentResponseObject.FundingSource = additionalInfo.value;
                                break;
                            case "authorisedAmountCurrency":
                                paymentResponseObject.Currency = additionalInfo.value;
                                break;
                            case "authorisedAmountValue":
                                paymentResponseObject.Amount = !string.IsNullOrEmpty(additionalInfo.value) ? Decimal.Divide(Convert.ToDecimal(long.Parse(additionalInfo.value)), Convert.ToDecimal(100)) : 0;
                                break;


                        }
                        additionalInfos.Add(additionalInfo);
                    }
                    paymentResponseObject.additionalInfos = additionalInfos;
                }
            }
            return await new ServiceResult().GetServiceResponseAsync(paymentResponseObject, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
        }*/

}


