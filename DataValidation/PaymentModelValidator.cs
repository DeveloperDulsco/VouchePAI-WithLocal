


using BussinessLogic.Abstractions;
using Domain;
using Domain.Request;
using Domain.Response;
using FluentValidation;



namespace DataValidation;

public class PaymentValidation : IPaymentValidation
{
    public Task<ValidationResult?> ValidateCapturePayment(RequestModel<PaymentRequest> request)
    {
        PaymentCaptureRequestValidator validator = new PaymentCaptureRequestValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(request);
         var result= ValidationConversion.ConvertToDomainValidationResult(validationResult);
         return  Task.FromResult(result);
    }

    public Task<ValidationResult?> ValidateFetchPayment(RequestModel<PaymentFetchRequest> request)
    {
        PaymentFetchRequestValidator validator = new PaymentFetchRequestValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(request);
        
        var result= ValidationConversion.ConvertToDomainValidationResult(validationResult);
        return  Task.FromResult(result);
    }

    public  Task<Domain.ValidationResult?> ValidateInsertPayment(RequestModel<PaymentModel> paymentRequest)
    {
        PaymentInsertRequestValidator validator = new PaymentInsertRequestValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(paymentRequest);
        
        var result= ValidationConversion.ConvertToDomainValidationResult(validationResult);
        return  Task.FromResult(result);
    }

    public Task<ValidationResult?> ValidateUpdatePayment(RequestModel<UpdatePaymentModel> request)
    {
        PaymentUpdateRequestValidator validator = new PaymentUpdateRequestValidator();
        FluentValidation.Results.ValidationResult validationResult = validator.Validate(request);
        
        var result= ValidationConversion.ConvertToDomainValidationResult(validationResult);
        return  Task.FromResult(result);
    }
}


public class PaymentInsertRequestValidator :   AbstractValidator<RequestModel<PaymentModel>>
{
    public PaymentInsertRequestValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.RequestObject).NotNull();
        RuleFor(x => x.RequestObject.paymentHeaders).NotNull();
        RuleFor(x => x.RequestObject.paymentHistories).NotNull();
      
        RuleForEach(x => x.RequestObject.paymentHeaders).ChildRules(header => 
         {
                header.RuleFor(x => x.TransactionID).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.ReservationNumber).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.ReservationNameID).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.MaskedCardNumber).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.ExpiryDate).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9/]*$");
                header.RuleFor(x => x.FundingSource).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.Amount).NotNull().NotEmpty().Matches(@"^[0-9.-]*$");
                
                header.RuleFor(x => x.RecurringIdentifier).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.AuthorisationCode).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.pspReferenceNumber).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.ParentPspRefereceNumber).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
                header.RuleFor(x => x.TransactionType).NotNull().NotEmpty().Matches(@"^[a-zA-Z]*$");
                header.RuleFor(x => x.ResultCode).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
               
                
              
         });

        
      
        

    }

   
}

public class PaymentFetchRequestValidator : AbstractValidator<RequestModel<PaymentFetchRequest>>
{
    public PaymentFetchRequestValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.RequestObject).NotNull();
        RuleFor(x => x.RequestObject.ReservationNumber).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
       
    }
}

public class PaymentCaptureRequestValidator : AbstractValidator<RequestModel<PaymentRequest>>
{
    public PaymentCaptureRequestValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.RequestObject).NotNull();
       
        RuleFor(x => x.RequestObject.ReservationNumber).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
        RuleFor(x => x.RequestObject.TransactionId).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
       
        RuleFor(x => x.RequestObject.Amount).NotNull().NotEmpty().NotEqual(0);
        RuleFor(x => x.RequestObject.OrginalPSPRefernce).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
       // RuleFor(x => x.RequestObject.CaptureObject.adjustAuthorisationData).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");

        
    }
}

public class PaymentUpdateRequestValidator : AbstractValidator<RequestModel<UpdatePaymentModel>>
{
    public PaymentUpdateRequestValidator()
    {
        RuleFor(x => x).NotNull();
        RuleFor(x => x.RequestObject).NotNull();
        RuleFor(x => x.RequestObject.ReservationNumber).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
        RuleFor(x => x.RequestObject.transactionID).NotNull().NotEmpty().Matches(@"^[a-zA-Z0-9]*$");
     
        RuleFor(x => x.RequestObject.transactionType).NotNull().NotEmpty().Matches(@"^[a-zA-Z]*$");
        

    }

}



static class ValidationConversion
{
internal static Domain.ValidationResult  ConvertToDomainValidationResult (FluentValidation.Results.ValidationResult validationResult)
{
    Domain.ValidationResult result = new Domain.ValidationResult();
    result.IsValid = validationResult.IsValid;
    foreach (var error in validationResult.Errors)
    {
        result.Errors.Add(new Domain.ValidationError
        {
            ErrorMessage = error.ErrorMessage,
           
        });
    }
    return result;
}
}




