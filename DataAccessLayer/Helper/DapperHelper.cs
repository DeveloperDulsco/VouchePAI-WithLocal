
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace DataAccessLayer.Helper
{
    public class DapperHelper
    {
        string _connectionString {get;set;}

        public DapperHelper(string connectionString)
        {
            _connectionString = connectionString;
           
                             

        }

        public async Task<IEnumerable<T>> ExecuteSPAsync<T>(string sprocName, object? sprocParams = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                IEnumerable<T> data = Activator.CreateInstance<List<T>>();
                await connection.OpenAsync();

                data = await connection.QueryAsync<T>(sprocName, param: sprocParams, commandType: CommandType.StoredProcedure);
                return data;
            }
        }

        public async Task<IEnumerable<dynamic>> ExecuteSPAsync(string sprocName, object? sprocParams = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sprocName, param: sprocParams, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string query, object? parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                return await connection.ExecuteAsync(query, param: parameters, commandType: CommandType.Text);
            }
        }

        public async Task<T> ExecuteScalarAsync<T>(string query, object? parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                return await connection.ExecuteScalarAsync<T>(query, param: parameters, commandType: CommandType.Text);
            }
        }

        public async Task<string?> getOperaPaymentType(string cardType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                var result = await connection.ExecuteScalarAsync<string>("SELECT top 1 OperaPaymentTypeCode FROM tbOperaPaymentTypeCode  where VendorPaymentTypeCode=@cardType", new { cardType = cardType });
                return result;
            }
        }
        public async Task<string?> getOperaPaymentTypes(string sprocName, object? sprocParams = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var result = await connection.ExecuteScalarAsync<string>(sprocName, param: sprocParams, commandType: CommandType.StoredProcedure);
                return result;
            }
        }


    }

}
