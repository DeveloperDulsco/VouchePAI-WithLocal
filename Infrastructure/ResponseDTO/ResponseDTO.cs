using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ResponseDTO
{
    public class ResponseDTO
    {
       
        public bool Result { get; set; } = false;
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
    }

  
}
