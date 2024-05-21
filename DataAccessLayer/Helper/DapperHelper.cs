
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
            _connectionString = "Data Source=94.203.133.74,1433;Initial Catalog=QC_SaavyPayDB;user id=sbs_administrator;password=P@ssw0rd@2020";

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



    }

}
