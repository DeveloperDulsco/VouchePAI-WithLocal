using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BussinessLogic;
using Infrastructure.CommonHelper;
using Microsoft.Extensions.Primitives;

namespace Infrastructure;

public class TokenService : ITokenRequest
{
    InfraConfigutations? config;
    public TokenService(InfraConfigutations _config)
    {
        config = _config;
    }
    public async Task<dynamic> GetAccessToken(Dictionary<string, StringValues> token)
    {
        var requestUrl = config?.PaymentSettings?.AccessTokenURL;

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

        return response;

    }
}


