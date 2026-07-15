using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Commons.Enums
{
    //public static class Enums
    //{
    public enum Roles
    {
        Admin = 1,
        HR = 2,
        Employee = 3
    }

    public enum LeaveStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }

    public enum LeaveType
    {
        None = 0,
        PL = 1, // Personal Leave
        SL = 2, // Sick Leave
        EL = 3, // Early Leave
        LWP = 4, // Leave Without Pay
        LC = 5, // Late Coming
    }
    //}
}
