using Infrastructure.CommonHelper;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Adyen;
using DataAccessLayer.Model;

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


    }
}
