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

       
        public async Task<ResponseModel> InsertPayment(RequestModel request)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                if (request != null)
                {
                    PaymentModel? paymentDetails = JsonSerializer.Deserialize <PaymentModel>(request.RequestObject.ToString());
                    if (paymentDetails != null)
                    {


                     var respose=   await _prepository.InsertPaymentDetails(paymentDetails.paymentHistories, paymentDetails.paymentHeaders, paymentDetails.paymentAdditionalInfos);
                        if(respose != null && respose.Result)
                        {
                            return new ResponseModel() {Result = true};

                        }
                        else
                        {
                            return new ResponseModel() { ErrorMessage = "Some Error Occured from DB", Result = false, ResponseObject = "" };

                        }
                    }
                    else
                    {
                        return new ResponseModel() { ErrorMessage = "Request object can not be null", Result = false, ResponseObject = "" };

                    }

                }
                else { 
                
                return new ResponseModel(){ ErrorMessage= "Request object can not be null",Result=false,ResponseObject=""};
                   
                }


            }
            catch {
               
            }
            return responseModel;
        }
        public async Task<ResponseModel> UpdatePaymentHeader(RequestModel request)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                if (request != null)
                {
                    UpdatePaymentModel updatePayment = JsonSerializer.Deserialize <UpdatePaymentModel>(request.RequestObject.ToString());
                    if (updatePayment != null)
                    {


                        var respose = await _prepository.UpdatePaymentHeaderData(updatePayment);
                        if (respose != null && respose.Result)
                        {
                            return new ResponseModel() { Result = respose.Result };

                        }
                        else
                        {
                            return new ResponseModel() { ErrorMessage =respose.ErrorMessage, Result = respose.Result, ResponseObject = "" };

                        }
                    }
                    else
                    {
                        return new ResponseModel() { ErrorMessage = "Request object can not be null", Result = false, ResponseObject = "" };

                    }

                }
                else
                {

                    return new ResponseModel() { ErrorMessage = "Request object can not be null", Result = false, ResponseObject = "" };

                }


            }
            catch
            {

            }
            return responseModel;
        }
        public async Task<ServiceResponse<ResponseModel>> FetchPaymentDetails(RequestModel request)
        {
           
            ServiceResult serviceResult=new ServiceResult();
            ResponseModel responseModel = new ResponseModel();
            var respose = await _prepository.FetchPaymentActiveTransactions(request.RequestObject.ToString());
            if (respose != null && respose.Result) return await serviceResult.GetServiceResponseAsync<ResponseModel>(responseModel,ApplicationGenericConstants.SUCCESS,ApiResponseCodes.SUCCESS,200,null);
            else
            return await serviceResult.GetServiceResponseAsync<ResponseModel>(null,ApplicationGenericConstants.FAILURE,ApiResponseCodes.FAILURE,400,null);


        }
    }
}
