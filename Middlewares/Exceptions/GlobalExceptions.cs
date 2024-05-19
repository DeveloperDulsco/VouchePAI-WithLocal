using Domain;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace Middlewares.Exceptions
{
    public class GlobalExceptions : IExceptionHandler
    {
        IWebHostEnvironment _environment;
        ILogger<GlobalExceptions> _logger { get; }
        public GlobalExceptions(IWebHostEnvironment environment, ILogger<GlobalExceptions> logger)
        {
            _environment = environment;
            _logger = logger;
           

        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            ServiceResult serviceResult = new ServiceResult();
            _logger.LogError(exception, exception.Message);
            if (_environment.IsDevelopment() || _environment.IsStaging())

                await httpContext.Response.WriteAsJsonAsync(await serviceResult.GetServiceResponseAsync<string>(exception.Message, ApplicationGenericConstants.UNKNOWN_ERROR, ApiResponseCodes.FAILURE, 500, null));
            else
                await httpContext.Response.WriteAsJsonAsync(await serviceResult.GetServiceResponseAsync<string>(ApplicationGenericConstants.UNKNOWN_ERROR, ApplicationGenericConstants.UNKNOWN_ERROR, ApiResponseCodes.FAILURE, 500, null));

            return true;



        }
    }
}
