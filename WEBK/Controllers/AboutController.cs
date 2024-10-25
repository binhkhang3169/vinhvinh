using Microsoft.AspNetCore.Mvc;

namespace WEBK.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
