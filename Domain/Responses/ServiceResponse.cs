namespace Domain;

public class ServiceResponse
{
    public ApiResponseCodes ApiResponseCode {get;set;}=ApiResponseCodes.FAILURE;
    public string? Message {get;set;}
    public  List<ValidationError>? ValidationErrors {get;set;}
    public string?RefId {get;set;}=System.Diagnostics.Activity.Current?.Id;
    public dynamic? ResponseData {get;set;}=null;

    public int StatusCode {get;set;}

}

public sealed class ServiceResponse<T>: ServiceResponse {
    public new T? ResponseData {get;set;}
}

public enum ApiResponseCodes
{
    SUCCESS=0,
    FAILURE=1,
}


public sealed class ValidationError
{
    public string? TechnicalError {get;set;}
    public string ? ErrorMessage {get;set;}
    public int ErrorNumber {get;set;}=1;
    List<MessageContent>? MessageContents {get;set;}

}

public sealed class MessageContent
{
    public string? LanguageCode {get;set;}
    public string? Content {get;set;}

}

