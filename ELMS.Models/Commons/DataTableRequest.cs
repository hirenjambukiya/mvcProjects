using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Models.Commons
{
    public class DataTableRequest
    {
        public int Draw { get; set; }

        public int Start { get; set; }

        public int Length { get; set; }

        public DataTableSearch Search { get; set; } = new();

        public List<DataTableOrder> Order { get; set; } = new();

        public List<DataTableColumn> Columns { get; set; } = new();

    }
}
