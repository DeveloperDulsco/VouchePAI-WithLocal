﻿using Domain;
using Microsoft.AspNetCore.Http;

namespace WebAPI;
internal static class ReturnResultBaseClass
{
    internal static IResult returnResult<T>(ServiceResponse<T> serviceResponse) =>
        serviceResponse.ApiResponseCode == ApiResponseCodes.SUCCESS ? TypedResults.Ok(serviceResponse) :
        serviceResponse.StatusCode == 400 ? TypedResults.BadRequest(serviceResponse) :
        serviceResponse.StatusCode == 401 ? TypedResults.Unauthorized() :
        serviceResponse.StatusCode == 204 ? TypedResults.NoContent() : TypedResults.NotFound(serviceResponse);


    internal static IResult TokenreturnResult<T>(ServiceResponse<T> serviceResponse) =>
    serviceResponse.StatusCode == 200 ? TypedResults.Ok(serviceResponse.ResponseData) :
    serviceResponse.StatusCode == 400 ? TypedResults.BadRequest(serviceResponse.ResponseData) :
    serviceResponse.StatusCode == 401 ? TypedResults.Unauthorized():
    serviceResponse.StatusCode == 204 ? TypedResults.NoContent() : TypedResults.NotFound(serviceResponse.ResponseData);

}
