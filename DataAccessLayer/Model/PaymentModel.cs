using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class PaymentModel
    {

        public List<PushPaymentHeaderModel>? paymentHeaders { get; set; }
        public List<PaymentAdditionalInfo>? paymentAdditionalInfos { get; set; }
        public List<PaymentHistory>? paymentHistories { get; set; }

    }
    public class PushPaymentHeaderModel
    {
        public string? TransactionID { get; set; }
        public string? ReservationNumber { get; set; }
        public string? ReservationNameID { get; set; }
        public string? MaskedCardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? FundingSource { get; set; }
        public string? Amount { get; set; }
        public string? Currency { get; set; }
        public string? RecurringIdentifier { get; set; }
        public string? AuthorisationCode { get; set; }
        public string? pspReferenceNumber { get; set; }
        public string? ParentPspRefereceNumber { get; set; }
        public string? TransactionType { get; set; }
        public string? ResultCode { get; set; }
        public string? ResponseMessage { get; set; }
        public bool? IsActive { get; set; }
        public string? StatusType { get; set; }
        public string? CardType { get; set; }
    }
    public class PaymentAdditionalInfo
    {
        public string? TransactionID { get; set; }
        public string? KeyHeader { get; set; }
        public string? KeyValue { get; set; }
    }
    public class UpdatePaymentModel
    {
        public string? transactionID { get; set; }
        public string? ResultCode { get; set; }
        public string? ResponseMessage { get; set; }
        public bool? isActive { get; set; }
        public string? transactionType { get; set; }
        public decimal amount { get; set; }
        public string? ReservationNumber { get; set; }
    }

    public class PaymentHistory
    {
        public string? TransactionID { get; set; }
        public string? ReservationNameID { get; set; }
        public string? ReservationNumber { get; set; }
        public string? PData { get; set; }
        public string? MDData { get; set; }
        public string? PaRes { get; set; }
        public string? PSPReference { get; set; }
        public string? ResultCode { get; set; }
        public string? RefusalReason { get; set; }
        public string? TransactionType { get; set; }
    }

    public class FetchPaymentTransaction
    {
        public int PaymentID { get; set; }
        public string? TransactionID { get; set; }
        public string? ReservationNumber { get; set; }
        public string? ReservationNameID { get; set; }
        public string? MaskedCardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? FundingSource { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? RecurringIdentifier { get; set; }
        public string? AuthorisationCode { get; set; }
        public string? pspReferenceNumber { get; set; }
        public string? ParentPspRefereceNumber { get; set; }
        public string? ResultCode { get; set; }
        public string? ResponseMessage { get; set; }
        public bool IsActive { get; set; }
        public string? TransactionType { get; set; }
        public string? DisplayTransactionType { get; set; }
        public string? StatusType { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string? CardType { get; set; }
        public string? OperaPaymentTypeCode { get; set; }

        public string? NotificationStatus { get; set; }

        public string? AdjustAuthorisationData { get; set; }
        public string? Reason { get; set; }
        public string? UserName { get; set; }
        public bool VisaCardExpiry { get; set; } = false;
    }
    public class RequestModel<T>
    {
        public T? RequestObject { get; set; }
        public string? ConnectionString { get; set; }
    }
    public class ResponseModel<T>
    {
        public T? ResponseObject { get; set; }
        public bool Result { get; set; } = false;
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
