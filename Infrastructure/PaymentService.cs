using Infrastructure.CommonHelper;

using System.Net.Http.Headers;

using System.Text;
using Microsoft.Extensions.Primitives;
using BussinessLogic;
using Domain;
using Domain.Response;
using Domain.Responses;
using System.Text.Json;



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
        if (paymentRequest?.RequestObject.Amount != null)
            amnt = (long)(paymentRequest.RequestObject.Amount * 100);
        Adyen.Model.Modification.CaptureRequest captureRequest = new Adyen.Model.Modification.CaptureRequest()
        {
            MerchantAccount = paymentRequest?.merchantAccount,
            OriginalReference = paymentRequest?.RequestObject.OrginalPSPRefernce,
            ModificationAmount = new Adyen.Model.Amount(config.PaymentSettings.AdyenPaymentCurrency, amnt)
        };

        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Add("x-api-key", config.PaymentSettings.ApiKey);

        HttpContent requestContent = new StringContent(JsonSerializer.Serialize(captureRequest), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(config.PaymentSettings.AdyenPaymentURL, requestContent);
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

            var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(JsonSerializer.Serialize(responseContent));

            return await new ServiceResult().GetServiceResponseAsync<PaymentResponse>(paymentResponse, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, (int)response.StatusCode, null);
        }
        else
            return await new ServiceResult().GetServiceResponseAsync<PaymentResponse>(null, ApplicationGenericConstants.FAILURE, ApiResponseCodes.FAILURE, (int)response.StatusCode, null);





    }



}
