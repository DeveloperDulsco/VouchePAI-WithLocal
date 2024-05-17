using Infrastructure.CommonHelper;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Adyen;
using DataAccessLayer.Model;
using System.Net.Http;

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
        public async Task<HttpResponseMessage> GetAccessToken(TokenRequest token)
        {
            var requestUrl = $"https://login.microsoftonline.com/a53a7a70-988d-4539-b456-708670a75463/oauth2/v2.0/token";

            HttpClient? httpClient = null;
            bool proxy = false;
            if (proxy)
            {
                httpClient = new HttpISHelper().getProxyClient("", null, null);
            }
            else
                httpClient = new HttpClient();
           

            var content = new FormUrlEncodedContent(new Dictionary<string, string> {
              { "client_id", token.Client_Id },
              { "client_secret",token.Client_Secret},
              { "grant_type", token.Grant_Type },
              { "scope", token.Scope },
            });
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUrl))
            {
                Content = content
            };

            var response = await httpClient.SendAsync(httpRequestMessage);
            return response;
            
        }
    }
}
