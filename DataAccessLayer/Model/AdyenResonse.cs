using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
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

}
