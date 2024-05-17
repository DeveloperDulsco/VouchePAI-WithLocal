using BussinessLogic;
using DataAccessLayer.Model;
using Domain;
using Domain.Request;
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

        IdentityGroup.MapPost("/",GenerateTokenAsync).Produces<Ok>().Produces<UnauthorizedHttpResult>().Produces<BadRequest>();
        return IdentityGroup;
    }
    [AllowAnonymous]
    [Consumes("application/x-www-form-urlencoded")]

    private static async Task<IResult?> GenerateTokenAsync([FromForm]  TokenRequest request)
    {
        var response = await new PaymentBL().GetAccessToken(new RequestModel<TokenRequest> { RequestObject = request});
        return ReturnResultBaseClass.returnResult(response);
    }



}

