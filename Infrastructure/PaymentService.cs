using Infrastructure.CommonHelper;

using System.Net.Http.Headers;

using System.Text;

using BussinessLogic;
using Domain;
using Domain.Response;
using Domain.Responses;
using System.Text.Json;
using Microsoft.IdentityModel.Logging;





namespace Infrastructure;

public class PaymentService : IAdenPayment
{

    InfraConfigutations? config;
    public PaymentService(InfraConfigutations _config)
    {
        config = _config;
    }

    public async Task<ServiceResponse<PaymentResponse?>> CapturePayment(PaymentRequest? paymentRequest)
    {
        HttpClient? httpClient = null;
        bool proxy = false;
        if (proxy)
        {
            httpClient = new HttpISHelper().getProxyClient("", null, null);
        }
        else
            httpClient = new HttpClient();
        long amnt = 0;
        if (paymentRequest?.Amount != null)
            amnt = (long)(paymentRequest.Amount * 100);
        Adyen.Model.Modification.CaptureRequest captureRequest = new Adyen.Model.Modification.CaptureRequest()
        {
            MerchantAccount = config?.PaymentSettings!.MerchantAccount,
            OriginalReference = paymentRequest?.OrginalPSPRefernce,
            ModificationAmount = new Adyen.Model.Amount(config?.PaymentSettings!.AdyenPaymentCurrency, amnt)
        };
        string s = Newtonsoft.Json.JsonConvert.SerializeObject(captureRequest);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("x-api-key", config?.PaymentSettings!.ApiKey);

        HttpContent requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(captureRequest), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(config?.PaymentSettings!.AdyenPaymentURL, requestContent);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
             PaymentResponse paymentResponseObject = new PaymentResponse();
            Adyen.Model.Modification.ModificationResult modificationResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Adyen.Model.Modification.ModificationResult>(await response.Content.ReadAsStringAsync());
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

                return await new ServiceResult().GetServiceResponseAsync<PaymentResponse?>(paymentResponseObject, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, (int)response.StatusCode, null);
            }
            else
            {
                return await new ServiceResult().GetServiceResponseAsync<PaymentResponse?>(paymentResponseObject, ApplicationGenericConstants.ERR_DATA_CAPTURE, ApiResponseCodes.SUCCESS, (int)response.StatusCode, null);
            }
        }
        else
            return await new ServiceResult().GetServiceResponseAsync<PaymentResponse?>(null, ApplicationGenericConstants.ERR_DATA_CAPTURE, ApiResponseCodes.FAILURE, (int)response.StatusCode, null);





    }



}
