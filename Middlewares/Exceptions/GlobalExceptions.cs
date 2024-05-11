using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;


namespace Middlewares.Exceptions
{
    public class GlobalExceptions : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            await httpContext.Response.WriteAsJsonAsync(exception.Message);
            return true;
        }
    }
}
