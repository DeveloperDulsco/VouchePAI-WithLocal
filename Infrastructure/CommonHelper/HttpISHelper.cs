
using System.Net;
using System.Text;
using System.Text.Json;

namespace Infrastructure.CommonHelper
{
    public class HttpISHelper
    {
        public async Task<HttpResponseMessage> ExecutePostAsync(string web_url, object? body, Dictionary<string, string>? headers, string? accesToken, Dictionary<string, string>? formDataBody, bool isProxyEnabled, string? proxyURL, string? proxyUN, string? proxyPSWD)
        {

            HttpClient? httpClient = null;
            if (isProxyEnabled)
            {
                httpClient = new HttpISHelper().getProxyClient(proxyURL, proxyUN, proxyPSWD);
            }
            else
                httpClient = new HttpClient();


            httpClient.BaseAddress = new Uri(web_url + (web_url.EndsWith("/") ? "" : "/"));

            httpClient.DefaultRequestHeaders.Clear();

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }



            if (!string.IsNullOrEmpty(accesToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accesToken);
            }
            HttpResponseMessage response = new HttpResponseMessage();
            if (body != null)
            {

                string jsondata = JsonSerializer.Serialize(body);
                StringContent content = new StringContent(jsondata, Encoding.UTF8, "application/json");
                content.Headers.ContentType.CharSet = null;
                response = await httpClient.PostAsync(web_url, content);
            }
            else
            {
                if (formDataBody != null && formDataBody.Count > 0)
                {
                    var req = new HttpRequestMessage(HttpMethod.Post, web_url);
                    req.Content = new FormUrlEncodedContent(formDataBody);
                    response = await httpClient.SendAsync(req);
                }
                else
                    response = await httpClient.PostAsync(web_url, null);
            }


            return response;


        }

        public HttpClient getProxyClient(string ProxyHost, string? proxyUserName, string? proxyPassword)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.UseDefaultCredentials = true;
            var proxy = new WebProxy
            {
                Address = new Uri(ProxyHost),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,

                Credentials = new NetworkCredential(
                userName: proxyUserName,
                password: proxyPassword)
            };

            var httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
            };
            return new HttpClient(handler: httpClientHandler, disposeHandler: true);
        }

    }
}
