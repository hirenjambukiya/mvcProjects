using ClosedXML.Excel;
using DynamicExcel.Core.Entities;
using DynamicExcel.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicExcel.Infrastructure.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<ImportResult> ImportExcelAsync(Stream fileStream, string fileName, DatabaseConnection connection)
        {
            var result = new ImportResult();
            var sw = Stopwatch.StartNew();

            try
            {
                using var workbook = new XLWorkbook(fileStream);
                var connectionString = connection.GetConnectionString();

                using var sqlConnection = new SqlConnection(connectionString);
                await sqlConnection.OpenAsync();

                foreach (var sheet in workbook.Worksheets)
                {
                    if (sheet.IsEmpty()) continue;

                    var firstRow = sheet.FirstRowUsed();
                    if (firstRow == null) continue;

                    var datePrefix = DateTime.Now.ToString("yyyyMMdd");
                    var rawTableName = SanitizeName(sheet.Name);
                    var tableName = $"{datePrefix}_{rawTableName}";

                    var dt = new DataTable(tableName);
                    var columnNames = new List<string>();

                    // Read Headers
                    int colIndex = 1;
                    foreach (var cell in firstRow.CellsUsed())
                    {
                        var rawColName = cell.GetString();
                        var safeColName = string.IsNullOrWhiteSpace(rawColName) ? $"Column_{colIndex}" : SanitizeName(rawColName);
                        
                        // Handle duplicate columns
                        if (columnNames.Contains(safeColName))
                        {
                            safeColName = $"{safeColName}_{colIndex}";
                        }
                        
                        columnNames.Add(safeColName);
                        dt.Columns.Add(safeColName, typeof(string)); // Defaulting to string to handle empty/nulls safely
                        colIndex++;
                    }

                    // Build Create Table SQL
                    var createTableSql = BuildCreateTableSql(tableName, columnNames);

                    // Execute Create Table
                    using (var cmd = new SqlCommand(createTableSql, sqlConnection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }

                    // Read Data
                    var dataRows = sheet.RowsUsed().Skip(1); // Skip header
                    foreach (var row in dataRows)
                    {
                        var dataRow = dt.NewRow();
                        bool hasData = false;
                        
                        for (int i = 0; i < columnNames.Count; i++)
                        {
                            var val = row.Cell(i + 1).GetString();
                            dataRow[i] = string.IsNullOrEmpty(val) ? DBNull.Value : val;
                            if (!string.IsNullOrEmpty(val)) hasData = true;
                        }

                        if (hasData)
                        {
                            dt.Rows.Add(dataRow);
                        }
                    }

                    // Bulk Insert
                    if (dt.Rows.Count > 0)
                    {
                        using var bulkCopy = new SqlBulkCopy(sqlConnection);
                        bulkCopy.DestinationTableName = $"[{tableName}]";
                        foreach (var col in columnNames)
                        {
                            bulkCopy.ColumnMappings.Add(col, col);
                        }
                        await bulkCopy.WriteToServerAsync(dt);
                        result.TotalRecordsImported += dt.Rows.Count;
                    }

                    result.TotalSheetsProcessed++;
                }

                result.Success = true;
                result.Message = "Import completed successfully.";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Import failed: {ex.Message}";
                result.ErrorLogs.Add(ex.ToString());
            }

            sw.Stop();
            result.ExecutionTimeSeconds = sw.Elapsed.TotalSeconds;

            return result;
        }

        private string BuildCreateTableSql(string tableName, List<string> columns)
        {
            var sql = $"IF OBJECT_ID('[{tableName}]', 'U') IS NOT NULL DROP TABLE [{tableName}]; ";
            sql += $"CREATE TABLE [{tableName}] (";
            sql += "[Id] INT IDENTITY(1,1) PRIMARY KEY, ";

            foreach (var col in columns)
            {
                sql += $"[{col}] NVARCHAR(MAX) NULL, ";
            }

            sql += "[CreatedDate] DATETIME DEFAULT GETDATE(), ";
            sql += "[ImportBatchId] UNIQUEIDENTIFIER DEFAULT NEWID() ";
            sql += ");";

            return sql;
        }

        private string SanitizeName(string input)
        {
            // Remove invalid SQL characters
            return Regex.Replace(input, @"[^\w\d_]", "");
        }
    }
}
