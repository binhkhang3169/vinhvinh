using Microsoft.AspNetCore.Mvc;

namespace WEBK.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet("/admin")]
        public IActionResult Index()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Login");
            }

            return View();
        }
        // POST: /admin/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Xóa session key
            HttpContext.Session.Remove("UserEmail");

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Login");
        }
    }
}