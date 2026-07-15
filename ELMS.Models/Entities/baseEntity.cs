using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Models.Entities
{
    public  class baseEntity
    {
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; }
        public Boolean IsDeleted { get; set; } = false;
    }
}
