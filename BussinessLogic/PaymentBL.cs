using BussinessLogic.Abstractions;
using DataAccessLayer.Helper;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using Domain;
using Infrastructure;
using Infrastructure.CommonHelper;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;


namespace BussinessLogic
{
    public class PaymentBL: IPayment
    {

        PaymentRepository _prepository = new PaymentRepository();
        PaymentService _pservice = new PaymentService();

        public async Task<ServiceResponse<object>> InsertPayment(RequestModel<PaymentModel> request)
        {
            ServiceResult serviceResult = new ServiceResult();

            if (request?.RequestObject is null) return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

            PaymentModel? paymentDetails = request?.RequestObject;
            var respose = await _prepository.InsertPaymentDetails(paymentDetails.paymentHistories, paymentDetails.paymentHeaders, paymentDetails.paymentAdditionalInfos);

            if (respose is not null && respose.Result)
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync<object>(null, respose?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);





        }
        public async Task<ServiceResponse<object>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request)
        {

            ServiceResult serviceResult = new ServiceResult();


            if (request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);

            UpdatePaymentModel? updatePayment = request?.RequestObject;
            var respose = await _prepository.UpdatePaymentHeaderData(updatePayment);

            if (respose is not null && respose.Result)
                return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync<object>(null, respose?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);




        }
        public async Task<ServiceResponse<IEnumerable<FetchPaymentTransaction>>> FetchPaymentDetails(RequestModel<string> request)
        {

            ServiceResult serviceResult = new ServiceResult();
            if (request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction>>(null, "Invalid Capture Request", ApiResponseCodes.FAILURE, 400, null);

            var respose = await _prepository.FetchPaymentActiveTransactions(request?.RequestObject);
            if (respose is not null && respose.Result)
                return await serviceResult.GetServiceResponseAsync(respose.ResponseObject, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync(respose?.ResponseObject, respose?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);



        }
        public async Task<ServiceResponse<PaymentResponse>> CapturePayment(RequestModel<PaymentRequest> request)
            {
            PaymentResponse paymentResponseObject = new PaymentResponse();
            ServiceResult serviceResult = new ServiceResult();
            if (request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<PaymentResponse>(null, "Invalid Capture Request", ApiResponseCodes.FAILURE, 400, null);

            ResponseModel<PaymentResponse> responseModel = new ResponseModel<PaymentResponse>();

            var response = await _pservice.PaymentCapture(request?.RequestObject);
            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    var responsestr = await response.Content.ReadAsStringAsync();
                    Adyen.Model.Modification.ModificationResult modificationResult = JsonConvert.DeserializeObject<Adyen.Model.Modification.ModificationResult>(await response.Content.ReadAsStringAsync());
                   
                    if (modificationResult != null && modificationResult.Response == Adyen.Model.Enum.ResponseEnum.CaptureReceived)
                    {
                        paymentResponseObject.PspReference = modificationResult.PspReference;
                        List<AdditionalInfo> additionalInfos = new List<AdditionalInfo>();
                        if (modificationResult.AdditionalData != null)
                        {
                            foreach (KeyValuePair<string, string> keyValuePair in modificationResult.AdditionalData)
                            {
                               AdditionalInfo additionalInfo = new AdditionalInfo();
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

                        responseModel.ResponseObject=paymentResponseObject;
                        responseModel.Result = true;
                       
                    }
                    else
                    {


                        responseModel.ErrorMessage = modificationResult?.Message;
                            responseModel.Result = false;
                        
                    }
                }
                else
                {
                    responseModel.Result = false;
                    responseModel.ErrorMessage = response.ReasonPhrase;
                   
                }

            }
            else
            {
                responseModel.Result = false;
                responseModel.ErrorMessage = "Payment gateway returned blank";
               
            }
            if (responseModel is not null && responseModel.Result)
                return await serviceResult.GetServiceResponseAsync(responseModel.ResponseObject, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync(responseModel?.ResponseObject, responseModel?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);
        }

        public async Task<ServiceResponse<object>> GetAccessToken(RequestModel<TokenRequest> request)
        {
            ServiceResult serviceResult = new ServiceResult();
            if(request?.RequestObject is null)
                return await serviceResult.GetServiceResponseAsync<object?>(null, "Invalid Token Request", ApiResponseCodes.FAILURE, 400, null);
            var response = await _pservice.GetAccessToken(request?.RequestObject);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response is not null && response.IsSuccessStatusCode)
               

            return await serviceResult.GetServiceResponseAsync<object>(responseContent, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
            else
                return await serviceResult.GetServiceResponseAsync<object>(responseContent, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, 400, null);
        }
    }
}
