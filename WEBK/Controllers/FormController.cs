using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WEBK.Models;

namespace WEBK.Controllers
{
    [Route("form")] // Thiết lập đường dẫn chính cho controller
    public class FormController : Controller
    {
        private readonly HttpClient _httpClient;

        public FormController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: form
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            var response = await _httpClient.GetStringAsync("https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from.json");
            var formsDict = JsonConvert.DeserializeObject<Dictionary<string, Form>>(response);
            var forms = formsDict?.Values.ToList() ?? new List<Form>();
            return View(forms);
        }

        // GET: form/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            var response = await _httpClient.GetStringAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from/{id}.json");
            var form = JsonConvert.DeserializeObject<Form>(response);
            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }

        // GET: form/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        // POST: form/create
        [HttpPost("create")]
        public async Task<IActionResult> Create(Form form)
        {
            form.Id = Guid.NewGuid().ToString();
            var response = await _httpClient.PostAsJsonAsync("https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from.json", form);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var firebaseResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                string firebaseId = firebaseResponse["name"];
                form.Id = firebaseId;
                await Edit(firebaseId, form);
                return RedirectToAction(nameof(Details), new { id = firebaseId });
            }

            ModelState.AddModelError(string.Empty, "Failed to create the form.");
            return View(form);
        }

        // GET: form/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            var response = await _httpClient.GetStringAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from/{id}.json");
            var form = JsonConvert.DeserializeObject<Form>(response);
            if (form == null)
            {
                return NotFound();
            }
            return View(form);
        }

        // POST: form/edit/{id}
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(string id, Form form)
        {
            if (id != form.Id)
            {
                return BadRequest();
            }

            var response = await _httpClient.PutAsJsonAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from/{id}.json", form);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return NotFound(new { message = "Form not found." });
        }

        // GET: form/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            var response = await _httpClient.DeleteAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from/{id}.json");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound(new { message = "Form not found." });
        }
        // GET: form/accept/{id}
        [HttpGet("accept/{id}")]
        public async Task<IActionResult> Accept(string id)
        {
            // Kiểm tra nếu chưa đăng nhập
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }

            // Lấy form từ Firebase dựa trên id
            var response = await _httpClient.GetStringAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from/{id}.json");
            var form = JsonConvert.DeserializeObject<Form>(response);

            // Nếu form không tồn tại
            if (form == null)
            {
                return NotFound(new { message = "Form not found." });
            }

            // Cập nhật thuộc tính Accept thành true
            form.Accept = true;

            // Gửi yêu cầu cập nhật form lên Firebase
            var updateResponse = await _httpClient.PutAsJsonAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/from/{id}.json", form);

            // Kiểm tra xem cập nhật có thành công không
            if (updateResponse.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index)); // Chuyển hướng về trang danh sách
            }

            return StatusCode(500, new { message = "Failed to update form." });
        }
        private bool Check()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return false;
            }

            return true;
        }
    }
}
