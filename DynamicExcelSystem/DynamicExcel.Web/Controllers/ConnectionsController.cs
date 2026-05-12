using DynamicExcel.Core.Entities;
using DynamicExcel.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicExcel.Web.Controllers
{
    public class ConnectionsController : Controller
    {
        private readonly IDatabaseConnectionRepository _repository;
        private readonly IDatabaseService _databaseService;

        public ConnectionsController(IDatabaseConnectionRepository repository, IDatabaseService databaseService)
        {
            _repository = repository;
            _databaseService = databaseService;
        }

        public IActionResult Index()
        {
            var connections = _repository.GetAll();
            return View(connections);
        }

        [HttpGet]
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new DatabaseConnection { AuthenticationType = "SQL" });
            else
                return View(_repository.GetById(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrEdit(DatabaseConnection model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    _repository.Add(model);
                    TempData["SuccessMessage"] = "Connection added successfully.";
                }
                else
                {
                    _repository.Update(model);
                    TempData["SuccessMessage"] = "Connection updated successfully.";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult TestConnection([FromBody] DatabaseConnection model)
        {
            if (model == null) return Json(new { success = false, message = "Invalid data" });
            
            var connectionString = model.GetConnectionString();
            var success = _databaseService.TestConnection(connectionString, out string errorMessage);
            
            return Json(new { success = success, message = success ? "Connection Successful!" : errorMessage });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return Json(new { success = true, message = "Deleted successfully" });
        }

        [HttpPost]
        public IActionResult SetDefault(int id)
        {
            _repository.SetDefaultConnection(id);
            return Json(new { success = true, message = "Default connection updated" });
        }
    }
}
