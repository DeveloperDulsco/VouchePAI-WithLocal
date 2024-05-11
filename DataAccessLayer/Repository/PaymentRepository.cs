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
        public bool InsertPaymentDetails(List<PaymentHistory> paymentHistories, List<PushPaymentHeaderModel> paymentHeaders, List<PaymentAdditionalInfo> paymentAdditionalInfos, string connectionString)
        {
            bool isSavedSuccessfully = false;

            try
            {


                var spResponse = new DapperHelper(connectionString).ExecuteSPAsync("Usp_InsertPaymentTransactions"
                        , new { TbPaymentHeaderType = new DBHelper().ToDataTable(paymentHeaders), TbPaymentAdditionalInfoType = new DBHelper().ToDataTable(paymentAdditionalInfos), TbPaymentHistoryType = new DBHelper().ToDataTable(paymentHistories) });
                if (spResponse != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {

            }
            return isSavedSuccessfully;
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
        public bool UpdatePaymentHeaderData(string transactionID, string ResultCode, string ResponseMessage, bool? isActive, string transactionType, decimal amount, string ReservationNumber,string connectionString)
        {
            try
            {
               


              var ProfilesDt = new DapperHelper(connectionString).ExecuteSPAsync("Usp_UpdatePaymentHeader"
                        , new { TransactionID = transactionID, ResultCode = ResultCode , ResponseMessage = ResponseMessage , IsActive = isActive, TransactionType= transactionType , Amount = amount , ReservationNumber = ReservationNumber }).Result.ToList();

                if (ProfilesDt != null && ProfilesDt.Count > 0)
                {

                   
                        return true;
                    }
                    else
                    {
                      return false;
                    }
                
            }
            catch (Exception ex)
            {
              }
            return false;
        }

        /// <summary>
        /// Fetch Active Preauthorization details
        /// </summary>
        /// <param name="ReservationNameID"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public async Task<List<FetchPaymentTransaction>> FetchPaymentActiveTransactions(string ReservationNameID, string connectionString)
        {
            var spResponse = new List<FetchPaymentTransaction>();
            try
            {

                 spResponse = new DapperHelper(connectionString).ExecuteSPAsync<FetchPaymentTransaction>("Usp_FetchPaymentTransactionsForPortal_Test",
                   new { ReservationNameID = ReservationNameID }).Result.ToList();
              
            }
            catch (Exception ex)
            {
            }
            return spResponse;
        }

    }
}
