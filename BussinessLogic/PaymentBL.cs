using DataAccessLayer.Helper;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using Domain;
using System.Text;
using System.Text.Json;


namespace BussinessLogic
{
    public class PaymentBL
    {

        PaymentRepository _prepository = new PaymentRepository();


        public async Task<ServiceResponse<object>> InsertPayment(RequestModel<PaymentModel> request)
        {
            ServiceResult serviceResult = new ServiceResult();
              
            if (request?.RequestObject is  null)  return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);
             
                PaymentModel? paymentDetails=request?.RequestObject;
                var respose = await _prepository.InsertPaymentDetails(paymentDetails.paymentHistories, paymentDetails.paymentHeaders, paymentDetails.paymentAdditionalInfos);
                  
                    if (respose is not null && respose.Result)
                        return await serviceResult.GetServiceResponseAsync<object>  (null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
                    else
                        return await serviceResult.GetServiceResponseAsync<object>(null, respose?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);
            
             
            
            

        }
        public async Task<ServiceResponse<object>> UpdatePaymentHeader(RequestModel<UpdatePaymentModel> request)
        {
           
            ServiceResult serviceResult = new ServiceResult();
          
            
             if (request?.RequestObject is  null) 
              return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.MISSING_PAYMENT, ApiResponseCodes.FAILURE, 400, null);
            
                UpdatePaymentModel? updatePayment = request?.RequestObject;
                var respose = await _prepository.UpdatePaymentHeaderData(updatePayment);
               
                if (respose is not null && respose.Result)
                    return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
                else
                    return await serviceResult.GetServiceResponseAsync<object>(null, respose?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);
            
            
                
                                                  
        }
        public async Task<ServiceResponse<IEnumerable<FetchPaymentTransaction>>> FetchPaymentDetails(RequestModel<string> request)
        {
           
            ServiceResult serviceResult=new ServiceResult();
            if (request?.RequestObject is  null) 
             return await serviceResult.GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction>>(null,"INVALID RESERVATION ID",ApiResponseCodes.FAILURE,400,null);
            
            var respose = await _prepository.FetchPaymentActiveTransactions(request?.RequestObject);
            if (respose is not null && respose.Result) 
                return await serviceResult.GetServiceResponseAsync(respose.ResponseObject,ApplicationGenericConstants.SUCCESS,ApiResponseCodes.SUCCESS,200,null);
            else 
                return await serviceResult.GetServiceResponseAsync(respose?.ResponseObject,respose?.ErrorMessage,ApiResponseCodes.FAILURE,400,null);
           


        }
    }
}
