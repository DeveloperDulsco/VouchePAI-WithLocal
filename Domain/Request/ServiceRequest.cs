using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Request
{
    public class ServiceRequest<T>
    {
        public T? RequestObject { get; set; }
    }
}
