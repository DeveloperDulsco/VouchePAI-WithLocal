using Infrastructure.CommonHelper;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Adyen;
using DataAccessLayer.Model;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Infrastructure
{
    public class PaymentService
    {


        public async Task<HttpResponseMessage> PaymentCapture(PaymentRequest paymentRequest)
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
            if (paymentRequest.RequestObject.Amount != null)
                amnt = (long)(paymentRequest.RequestObject.Amount * 100);
            Adyen.Model.Modification.CaptureRequest captureRequest = new Adyen.Model.Modification.CaptureRequest()
            {
                MerchantAccount = paymentRequest?.merchantAccount,
                OriginalReference = paymentRequest?.RequestObject.OrginalPSPRefernce,
                ModificationAmount = new Adyen.Model.Amount("SGD", amnt)
            };

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-api-key", paymentRequest?.ApiKey);

            HttpContent requestContent = new StringContent(JsonConvert.SerializeObject(captureRequest), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync("https://pal-test.adyen.com/pal/servlet/Payment/v52/capture", requestContent);
            return response;
        }
        public async Task<HttpResponseMessage> GetAccessToken(Dictionary<string, StringValues> token)
        {
            var requestUrl = "https://login.microsoftonline.com/a53a7a70-988d-4539-b456-708670a75463/oauth2/v2.0/token";

            HttpClient? httpClient = null;
            bool proxy = false;
            if (proxy)
            {
                httpClient = new HttpISHelper().getProxyClient("", null, null);
            }
            else
                httpClient = new HttpClient();

            var formEnCoder = new Dictionary<string, string>();

            foreach (var item in token)
                formEnCoder.Add(item.Key, item.Value[0]);

            var content = new FormUrlEncodedContent(formEnCoder);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await httpClient.PostAsync(requestUrl, content);
            if (response.IsSuccessStatusCode)
                return response;

        }
    }
}
