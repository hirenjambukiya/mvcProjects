using Microsoft.Data.SqlClient;
using System.Data;

namespace ex_TableDataDashboard.Repositories.Interfaces
{
    public interface IDatabaseRepository
    {
        List<string> GetDatabases(SqlConnection conn);
        List<string> GetTables(SqlConnection conn);
        DataTable GetTableData(SqlConnection conn, string table);
    }
}
