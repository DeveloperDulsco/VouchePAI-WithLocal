using Domain;
using Microsoft.AspNetCore.Http;

namespace WebAPI;
internal static class ReturnResultBaseClass
{
internal static IResult returnResult<T>(ServiceResponse<T> serviceResponse)=>
    serviceResponse.ApiResponseCode==ApiResponseCodes.SUCCESS ? TypedResults.Ok(serviceResponse):
    serviceResponse.StatusCode==400 ? TypedResults.BadRequest(serviceResponse):
    serviceResponse.StatusCode==204 ? TypedResults.NoContent(): TypedResults.NotFound(serviceResponse);

}
