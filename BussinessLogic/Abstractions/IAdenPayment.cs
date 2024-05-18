using Domain;
using Domain.Response;
using Domain.Responses;

namespace BussinessLogic;


public interface IAdenPayment
{
    public Task<ServiceResponse<PaymentResponse>> CapturePayment(PaymentRequest paymentRequest);


}
