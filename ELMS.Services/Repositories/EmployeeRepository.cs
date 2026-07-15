using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ELMS.Commons.Constants;
using ELMS.Models.DTOs;
using ELMS.Services.Interfaces;

namespace ELMS.Services.Repositories
{
    public class EmployeeRepository : IEmployee
    {
        private readonly IDBManager _dBManager;
        public IDbConnection _dbConnection;

        public EmployeeRepository(IDBManager dBManager)
        {
            _dBManager = dBManager;
            _dbConnection = _dBManager.CreateConnection("DefaultConnection");
        }
        public LeaveSummaryDto GetLeaveSummaryByEmployeeId(Int64 UserId)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", UserId);

                var data = _dbConnection.QueryFirstOrDefault<LeaveSummaryDto>(DBcnt.usp_GetLeaveSummarybyUserId, dynamicParameters, commandType: CommandType.StoredProcedure);

                return data;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
