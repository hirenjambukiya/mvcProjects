using ex_EMSWithAJAX.Models;
using ex_EMSWithAJAX.Services;
using ex_EMSWithAJAX.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ex_EMSWithAJAX.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEmployeeList()
        {
            var employees = _employeeService.GetAll();

            return Json(employees);
        }

        [HttpGet]
        public IActionResult GetEmployeeForm()
        {
            var model = new EmployeeViewModel();

            model.Countries = _employeeService.GetCountries();

            return PartialView("_EmployeeForm", model);
        }

        [HttpGet]
        public IActionResult GetStates(int countryId)
        {
            var states = _employeeService.GetStatesByCountry(countryId);

            return Json(states);
        }
        [HttpGet]
        public IActionResult GetCities(int stateId)
        {
            var cities = _employeeService.GetCitiesByState(stateId);

            return Json(cities);
        }

        [HttpPost]
        public IActionResult SaveEmployee([FromBody] EmployeeSaveViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Please correct the validation errors."
                });
            }

            if (model.DateOfBirth.Value.Date > DateTime.Today)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Date of birth cannot be in the future."
                });
            }
            int diff = DateTime.Now.Year - model.DateOfBirth.Value.Year;
            if (diff < 18)
            {
                return Conflict(new
                {
                    success = false,
                    message = "Under 18 not allowed."
                });
            }
            if (_employeeService.CheckEmail(model.Email,model.EmployeeId))
            {
                return Conflict(new
                {
                    success = false,
                    message = "Email already exists."
                });
            }
            try
            {
                var employee = new Employee
                {
                    EmployeeId = model.EmployeeId,
                    Name = model.Name,
                    Gender = model.Gender,
                    Email = model.Email,
                    DateOfBirth = model.DateOfBirth!.Value,
                    Salary = model.Salary!.Value,
                    Address = model.Address,
                    CountryId = model.CountryId!.Value,
                    StateId = model.StateId!.Value,
                    CityId = model.CityId!.Value
                };

                var employeeId = _employeeService.Insert(employee);

                return Ok(new
                {
                    success = true,
                    message = "Employee saved successfully.",
                    employeeId
                });
            }
            catch (Exception ex)
            {
                // Log ex in a real application.

                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while saving the employee."
                });
            }
        }

        [HttpGet]

        public IActionResult CheckEmail(string email, int employeeId)
        {
            bool isexist = _employeeService.CheckEmail(email);

            if (isexist)
            {
                return Json(new { exists = true });
            }
            return Json(new { exits = false });
        }
    }
}
