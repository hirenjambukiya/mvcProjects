using ex_EMSWithAJAX.Models;

namespace ex_EMSWithAJAX.Services
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetAll();

        Employee? GetById(int employeeId);

        bool CheckEmail(string email, int employeeId = 0);

        int Insert(Employee employee);

        bool Update(Employee employee);

        bool Delete(int employeeId);

        IEnumerable<Country> GetCountries();

        IEnumerable<State> GetStatesByCountry(int countryId);

        IEnumerable<City> GetCitiesByState(int stateId);
    }
}
