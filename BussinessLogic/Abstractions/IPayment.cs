using DataAccessLayer.Model;
using Domain;
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
        Task<ServiceResponse<object>> GetAccessToken(RequestModel<TokenRequest> request);


    }
}
