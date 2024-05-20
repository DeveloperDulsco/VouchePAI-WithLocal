using Domain.Response;
using Domain.Responses;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Abstractions
{
    public interface IOperaService
    {
        public Task<ServiceResponse<object>> MakePayment(OwsRequestModel Request);
        public Task<ServiceResponse<object>> ModifyReservation(OwsRequestModel Request);
    }
}
