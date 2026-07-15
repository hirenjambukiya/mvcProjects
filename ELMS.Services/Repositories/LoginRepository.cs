using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using ELMS.Commons.Constants;
using ELMS.Models.Entities;
using ELMS.Services.Interfaces;

namespace ELMS.Services.Repositories
{
    public class LoginRepository : ILoging
    {
		private readonly IDBManager _dbManager;
        public IDbConnection _dbConnection;
        public LoginRepository(IDBManager dBManager)
        {
            _dbManager = dBManager;
            _dbConnection = _dbManager.CreateConnection("DefaultConnection");
        }
        public  mst_users GetUserByUserName(string userName)
        {
			try
			{
                
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserName",userName);

                var user =  _dbConnection.QueryFirstOrDefault<mst_users>(DBcnt.usp_GetUserByUserName,dynamicParameters,commandType:CommandType.StoredProcedure);
                return user;
            }
			catch (Exception)
			{

				throw;
			}
            finally
            {
                _dbConnection.Close();
            }
        }

        public void InsertUser(mst_users user)
        {
            try
            {
                var dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", user.UserId);
                dynamicParameters.Add("@FirtsName", user.FirtsName);
                dynamicParameters.Add("@LastName", user.LastName);
                dynamicParameters.Add("@RoleId", user.RoleId);
                dynamicParameters.Add("@EmailAddress", user.EmailAddress);
                dynamicParameters.Add("@Password", user.Password);
                var data = _dbConnection.Execute(DBcnt.usp_Insert_Upadte_Users, dynamicParameters, commandType: CommandType.StoredProcedure);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _dbConnection.Close();
            }
        }
    }
}
