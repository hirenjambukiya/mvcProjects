using ex_TableDataDashboard.Models;
using ex_TableDataDashboard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace ex_TableDataDashboard.Services
{
    public class DashboardService
    {
        private readonly IDatabaseRepository _repository;

        public DashboardService(IDatabaseRepository repository)
        {
            _repository = repository;
        }

        public SqlConnection GetConnection(string server, string uid, string pwd, string db = "master")
        {
            return new SqlConnection($"Server={server};Database={db};User Id={uid};Password={pwd};TrustServerCertificate=True;");
        }

        public DashboardViewModel LoadDashboard(string server, string uid, string pwd, string selectedDb = null, string selectedTable = null)
        {
            var model = new DashboardViewModel();

            var baseConn = GetConnection(server, uid, pwd);
            model.Databases = _repository.GetDatabases(baseConn);

            if (!string.IsNullOrEmpty(selectedDb))
            {
                model.SelectedDatabase = selectedDb;
                var dbConn = GetConnection(server, uid, pwd, selectedDb);
                model.Tables = _repository.GetTables(dbConn);

                if (!string.IsNullOrEmpty(selectedTable))
                {
                    model.SelectedTable = selectedTable;
                    model.TableData = _repository.GetTableData(dbConn, selectedTable);
                    model.AlertMessage = $"Fetched data from table: {selectedTable}";
                }
            }

            return model;
        }
    }
}
