using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ELMS.Commons.Constants;
using ELMS.Models.DTOs;
using ELMS.Services.Interfaces;
using static Dapper.SqlMapper;

namespace ELMS.Services.Repositories
{
    public class HRRepository : IHR
    {
        private readonly IDBManager _dBManager;
        public IDbConnection _dbConnection;
        public HRRepository(IDBManager dBManager)
        {
            _dBManager = dBManager;
            _dbConnection = _dBManager.CreateConnection("DefaultConnection");
        }
        public void ApproveleavebyId(Int64 LeaveId)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@LeaveId", LeaveId);
                dynamicParameters.Add("@ActionType", Commoncnt.Approved);
                _dbConnection.Execute(DBcnt.usp_UpdateLeaveStatus, dynamicParameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public DataTableResponse<LeaveListDto> GetEmployeesLeaveList(string? Search = null, int PageNumber = 1, int PageSize = 10)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Search", Search);
                dynamicParameters.Add("@PageNumber", PageNumber);
                dynamicParameters.Add("@PageSize", PageSize);
                var data = _dbConnection.QueryMultiple(DBcnt.usp_GetEmployeeLeaveList, dynamicParameters, commandType: CommandType.StoredProcedure);

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
        public LeaveSummaryDto GetLeaveSummary()
        {
            try
            {
                var data = _dbConnection.QueryFirstOrDefault<LeaveSummaryDto>(DBcnt.usp_GetLeaveSummary, commandType: CommandType.StoredProcedure);

                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void RejectleavebyId(long LeaveId, string HRComment)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@LeaveId", LeaveId);
                dynamicParameters.Add("@ActionType", Commoncnt.Rejected);
                dynamicParameters.Add("@HRComment", HRComment);
                _dbConnection.Execute(DBcnt.usp_UpdateLeaveStatus, dynamicParameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
