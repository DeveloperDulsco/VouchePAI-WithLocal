using Domain;
using Domain.Request;
using Domain.Response;

namespace BussinessLogic.Abstractions;

public interface IPaymentValidation
{

    public Task<ValidationResult?> ValidateInsertPayment (RequestModel<PaymentModel> paymentRequest);
    public Task<ValidationResult?> ValidateFetchPayment (RequestModel<PaymentFetchRequest> request);
    public Task<ValidationResult?> ValidateUpdatePayment (RequestModel<UpdatePaymentModel> request);
     public Task<ValidationResult?> ValidateCapturePayment(RequestModel<PaymentRequest> request);
     



}
