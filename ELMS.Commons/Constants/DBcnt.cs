using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELMS.Commons.Constants
{
    public class DBcnt
    {
        #region SP Names
        public const string usp_GetUserByUserName = "usp_GetUserByUserName";
        public const string usp_Insert_Upadte_Users = "usp_Insert_Upadte_Users";
        public const string usp_GetLeaveSummary = "usp_GetLeaveSummary";
        public const string usp_GetLeaveSummarybyUserId = "usp_GetLeaveSummarybyUserId";
        public const string usp_Insert_Update_Leaveapplication = "usp_Insert_Update_Leaveapplication";
        public const string usp_GetLeaveList = "usp_GetLeaveList";
        public const string usp_GetLeaveById = "usp_GetLeaveById";
        public const string usp_GetEmployeeLeaveList = "usp_GetEmployeeLeaveList";
        public const string usp_UpdateLeaveStatus = "usp_UpdateLeaveStatus";

        #endregion
    }
}
