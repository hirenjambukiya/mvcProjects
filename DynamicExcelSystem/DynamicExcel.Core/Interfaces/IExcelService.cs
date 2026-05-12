using DynamicExcel.Core.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DynamicExcel.Core.Interfaces
{
    public interface IExcelService
    {
        Task<ImportResult> ImportExcelAsync(Stream fileStream, string fileName, DatabaseConnection connection);
    }

    public class ImportResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int TotalSheetsProcessed { get; set; }
        public int TotalRecordsImported { get; set; }
        public List<string> ErrorLogs { get; set; } = new List<string>();
        public double ExecutionTimeSeconds { get; set; }
    }
}
