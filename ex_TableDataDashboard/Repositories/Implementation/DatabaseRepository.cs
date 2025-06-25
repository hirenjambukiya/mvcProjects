using ex_TableDataDashboard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ex_TableDataDashboard.Repositories.Implementation
{
    public class DatabaseRepository : IDatabaseRepository
    {
        public List<string> GetDatabases(SqlConnection conn)
        {
            var databases = new List<string>();
            try
            {
                conn.Open();
                using var cmd = new SqlCommand("SELECT name FROM sys.databases", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    databases.Add(reader.GetString(0));
            }
            finally { conn.Close(); }
            return databases;
        }

        public List<string> GetTables(SqlConnection conn)
        {
            var tables = new List<string>();
            try
            {
                conn.Open();
                using var cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                    tables.Add(reader.GetString(0));
            }
            finally { conn.Close(); }
            return tables;
        }

        public DataTable GetTableData(SqlConnection conn, string table)
        {
            var dt = new DataTable();
            try
            {
                conn.Open();
                var cmd = new SqlCommand($"SELECT * FROM [{table}] ORDER BY 1 DESC", conn);
                var adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            finally { conn.Close(); }
            return dt;
        }
    }
}
