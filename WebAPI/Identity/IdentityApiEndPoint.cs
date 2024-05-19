using BussinessLogic;
using Domain.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

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

        IdentityGroup.MapPost("/", GenerateTokenAsync).Produces<Ok>().Produces<UnauthorizedHttpResult>().Produces<BadRequest>().AllowAnonymous();
        return IdentityGroup;
    }




    private static async Task<IResult?> GenerateTokenAsync(HttpRequest httpRequest, PaymentBL paymentBL)
    {

        var vals = (await httpRequest.ReadFormAsync()).ToDictionary(); ;
        var serviceResponse = await paymentBL.GetAccessToken(vals);
        return ReturnResultBaseClass.TokenreturnResult(serviceResponse);
    }




}

