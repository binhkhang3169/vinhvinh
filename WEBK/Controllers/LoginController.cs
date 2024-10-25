using Microsoft.AspNetCore.Mvc;
using WEBK.Services;

namespace WEBK.Controllers
{
    public class LoginController : Controller
    {
        private readonly FirebaseAuthService _firebaseAuthService;

        public LoginController(FirebaseAuthService firebaseAuthService)
        {
            _firebaseAuthService = firebaseAuthService;
        }

        // GET: /login
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /login
        [HttpPost("/login")]
        public async Task<IActionResult> Login(string authKey)
        {
            var user = await _firebaseAuthService.GetUserByAuthKeyAsync(authKey);

            if (user != null)
            {
                // Lưu thông tin người dùng vào session hoặc cookie
                HttpContext.Session.SetString("UserEmail", user.Email);

                // Chuyển hướng đến trang admin sau khi đăng nhập thành công
                return RedirectToAction("Index", "Admin");
            }

            ModelState.AddModelError(string.Empty, "Invalid authentication key.");
            return View();
        }
    }
}
