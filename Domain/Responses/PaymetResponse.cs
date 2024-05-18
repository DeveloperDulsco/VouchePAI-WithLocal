using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Responses
{
    public class PaymentResponse
    {
        public string CardToken { get; set; }
        public string RefusalReason { get; set; }
        public string CardExpiryDate { get; set; }
        public string PaymentToken { get; set; }
        public string MerchantRefernce { get; set; }
        public string AuthCode { get; set; }
        public string CardType { get; set; }
        public string FundingSource { get; set; }
        public string PspReference { get; set; }
        public string ResultCode { get; set; }
        public string MaskCardNumber { get; set; }
        public string Currency { get; set; }
        public decimal? Amount { get; set; }
        public List<AdditionalInfo> additionalInfos { get; set; }
        public List<PaymentReceipt> paymentReceipts { get; set; }
        public string CardAquisitionID { get; set; }

    }
    public class AdditionalInfo
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    public class PaymentReceipt
    {
        public List<ReceiptItem> receiptItems { get; set; }
        public PaymentReceiptType PaymentReceiptType { get; set; }
        public bool isSignatureRequired { get; set; }

    }
    public class ReceiptItem
    {
        public string ItemValue { get; set; }
        public string ItemName { get; set; }
        public string ItemKey { get; set; }
    }

    public enum PaymentReceiptType
    {
        CustomerCopy, MerchantCopy
    }
    public class FetchPaymentTransaction
    {
        public int PaymentID { get; set; }
        public string TransactionID { get; set; }
        public string ReservationNumber { get; set; }
        public string ReservationNameID { get; set; }
        public string MaskedCardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string FundingSource { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string RecurringIdentifier { get; set; }
        public string AuthorisationCode { get; set; }
        public string pspReferenceNumber { get; set; }
        public string ParentPspRefereceNumber { get; set; }
        public string ResultCode { get; set; }
        public string ResponseMessage { get; set; }
        public bool IsActive { get; set; }
        public string TransactionType { get; set; }
        public string DisplayTransactionType { get; set; }
        public string StatusType { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string CardType { get; set; }
        public string OperaPaymentTypeCode { get; set; }

        public string NotificationStatus { get; set; }

        public string AdjustAuthorisationData { get; set; }
        public string Reason { get; set; }
        public string UserName { get; set; }
        public bool VisaCardExpiry { get; set; } = false;
    }
    public class ResponseModel<T>
    {
        public T? ResponseObject { get; set; }
        public bool Result { get; set; } = false;
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
