using Domain;

namespace Domain;

public class ServiceResult
{
    public async Task<ServiceResponse<T?>> GetServiceResponseAsync<T>(T? responseData, string? message, ApiResponseCodes apiResponseCode, int statusCode,List<ValidationError>? validationErrors=null)=>
    await Task.FromResult(new ServiceResponse<T?>()
    {
        ResponseData=responseData,
        Message=message,
        ApiResponseCode=apiResponseCode,
        StatusCode=statusCode,
        ValidationErrors=validationErrors
    });

}


//This code should be moved Business Logic