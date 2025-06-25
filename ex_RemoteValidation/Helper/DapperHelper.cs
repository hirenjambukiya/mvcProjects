using Microsoft.Data.SqlClient;
using System.Data;

namespace ex_RemoteValidation.Helper
{
    public class DapperHelper
    {
        private readonly IConfiguration _config;
        public DapperHelper(IConfiguration config)
        {
            _config = config;
        }

        public IDbConnection CreateConnection() =>
            new SqlConnection(_config.GetConnectionString("DefaultConnection"));
    }
}
