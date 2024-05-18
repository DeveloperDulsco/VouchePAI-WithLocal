using Domain;
using Domain.Response;
using Domain.Responses;
using Microsoft.Extensions.Primitives;

namespace BussinessLogic.Abstractions
{
    public interface IPayment
    {
        Task<ServiceResponse<object>> InsertPayment(RequestModel<PaymentModel> request);
        Task<ServiceResponse<object>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request);
        Task<ServiceResponse<IEnumerable<FetchPaymentTransaction>>> FetchPaymentDetails(RequestModel<string> request);
        Task<ServiceResponse<PaymentResponse>> CapturePayment(RequestModel<PaymentRequest> request);

    }
}
