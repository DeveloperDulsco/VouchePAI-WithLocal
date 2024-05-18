using DataAccessLayer.Model;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Abstractions
{
    public interface IPayment
    {
        Task<ServiceResponse<PaymentResponse>> CapturePayment(RequestModel<PaymentRequest> request);
        Task<ServiceResponse<object>> InsertPayment(RequestModel<PaymentModel> request);
        Task<ServiceResponse<object>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request);
        Task<ServiceResponse<IEnumerable<FetchPaymentTransaction>>> FetchPaymentDetails(RequestModel<string> request);
        Task<ServiceResponse<dynamic>> GetAccessToken<dynamic>(RequestModel<Dictionary<string,StringValues>>request);


    }
}
