using Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;


namespace Middlewares.Exceptions
{
    public class GlobalExceptions : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            
            ServiceResult serviceResult=new ServiceResult();
            await httpContext.Response.WriteAsJsonAsync(await serviceResult.GetServiceResponseAsync<string>(exception.Message,ApplicationGenericConstants.UNKNOWN_ERROR,ApiResponseCodes.FAILURE, 500,null));
            return true;
        }
    }
}
