using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Model
{
    public class PaymentRequest
    {
        public string merchantAccount { get; set; }
        public string ApiKey { get; set; }
      
        public CaptureRequest RequestObject { get; set; }
    }
    public class CaptureRequest
    {
        public string OrginalPSPRefernce { get; set; }
        public decimal? Amount { get; set; }
        public string adjustAuthorisationData { get; set; }
        //public string MerchantReference { get; set; }
    }
}
