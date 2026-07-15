using ELMS.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace ELMS.Services.Repositories
{
    public class DBManager:IDBManager
    {
        private readonly IConfiguration _configuration;
        public DBManager(IConfiguration configuration)
        {
            _configuration = configuration; 
        }
        public IDbConnection CreateConnection(string ConKey)
        {
            try
            {   string connectionString = _configuration.GetConnectionString(ConKey);
                return new SqlConnection(connectionString);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
