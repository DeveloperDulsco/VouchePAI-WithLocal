

namespace Domain.Response
{
    public class RequestModel<T>
    {
        public T? RequestObject { get; set; }
        public string? ConnectionString { get; set; }
    }
    public class PaymentRequest
    {
        public string? merchantAccount { get; set; }


        public CaptureRequest? RequestObject { get; set; }
        public string? ReservationNameID { get; set; }
        public string? ReservationNumber { get; set; }
        public string? TransactionId { get; set; }
    }
    public class CaptureRequest
    {
        public string? OrginalPSPRefernce { get; set; }
        public decimal Amount { get; set; }
        public string? adjustAuthorisationData { get; set; }
        //public string MerchantReference { get; set; }
    }
    public class PaymentModel
    {

        public List<PushPaymentHeaderModel>? paymentHeaders { get; set; }
        public List<PaymentAdditionalInfo>? paymentAdditionalInfos { get; set; }
        public List<PaymentHistory>? paymentHistories { get; set; }
        public TransactionType transaction { get; set; }


    }
    public enum TransactionType
    {
        PreAuth,
        Capture,
        Sale
    }
    public class PushPaymentHeaderModel
    {
        public string TransactionID { get; set; }
        public string ReservationNumber { get; set; }
        public string ReservationNameID { get; set; }
        public string MaskedCardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string FundingSource { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string RecurringIdentifier { get; set; }
        public string AuthorisationCode { get; set; }
        public string pspReferenceNumber { get; set; }
        public string ParentPspRefereceNumber { get; set; }
        public string TransactionType { get; set; }
        public string ResultCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool? IsActive { get; set; }
        public string StatusType { get; set; }
        public string CardType { get; set; }
        public  string OperaPaymentTypeCode { get; set; }
        public string? ApprovalCode { get; set; }
    }
    public class PushPaymentModel
    {
        public string TransactionID { get; set; }
        public string ReservationNumber { get; set; }
        public string ReservationNameID { get; set; }
        public string MaskedCardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string FundingSource { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string RecurringIdentifier { get; set; }
        public string AuthorisationCode { get; set; }
        public string pspReferenceNumber { get; set; }
        public string ParentPspRefereceNumber { get; set; }
        public string TransactionType { get; set; }
        public string ResultCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool? IsActive { get; set; }
        public string StatusType { get; set; }
        public string CardType { get; set; }
       
    }
    public class PaymentAdditionalInfo
    {
        public string TransactionID { get; set; }
        public string KeyHeader { get; set; }
        public string KeyValue { get; set; }
    }
    public class UpdatePaymentModel
    {
        public string transactionID { get; set; }
        public string ResultCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool? isActive { get; set; }
        public string? transactionType { get; set; }
        public decimal amount { get; set; }
        public string ReservationNumber { get; set; }
    }
    public class PaymentFetchRequest
    {
        public string? ReservationNumber { get; set; }
    }
    public class PaymentHistory
    {
        public string TransactionID { get; set; }
        public string ReservationNameID { get; set; }
        public string ReservationNumber { get; set; }
        public string PData { get; set; }
        public string MDData { get; set; }
        public string PaRes { get; set; }
        public string PSPReference { get; set; }
        public string ResultCode { get; set; }
        public string RefusalReason { get; set; }
        public string TransactionType { get; set; }
    }
}
