namespace Middlewares;

using Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

public static class AuthenticationMiddleware
{
    public static void validateToken(this WebApplication app)
    {

        app.Use(async (context,next)=>{

                async Task returnResponse(int StatusCode, string message)
                {
                    context.Response.StatusCode=StatusCode;
                    if(StatusCode==401) await context.Response.WriteAsync( ApplicationGenericConstants.UNAUTHORIZED);
                    else {

                        ServiceResponse<string> serviceResponse =new ServiceResponse<string>{

                            StatusCode=StatusCode,
                            Message=ApplicationGenericConstants.UNKNOWN_ERROR,
                            ApiResponseCode=ApiResponseCodes.FAILURE,
                            ResponseData=message

                        };

                        await context.Response.WriteAsJsonAsync(serviceResponse);
                    }
                }

                var path=context.Request.Path.ToString().ToLower();
                    if(path.StartsWith("/ping",StringComparison.OrdinalIgnoreCase)) { 

                        await Task.Delay(1000);
                        await returnResponse(200,"pong");

                        }

                        else if(path.StartsWith("/swagger",StringComparison.OrdinalIgnoreCase))
                        {
                            if(app.Environment.IsDevelopment() || app.Environment.IsStaging()) await next.Invoke();
                            else await returnResponse(401,string.Empty);

                        }
                        else {
                            try
                            {
                                context.Request.Headers.TryGetValue("Authorization",out StringValues token);
                                if(string.IsNullOrEmpty(token)) await returnResponse(401,string.Empty);
                                else {
                                    bool isTokenValid=true; //Validate the token here 
                                    try
                                    {
                                        if(isTokenValid) await next.Invoke();
                                        else await returnResponse(401,string.Empty);
                                    }
                                    catch(Exception e)
                                    {
                                         if(app.Environment.IsDevelopment() || app.Environment.IsStaging()) await returnResponse(500,e.ToString());
                                         else
                                         await returnResponse(500,ApplicationGenericConstants.UNKNOWN_ERROR);
                                         
                                          
                                    }
                                }
                            }

                             catch(Exception e)
                                    {
                                         if(app.Environment.IsDevelopment() || app.Environment.IsStaging()) await returnResponse(401,e.ToString());
                                         else
                                          await returnResponse(401,string.Empty);
                                         
                                          
                                    }

                        }


        });



    }

}
