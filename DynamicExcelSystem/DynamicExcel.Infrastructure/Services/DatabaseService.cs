using DynamicExcel.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;

namespace DynamicExcel.Infrastructure.Services
{
    public class DatabaseService : IDatabaseService
    {
        public bool TestConnection(string connectionString, out string errorMessage)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    errorMessage = string.Empty;
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
    }
}
