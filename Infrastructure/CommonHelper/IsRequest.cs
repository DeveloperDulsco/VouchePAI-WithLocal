using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CommonHelper
{
    public class IsRequestModel
    {
       public string? web_url { get; set; } 
       public object? body { get; set; }
       public Dictionary<string, string>? headers { get; set; }
       public string? accesToken { get; set; }
       public Dictionary<string, string>? formDataBody { get; set; }
        
       public bool isProxyEnabled { get; set; }
       public string? proxyURL { get; set; }
       public string? proxyUN { get; set; }
       public string? proxyPSWD { get; set; }
    }
    
   
}
