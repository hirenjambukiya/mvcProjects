using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ELMS.Commons.Constants;
using ELMS.Models.DTOs;
using ELMS.Models.Entities;
using ELMS.Services.Interfaces;
using Newtonsoft.Json.Serialization;

namespace ELMS.Services.Repositories
{
    public class LeaveRepository : ILeave
    {
        private readonly IDBManager _dbManager;
        public IDbConnection dbConnection;
        public LeaveRepository(IDBManager dBManager)
        {
            _dbManager = dBManager;
            dbConnection = _dbManager.CreateConnection("DefaultConnection");
        }
        public void ApplyLeave(tbl_leaveapplication tbl_Leaveapplication)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@LeaveId", tbl_Leaveapplication.LeaveId);
                dynamicParameters.Add("@UserId", tbl_Leaveapplication.UserId);
                dynamicParameters.Add("@LeaveStatusId", tbl_Leaveapplication.LeaveStatusId);
                dynamicParameters.Add("@LeaveType", tbl_Leaveapplication.LeaveType);
                dynamicParameters.Add("@StartDate", tbl_Leaveapplication.StartDate);
                dynamicParameters.Add("@EndDate", tbl_Leaveapplication.EndDate);
                dynamicParameters.Add("@Reason", tbl_Leaveapplication.Reason);
                dynamicParameters.Add("@AttachedFileName", tbl_Leaveapplication.AttachedFileName);
                dbConnection.Execute(DBcnt.usp_Insert_Update_Leaveapplication,dynamicParameters,commandType:CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public DataTableResponse<LeaveListDto> GetLeavebyUserid(long UserId, string? Search = null, int PageNumber = 1, int PageSize = 10)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", UserId);
                dynamicParameters.Add("@Search", Search);
                dynamicParameters.Add("@PageNumber", PageNumber);
                dynamicParameters.Add("@PageSize", PageSize);
                var data = dbConnection.QueryMultiple(DBcnt.usp_GetLeaveList, dynamicParameters, commandType: CommandType.StoredProcedure);

                List<LeaveListDto> leaveList = data.Read<LeaveListDto>().ToList();
                int totalRecords = data.ReadFirst<int>();
                    
                return new DataTableResponse<LeaveListDto>
                {
                    RecordsTotal = totalRecords,
                    RecordsFiltered = totalRecords,
                    Data = leaveList
                };

            }
            catch (Exception)
            {

                throw;
            }
        }

        public LeaveForm GetLeaveById(long leaveId)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@LeaveId", leaveId);

            return dbConnection.QueryFirstOrDefault<LeaveForm>(DBcnt.usp_GetLeaveById,parameter,commandType: CommandType.StoredProcedure);
        }
    }
}
