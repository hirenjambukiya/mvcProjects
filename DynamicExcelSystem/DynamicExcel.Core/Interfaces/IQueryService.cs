using DynamicExcel.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamicExcel.Core.Interfaces
{
    public interface IQueryService
    {
        Task<QueryResult> ExecuteQueryAsync(DatabaseConnection connection, string rawSql, int start, int length);
    }

    public class QueryResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Columns { get; set; } = new List<string>();
        public List<Dictionary<string, object>> Data { get; set; } = new List<Dictionary<string, object>>();
        public int TotalRecords { get; set; }
        public double ExecutionTimeSeconds { get; set; }
    }
}
