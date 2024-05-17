using BussinessLogic;
using DataAccessLayer.Model;
using Domain;
using Domain.Request;
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

        IdentityGroup.MapPost("/", GenerateTokenAsync).Produces<Ok>().Produces<UnauthorizedHttpResult>().Produces<BadRequest>();

        return IdentityGroup;

    }



    private static async Task<IResult?> GenerateTokenAsync([FromBody] ServiceRequest<PaymentModel> request)
    {

        //var response = await new PaymentBL().InsertPayment(new RequestModel<PaymentModel> { RequestObject = request.RequestObject});
        //return ReturnResultBaseClass.returnResult(response);
    }



}

