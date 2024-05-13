using DataAccessLayer.Helper;
using DataAccessLayer.Model;
using DataAccessLayer.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
                    PaymentModel paymentDetails = JsonConvert.DeserializeObject<PaymentModel>(request.RequestObject.ToString());
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
                    UpdatePaymentModel updatePayment = JsonConvert.DeserializeObject<UpdatePaymentModel>(request.RequestObject.ToString());
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
        public async Task<ResponseModel> FetchPaymentDetails(RequestModel request)
        {
            ResponseModel responseModel = new ResponseModel();
            try
            {
                
                   if (request != null)
                    {  


                        var respose = await _prepository.FetchPaymentActiveTransactions(request.RequestObject.ToString());
                        if (respose != null && respose.Result)
                        {
                            return new ResponseModel() { Result = respose.Result,ResponseObject=respose.ResponseObject };

                        }
                        else
                        {
                            return new ResponseModel() { ErrorMessage = respose.ErrorMessage, Result = respose.Result, ResponseObject = "" };

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
    }
}
