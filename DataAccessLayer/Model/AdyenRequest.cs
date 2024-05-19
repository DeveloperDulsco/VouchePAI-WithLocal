
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class PaymentRequest
    {
        public string? merchantAccount { get; set; }
        public string? ApiKey { get; set; }

        public CaptureRequest? RequestObject { get; set; }
    }
    public class CaptureRequest
    {
        public string? OrginalPSPRefernce { get; set; }
        public decimal? Amount { get; set; }
        public string? adjustAuthorisationData { get; set; }
        //public string MerchantReference { get; set; }
    }
    public class TokenRequest
    {

        public string? Client_Id { get; set; }


        public string? Scope { get; set; }

        public string? Client_Secret { get; set; }

        public string? Grant_Type { get; set; }


    }

    public class TokenResponse
    {

        public string? access_token { get; set; }


        public int expires_in { get; set; }


        public int ext_expires_in { get; set; }


        public string? token_type { get; set; }
    }

    public class ErrorResponse
    {

        public string? error { get; set; }


        public string? error_description { get; set; }


        public List<int>? error_codes { get; set; }


        public string? timestamp { get; set; }


        public string? trace_id { get; set; }

        public string? correlation_id { get; set; }
    }
}
