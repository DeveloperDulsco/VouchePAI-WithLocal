using Infrastructure.CommonHelper;


namespace Infrastructure
{
    public class PaymentService
    {
        public async Task<HttpResponseMessage> CapturePayment(IsRequestModel request)
        {
            HttpResponseMessage responseMessage = await new HttpISHelper().ExecutePostAsync(request.web_url,request.body,request.headers,request.accesToken,request.formDataBody,request.isProxyEnabled, request.proxyURL,request.proxyUN,request.proxyPSWD);
            return responseMessage;
        }
    }
}
