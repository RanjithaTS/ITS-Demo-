using Microsoft.AspNetCore.Mvc;

namespace Inventory_CSI.Controllers
{
    public class PartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
