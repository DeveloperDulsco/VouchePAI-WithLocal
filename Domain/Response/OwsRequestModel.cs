using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Response
{
    public class OwsRequestModel
    {
       
            public string HotelDomain { get; set; }
            public string KioskID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string SystemType { get; set; }
            public string Language { get; set; }
            public string LegNumber { get; set; }
            public string ChainCode { get; set; }
            public string DestinationEntityID { get; set; }
            public string DestinationSystemType { get; set; }
            public MakePaymentRequest MakePaymentRequest { get; set; }
            public ModifyBookingRequest modifyBookingRequest { get; set; }

    }
    public class MakePaymentRequest
    {
        public string PaymentInfo { get; set; }
        public decimal Amount { get; set; }
        public string StationID { get; set; }
        public int? WindowNumber { get; set; }
        public string ReservationNameID { get; set; }
        public string PaymentTypeCode { get; set; }
        public string MaskedCardNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string UserName { get; set; }
        public string PaymentRefernce { get; set; }
        public string ApprovalCode { get; set; }
        public string PaymentTerminalID { get; set; }
        public string Rule { get; set; }
        public bool IsSale { get; set; }
        public string supplementary { get; set; }
    }
    public class PaymentMethod
    {
        public string PaymentType { get; set; } //Added
        public string MaskedCardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string AprovalCode { get; set; }
    }
    public class ModifyBookingRequest
    {
        public string ReservationNumber { get; set; }
        public bool? isUDFFieldSpecified { get; set; }
        public List<UDFField> uDFFields { get; set; }
        public bool? updateCreditCardDetails { get; set; }
        public string GarunteeTypeCode { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public bool? isETASpecified { get; set; }
        public DateTime? ETA { get; set; }
    }
    public class UDFField
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
