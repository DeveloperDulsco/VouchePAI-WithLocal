using Domain;
using Domain.Response;
using Domain.Responses;
using System.Text;
using System.Xml;

namespace BussinessLogic;


public interface IAdenPayment
{
    public Task<ServiceResponse<PaymentResponse>> CapturePayment(PaymentRequest paymentRequest);

    public  Task<ServiceResponse<object?>> ModifyBooking(OwsRequestModel owsRequest);
    public Task<ServiceResponse<object?>> MakePayment(OwsRequestModel owsRequest);
}
