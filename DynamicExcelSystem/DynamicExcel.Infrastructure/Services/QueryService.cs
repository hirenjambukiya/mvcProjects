using Dapper;
using DynamicExcel.Core.Entities;
using DynamicExcel.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicExcel.Infrastructure.Services
{
    public class QueryService : IQueryService
    {
        public async Task<QueryResult> ExecuteQueryAsync(DatabaseConnection connection, string rawSql, int start, int length)
        {
            var result = new QueryResult();
            var sw = Stopwatch.StartNew();

            try
            {
                // Basic Security check
                if (IsDestructiveQuery(rawSql))
                {
                    result.Success = false;
                    result.Message = "Security Error: Destructive statements (DROP, DELETE, TRUNCATE, UPDATE, INSERT) are not allowed in this interface.";
                    return result;
                }

                var connectionString = connection.GetConnectionString();
                using var sqlConnection = new SqlConnection(connectionString);
                await sqlConnection.OpenAsync();

                // 1. Get Total Count
                // We wrap the user's query as a subquery to count total records
                var countSql = $"SELECT COUNT(1) FROM ({rawSql}) AS CountWrapper";
                result.TotalRecords = await sqlConnection.ExecuteScalarAsync<int>(countSql);

                // 2. Fetch Paginated Data
                // If length is -1, fetch all
                string pagedSql = rawSql;
                if (length > 0)
                {
                    // To do OFFSET FETCH, the query MUST have an ORDER BY.
                    // If the user didn't provide one, we must add a default one or inject it.
                    // A safe way for SQL Server >= 2012 is to ORDER BY (SELECT NULL) if no order by exists.
                    if (!rawSql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
                    {
                        pagedSql = $"SELECT * FROM ({rawSql}) AS PagedWrapper ORDER BY (SELECT NULL) OFFSET {start} ROWS FETCH NEXT {length} ROWS ONLY";
                    }
                    else
                    {
                        // If it has order by but we wrap it, SQL Server complains if ORDER BY is in subquery without TOP.
                        // So we just append offset/fetch directly assuming it's a simple query
                        pagedSql = $"{rawSql} OFFSET {start} ROWS FETCH NEXT {length} ROWS ONLY";
                    }
                }

                // Dapper returns dynamic objects (IDictionary<string, object>)
                var rows = await sqlConnection.QueryAsync<dynamic>(pagedSql);
                
                var dataList = rows.Select(x => (IDictionary<string, object>)x).ToList();
                
                if (dataList.Any())
                {
                    result.Columns = dataList.First().Keys.ToList();
                    
                    foreach (var row in dataList)
                    {
                        var dict = new Dictionary<string, object>();
                        foreach (var kvp in row)
                        {
                            dict[kvp.Key] = kvp.Value == null ? string.Empty : kvp.Value.ToString();
                        }
                        result.Data.Add(dict);
                    }
                }

                result.Success = true;
                result.Message = "Query executed successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Query Error: {ex.Message}";
            }

            sw.Stop();
            result.ExecutionTimeSeconds = sw.Elapsed.TotalSeconds;

            return result;
        }

        private bool IsDestructiveQuery(string sql)
        {
            var upperSql = sql.ToUpperInvariant();
            var destructiveKeywords = new[] { "DROP", "DELETE", "TRUNCATE", "UPDATE", "INSERT", "ALTER", "EXEC", "EXECUTE" };
            
            // Regex to check for whole words
            foreach (var keyword in destructiveKeywords)
            {
                if (Regex.IsMatch(upperSql, $@"\b{keyword}\b"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
