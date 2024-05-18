using BussinessLogic;
using Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Middlewares;
namespace WebAPI.Identity;

internal class IdentityApiEndPoint : IEndPointDefinition
{
    public void RegisterEndPoints(WebApplication application)
    {
        var IdentityGroup = application.MapGroup("identity/oauth2/v2.0/token");
        IdentityAPI(IdentityGroup);

    }
    private static RouteGroupBuilder IdentityAPI(RouteGroupBuilder IdentityGroup)
    {

        IdentityGroup.MapPost("/", GenerateTokenAsync).Produces<Ok>().Produces<UnauthorizedHttpResult>().Produces<BadRequest>();
        return IdentityGroup;
    }

    [AllowAnonymous]
    [Consumes("application/x-www-form-urlencoded")]

    private static async Task<IResult?> GenerateTokenAsync(HttpRequest httpRequest)
    {
        var vals = (await httpRequest.ReadFormAsync()).ToDictionary(); ;
        var serviceResponse = await new PaymentBL().GetAccessToken<dynamic>(new RequestModel<Dictionary<string, StringValues>> { RequestObject = vals });
        return ReturnResultBaseClass.TokenreturnResult(serviceResponse);
    }




}

