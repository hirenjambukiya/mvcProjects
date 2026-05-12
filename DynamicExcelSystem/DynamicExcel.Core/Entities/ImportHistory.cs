using System;

namespace DynamicExcel.Core.Entities
{
    public class ImportHistory
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int ConnectionId { get; set; }
        public string ConnectionName { get; set; }
        public DateTime ImportDate { get; set; }
        public int TotalSheets { get; set; }
        public int TotalRecords { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public double ExecutionTimeSeconds { get; set; }
    }
}
