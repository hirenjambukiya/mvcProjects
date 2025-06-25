using ex_TableDataDashboard.Models;
using ex_TableDataDashboard.Repositories.Interfaces;
using ex_TableDataDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace ex_TableDataDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardService _service;
        private readonly IDatabaseRepository _databaseRepository;

        public DashboardController(DashboardService service,IDatabaseRepository databaseRepository)
        {
            _service = service;
            _databaseRepository = databaseRepository;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(SqlLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            TempData["Server"] = model.Server;
            TempData["UserId"] = model.UserId;
            TempData["Password"] = model.Password;

            return RedirectToAction("Dashboard");
        }
        
        public IActionResult Dashboard(string selectedDb = null)
        {
            try
            {
                var server = TempData["Server"]?.ToString();
                var userId = TempData["UserId"]?.ToString();
                var password = TempData["Password"]?.ToString();

                if (server == null || userId == null || password == null)
                    return RedirectToAction("Login");

                TempData.Keep(); // Keep connection info

                var conn = _service.GetConnection(server, userId, password);
                var model = new DashboardViewModel
                {
                    Databases = _databaseRepository.GetDatabases(conn)
                };

                if (!string.IsNullOrEmpty(selectedDb))
                {
                    model.SelectedDatabase = selectedDb;
                    var dbConn = _service.GetConnection(server, userId, password, selectedDb);
                    model.Tables = _databaseRepository.GetTables(dbConn);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error loading dashboard: {ex.Message}";
                return View(new DashboardViewModel());
            }
        }

        [HttpPost]
        public IActionResult Search(DashboardViewModel model)
        {
            try
            {
                var server = TempData["Server"]?.ToString();
                var userId = TempData["UserId"]?.ToString();
                var password = TempData["Password"]?.ToString();

                TempData.Keep();

                if (!ModelState.IsValid)
                    return RedirectToAction("Dashboard", new { selectedDb = model.SelectedDatabase });

                var conn = _service.GetConnection(server, userId, password, model.SelectedDatabase);

                model.Databases = _databaseRepository.GetDatabases(_service.GetConnection(server, userId, password));
                model.Tables = _databaseRepository.GetTables(conn);
                model.TableData = _databaseRepository.GetTableData(conn, model.SelectedTable);
                model.AlertMessage = $"Data fetched from {model.SelectedTable} table.";

                return View("Dashboard", model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Search failed: {ex.Message}";
                return RedirectToAction("Dashboard");
            }
        }
    }
}
