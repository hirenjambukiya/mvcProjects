using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Models.Commons
{
    public class DataTableColumn
    {
        public string Data { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public bool Searchable { get; set; }

        public bool Orderable { get; set; }

    }
}
