using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Commons.Enums;

namespace ELMS.Models.Entities
{
    public class mst_roles: baseEntity
    {
        public Int64 RoleId { get; set; }
        public Roles RoleType { get; set; }
        public string? Description { get; set; }
    }
}
