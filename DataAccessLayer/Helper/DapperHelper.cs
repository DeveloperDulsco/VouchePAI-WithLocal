
using System.Data;
using System.Data.SqlClient;
using Dapper;
namespace DataAccessLayer.Helper
{
    public class DapperHelper
    {
        private readonly string _connectionString;

        public DapperHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<T>> ExecuteSPAsync<T>(string sprocName, object? sprocParams = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync<T>(sprocName, param: sprocParams, commandType: CommandType.StoredProcedure);
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



    }

}
