using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Model;
using DataAccessLayer.Helper;
using System.Net.Http;

namespace DataAccessLayer.Repository
{
    public class PaymentRepository
    {

        /// <summary>
        /// inserting Payment details to payment header table
        /// </summary>
        /// <param name="paymentHistories"></param>
        /// <param name="paymentHeaders"></param>
        /// <param name="paymentAdditionalInfos"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool>> InsertPaymentDetails(List<PaymentHistory> paymentHistories, List<PushPaymentHeaderModel> paymentHeaders, List<PaymentAdditionalInfo> paymentAdditionalInfos)
        {
           
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
                var spResponse = await new DapperHelper().ExecuteSPAsync("Usp_InsertPaymentTransactions"
                        , new { TbPaymentHeaderType = new DBHelper().ToDataTable(paymentHeaders), TbPaymentAdditionalInfoType = new DBHelper().ToDataTable(paymentAdditionalInfos), TbPaymentHistoryType = new DBHelper().ToDataTable(paymentHistories) });
                if (spResponse is not null)
                    responseModel.Result = true;
               
                else
                    {
                        responseModel.Result = false;
                        responseModel.ErrorMessage = "Failed to Update Data";
                    }
                   return responseModel;
        }
        /// <summary>
        /// Update the Payment Header after 
        /// </summary>
        /// <param name="transactionID"></param>
        /// <param name="ResultCode"></param>
        /// <param name="ResponseMessage"></param>
        /// <param name="isActive"></param>
        /// <param name="transactionType"></param>
        /// <param name="amount"></param>
        /// <param name="ReservationNumber"></param>
        /// <returns></returns>
        public async Task<ResponseModel<bool>> UpdatePaymentHeaderData(UpdatePaymentModel updatePayment)
        {
            ResponseModel<bool> responseModel = new ResponseModel<bool>();
                 var ProfilesDt = await new DapperHelper().ExecuteSPAsync("Usp_UpdatePaymentHeader"
                        , new { TransactionID = updatePayment.transactionID, ResultCode = updatePayment.ResultCode, ResponseMessage = updatePayment.ResponseMessage , IsActive = updatePayment.isActive, TransactionType= updatePayment.transactionType , Amount = updatePayment.amount , ReservationNumber = updatePayment.ReservationNumber });

                if (ProfilesDt != null && ProfilesDt.Any())
            
                        responseModel.Result = true;
                    else
                       {    responseModel.Result = false;
                            responseModel.ErrorMessage = "Failed to Update Data";
                       }
            return responseModel;
        }
       

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

    }
}
