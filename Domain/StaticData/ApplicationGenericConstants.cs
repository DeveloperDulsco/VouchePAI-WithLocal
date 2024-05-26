namespace Domain;

public static class ApplicationGenericConstants
{
    public const string SUCCESS = "SUCCESS";
    public const string FAILURE = "FAILURE";

    public const string UNKNOWN_ERROR = "Unknown Error";
    public const string UNAUTHORIZED = "Unauthorized";
    public const string MISSING_PAYMENT = "Missing or invalid payments Details";
    public const string FAILD_TO_LOAD = "Failed to Load Data ";
    public const string ERR_DATA_CAPTURE = "Error in Data Capture";
    public const string MISSING_CONNECTION_STRING = "Error in Data Fetch";
    public const string MISSING_PAYMENT_SETTINGS = "Error in Data Update";
    public const string DBCON_PARAM = "DBConnection";
    public const string PAYMENT_VALIDATION = "PaymentValidation error";

    public const string PAYMENT_PMS_ERROR ="Error in Updating Payment in PMS";



}


public static class IdentityAPIConstants
{
    public const string IdentityEndPoint = "identity/oauth2/v2.0/token";

   
}


public static class PaymentAPIConstants
{
    public const string PaymentAPIEndV1Point = "v1/Payments";
    public const string PaymentAPIEndPoint = "Payments";
    public const string CapturePaymentEndpoint = "/CapturePayment";

   
}

public static class OperaConstants
{
    public const string PreAuthUDFFieldName = "PreAuthUDF";
    public const string PreAuthAmntUDFieldName = "PreAuthAmntUDF";

    public const string StationIDCheckIn = "MCI";
    public const string StationIDCheckOut = "MCO";
    


   
}
