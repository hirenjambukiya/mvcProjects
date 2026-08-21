using System.Data;
using ex_EMSWithAJAX.Data;
using ex_EMSWithAJAX.Models;
using Dapper;

namespace ex_EMSWithAJAX.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DbConnectionFactory _connectionFactory;

        public EmployeeRepository(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Employee> GetAll()
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.Query<Employee>(
                "usp_Employee_GetAll",
                commandType: CommandType.StoredProcedure);
        }

        public Employee? GetById(int employeeId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QueryFirstOrDefault<Employee>(
                "usp_Employee_GetById",
                new
                {
                    EmployeeId = employeeId
                },
                commandType: CommandType.StoredProcedure);
        }

        public bool CheckEmail(string email, int employeeId = 0)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = connection.QueryFirstOrDefault<bool>(
                "usp_Employee_CheckEmail",
                new
                {
                    Email = email,
                    EmployeeId = employeeId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public int Insert(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.QuerySingle<int>(
                "usp_Employee_Insert",
                new
                {
                    employee.Name,
                    employee.Gender,
                    employee.Email,
                    employee.DateOfBirth,
                    employee.Salary,
                    employee.Address,
                    employee.CountryId,
                    employee.StateId,
                    employee.CityId
                },
                commandType: CommandType.StoredProcedure);
        }

        public bool Update(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = connection.QuerySingle<int>(
                "usp_Employee_Update",
                new
                {
                    employee.EmployeeId,
                    employee.Name,
                    employee.Gender,
                    employee.Email,
                    employee.DateOfBirth,
                    employee.Salary,
                    employee.Address,
                    employee.CountryId,
                    employee.StateId,
                    employee.CityId
                },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public bool Delete(int employeeId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rowsAffected = connection.QuerySingle<int>(
                "usp_Employee_Delete",
                new
                {
                    EmployeeId = employeeId
                },
                commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }

        public IEnumerable<Country> GetCountries()
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.Query<Country>(
                "usp_Country_GetAll",
                commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<State> GetStatesByCountry(int countryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.Query<State>(
                "usp_State_GetByCountry",
                new
                {
                    CountryId = countryId
                },
                commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<City> GetCitiesByState(int stateId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return connection.Query<City>(
                "usp_City_GetByState",
                new
                {
                    StateId = stateId
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
