using Domain;
using Domain.Response;
using Domain.Responses;
using Microsoft.Extensions.Primitives;

namespace BussinessLogic.Abstractions
{
    public interface IPayment
    {
        public Task<ServiceResponse<object>> InsertPayment(RequestModel<PaymentModel> request);
        public Task<ServiceResponse<object>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request);
        public Task<ServiceResponse<IEnumerable<FetchPaymentTransaction>>> FetchPaymentDetails(RequestModel<PaymentFetchRequest> request);
        //Task<ServiceResponse<PaymentResponse>> CapturePayment(RequestModel<PaymentRequest> request);
        public Task<ServiceResponse<string>> getOperaPaymentType(string AdeyanPaymentType);

    }
}
