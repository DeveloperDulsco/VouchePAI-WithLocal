using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic
{


   
    public class PmsUpdateSettings
    {
        public string? HotelDomain { get; set; }
        public string? KioskID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? SystemType { get; set; }
        public string? Language { get; set; }
        public string? LegNumber { get; set; }
        public string? ChainCode { get; set; }
        public string? DestinationEntityID { get; set; }
        public string? PreAuthUDF { get; set; }
        public string? PreAuthAmntUDF { get; set; }
        public string? DestinationSystemType { get; set; }
        public string? GuranteeTypeCode { get; set; }
        public bool IsUDFUpdate {  get; set; }=true;
        public bool TestCard { get; set; } = false;
        
    }
}
