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
        public bool IsUDFUpdate {  get; set; }=true;
        public bool TestCard { get; set; } = false;
        
    }
}
