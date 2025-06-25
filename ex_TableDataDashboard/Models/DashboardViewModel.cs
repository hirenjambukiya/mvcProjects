using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ex_TableDataDashboard.Models
{
    public class DashboardViewModel
    {
        public List<string> Databases { get; set; } = new();
        public List<string> Tables { get; set; } = new();

        [Required]
        public string SelectedDatabase { get; set; }

        [Required]
        public string SelectedTable { get; set; }

        public DataTable? TableData { get; set; }
        public string? AlertMessage { get; set; }
    }

}
