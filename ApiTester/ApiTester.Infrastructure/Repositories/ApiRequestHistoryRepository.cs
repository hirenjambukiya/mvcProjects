using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTester.Domain.Entities;
using ApiTester.Domain.Repositories;
using ApiTester.Infrastructure.Data;
using Dapper;

namespace ApiTester.Infrastructure.Repositories
{
    public class ApiRequestHistoryRepository : IApiRequestHistoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ApiRequestHistoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddAsync(ApiRequestHistory history)
        {
            var query = @"
                INSERT INTO ApiRequestHistory 
                (ApiUrl, HttpMethod, RequestHeaders, RequestJson, ResponseJson, StatusCode, ResponseTime, IsSuccess, ErrorMessage, CreatedDate)
                VALUES 
                (@ApiUrl, @HttpMethod, @RequestHeaders, @RequestJson, @ResponseJson, @StatusCode, @ResponseTime, @IsSuccess, @ErrorMessage, @CreatedDate);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QuerySingleAsync<int>(query, history);
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var query = "DELETE FROM ApiRequestHistory WHERE Id = @Id";
            using (var connection = _connectionFactory.CreateConnection())
            {
                var rowsAffected = await connection.ExecuteAsync(query, new { Id = id });
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<ApiRequestHistory>> GetAllAsync()
        {
            var query = "SELECT * FROM ApiRequestHistory ORDER BY CreatedDate DESC";
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<ApiRequestHistory>(query);
            }
        }

        public async Task<ApiRequestHistory> GetByIdAsync(int id)
        {
            var query = "SELECT * FROM ApiRequestHistory WHERE Id = @Id";
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ApiRequestHistory>(query, new { Id = id });
            }
        }

        public async Task<int> GetTotalCallsAsync()
        {
            var query = "SELECT COUNT(Id) FROM ApiRequestHistory";
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<int> GetSuccessCountAsync()
        {
            var query = "SELECT COUNT(Id) FROM ApiRequestHistory WHERE IsSuccess = 1";
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<int> GetFailedCountAsync()
        {
            var query = "SELECT COUNT(Id) FROM ApiRequestHistory WHERE IsSuccess = 0";
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<double> GetAverageResponseTimeAsync()
        {
            var query = "SELECT ISNULL(AVG(CAST(ResponseTime AS float)), 0) FROM ApiRequestHistory WHERE IsSuccess = 1";
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<double>(query);
            }
        }

        public async Task<IEnumerable<ApiRequestHistory>> GetRecentCallsAsync(int count)
        {
            var query = "SELECT TOP (@Count) * FROM ApiRequestHistory ORDER BY CreatedDate DESC";
            using (var connection = _connectionFactory.CreateConnection())
            {
                return await connection.QueryAsync<ApiRequestHistory>(query, new { Count = count });
            }
        }
    }
}
