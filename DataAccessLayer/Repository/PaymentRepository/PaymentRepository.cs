using DataAccessLayer.Model;
using DataAccessLayer.Helper;
using BussinessLogic.Abstractions;
using Domain;
using Microsoft.Extensions.Primitives;
using Domain.Response;

namespace DataAccessLayer.Repository
{
    public class PaymentRepository : IPayment
    {

     
       

        /// <summary>
        /// Fetch Active Preauthorization details
        /// </summary>
        /// <param name="ReservationNameID"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<ResponseModel<IEnumerable<FetchPaymentTransaction>>> FetchPaymentActiveTransactions(string reservationNameID)
        {
            ResponseModel<IEnumerable<FetchPaymentTransaction>> responseModel = new ResponseModel<IEnumerable<FetchPaymentTransaction>>();
                 var spResponse = await new DapperHelper().ExecuteSPAsync<FetchPaymentTransaction>("Usp_FetchPaymentTransaction",new { ReservationNameID= reservationNameID });
                if (spResponse != null && spResponse.Any())
                {
                    responseModel.Result = true;
                    responseModel.ResponseObject = spResponse;
                }

                else
                {
                    responseModel.Result = false;
                    responseModel.ErrorMessage = "Failed to Get Data";
                }
                   return responseModel;
        }

        Task<ServiceResponse<Domain.Responses.PaymentResponse>> IPayment.CapturePayment(Domain.Response.RequestModel<Domain.Response.PaymentRequest> request)
        {
            throw new NotImplementedException();
        }

        async Task<ServiceResponse<object>> IPayment.InsertPayment(Domain.Response.RequestModel<Domain.Response.PaymentModel> request)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            var spResponse = await new DapperHelper().ExecuteSPAsync("Usp_InsertPaymentTransactions"
                    , new { TbPaymentHeaderType = new DBHelper().ToDataTable(request.RequestObject.paymentHeaders), TbPaymentAdditionalInfoType = new DBHelper().ToDataTable(request.RequestObject.paymentAdditionalInfos), TbPaymentHistoryType = new DBHelper().ToDataTable(request.RequestObject.paymentHistories) });
            if (spResponse is not null)
                responseModel.Result = true;

            else
            {
                responseModel.Result = false;
                responseModel.ErrorMessage = "Failed to Update Data";
            }
            return await new ServiceResult().GetServiceResponseAsync<object>(responseModel, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }

        async Task<ServiceResponse<object>> IPayment.UpdatePaymentHeader(Domain.Response.RequestModel<Domain.Response.UpdatePaymentModel> request)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
            var ProfilesDt = await new DapperHelper().ExecuteSPAsync("Usp_UpdatePaymentHeader"
                   , new { TransactionID = request.RequestObject.transactionID, ResultCode = request.RequestObject.ResultCode, ResponseMessage = request.RequestObject.ResponseMessage, IsActive = request.RequestObject.isActive, TransactionType = request.RequestObject.transactionType, Amount = request.RequestObject.amount, ReservationNumber = request.RequestObject.ReservationNumber });

            if (ProfilesDt != null && ProfilesDt.Any())

                responseModel.Result = true;
            else
            {
                responseModel.Result = false;
                responseModel.ErrorMessage = "Failed to Update Data";
            }
            return await new ServiceResult().GetServiceResponseAsync<object>(responseModel, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }

        async Task<ServiceResponse<IEnumerable<Domain.Responses.FetchPaymentTransaction>>> IPayment.FetchPaymentDetails(Domain.Response.RequestModel<string> request)
        {
            ResponseModel<IEnumerable<FetchPaymentTransaction>> responseModel = new ResponseModel<IEnumerable<FetchPaymentTransaction>>();
            var spResponse = await new DapperHelper().ExecuteSPAsync<FetchPaymentTransaction>("Usp_FetchPaymentTransaction", new { ReservationNameID = request.RequestObject });
            if (spResponse != null && spResponse.Any())
            {
                responseModel.Result = true;
                responseModel.ResponseObject = spResponse;
            }

            else
            {
                responseModel.Result = false;
                responseModel.ErrorMessage = "Failed to Get Data";
            }
            return await new ServiceResult().GetServiceResponseAsync<IEnumerable<FetchPaymentTransaction>>(responseData: (IEnumerable<FetchPaymentTransaction>?)responseModel, responseModel.ErrorMessage, ApiResponseCodes.SUCCESS, 200, null);
        }

        Task<ServiceResponse<dynamic>> IPayment.GetAccessToken<dynamic>(Domain.Response.RequestModel<Dictionary<string, StringValues>> request)
        {
            throw new NotImplementedException();
        }
    }
}
