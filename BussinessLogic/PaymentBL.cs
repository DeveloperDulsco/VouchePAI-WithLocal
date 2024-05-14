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


        public async Task<ServiceResponse<object>> InsertPayment(RequestModel request)
        {
            ServiceResult serviceResult = new ServiceResult();
            PaymentModel? paymentDetails = JsonSerializer.Deserialize<PaymentModel>(request.RequestObject.ToString());   
            if (paymentDetails is not null)
            {   
                var respose = await _prepository.InsertPaymentDetails(paymentDetails.paymentHistories, paymentDetails.paymentHeaders, paymentDetails.paymentAdditionalInfos);
               
                    if (respose is not null && respose.Result)
                        return await serviceResult.GetServiceResponseAsync<object>  (null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
                    else
                        return await serviceResult.GetServiceResponseAsync<object>(null, respose?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);
            }
             
            return await serviceResult.GetServiceResponseAsync<object>(null, "Missing or invalid payments Details", ApiResponseCodes.FAILURE, 400, null);
            

        }
        public async Task<ServiceResponse<object>> UpdatePaymentHeader(RequestModel request)
        {
           
            ServiceResult serviceResult = new ServiceResult();
          
            UpdatePaymentModel updatePayment = JsonSerializer.Deserialize <UpdatePaymentModel>(request.RequestObject.ToString());
            if (updatePayment is not  null)
            {
                var respose = await _prepository.UpdatePaymentHeaderData(updatePayment);
                if (respose is not null && respose.Result)
                    return await serviceResult.GetServiceResponseAsync<object>(null, ApplicationGenericConstants.SUCCESS, ApiResponseCodes.SUCCESS, 200, null);
                else
                return await serviceResult.GetServiceResponseAsync<object>(null, respose?.ErrorMessage, ApiResponseCodes.FAILURE, 400, null);
            }
            
                return await serviceResult.GetServiceResponseAsync(request?.RequestObject, "Missing or invalid payments Details", ApiResponseCodes.FAILURE, 400, null);
                                                  
        }
        public async Task<ServiceResponse<IEnumerable<FetchPaymentTransaction>>> FetchPaymentDetails(RequestModel request)
        {
           
            ServiceResult serviceResult=new ServiceResult();
            var respose = await _prepository.FetchPaymentActiveTransactions(request.RequestObject.ToString());
            if (respose is not null && respose.Result) return await serviceResult.GetServiceResponseAsync(respose.ResponseObject,ApplicationGenericConstants.SUCCESS,ApiResponseCodes.SUCCESS,200,null);
            else
            return await serviceResult.GetServiceResponseAsync(respose?.ResponseObject,respose?.ErrorMessage,ApiResponseCodes.FAILURE,400,null);


        }
    }
}
