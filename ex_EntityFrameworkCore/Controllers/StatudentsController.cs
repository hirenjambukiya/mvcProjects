using Microsoft.AspNetCore.Mvc;

namespace ex_EntityFrameworkCore.Controllers
{
    public class StatudentsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
