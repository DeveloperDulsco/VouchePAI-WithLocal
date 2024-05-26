namespace Middlewares;

using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;


public static  class LoggingMiddleware
{
    public static void LogRequestResponse(this WebApplication app)
    {

        
        app.Use(async (context, next) =>
        {
          
            
            context.Request.EnableBuffering();
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;
            var logger= context.RequestServices.GetService<ILogger<HttpContext>>();
            Console.WriteLine($"Request: {requestBody}");
            logger.LogInformation("Request", $"Request: {requestBody}");
            await next.Invoke();
        });

    }

}
