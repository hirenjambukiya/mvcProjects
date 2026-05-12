using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DynamicExcel.Web.Models;

using DynamicExcel.Core.Interfaces;
using System.Linq;

namespace DynamicExcel.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IImportHistoryRepository _historyRepo;
    private readonly IDatabaseConnectionRepository _connectionRepo;

    public HomeController(ILogger<HomeController> logger, IImportHistoryRepository historyRepo, IDatabaseConnectionRepository connectionRepo)
    {
        _logger = logger;
        _historyRepo = historyRepo;
        _connectionRepo = connectionRepo;
    }

    public IActionResult Index()
    {
        var history = _historyRepo.GetAll().ToList();
        var connections = _connectionRepo.GetAll().ToList();

        ViewBag.TotalImports = history.Count;
        ViewBag.TotalRecords = history.Sum(x => x.TotalRecords);
        ViewBag.ActiveConnections = connections.Count;
        ViewBag.FailedImports = history.Count(x => !x.Success);
        ViewBag.RecentActivity = history.Take(5).ToList();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
