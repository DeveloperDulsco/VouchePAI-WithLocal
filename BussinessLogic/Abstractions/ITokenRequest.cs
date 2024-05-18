using Domain;
using Domain.Response;
using Microsoft.Extensions.Primitives;

namespace BussinessLogic;


public interface ITokenRequest
{
    public Task<HttpResponseMessage> GetAccessToken(Dictionary<string, StringValues> token);

}


