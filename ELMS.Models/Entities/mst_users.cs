using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELMS.Commons.Enums;

namespace ELMS.Models.Entities
{
    public class mst_users: baseEntity
    {
        public Int64 UserId { get; set; }
        public required string FirtsName { get; set; }
        public required string LastName { get; set; }
        public Roles RoleId { get; set; }
        public string? EmailAddress { get; set; }
        public required string Password { get; set; }
        public Boolean IsActive { get; set; }
    }
}
