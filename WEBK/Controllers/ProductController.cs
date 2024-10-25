using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WEBK.Models;

namespace WEBK.Controllers
{
    [Route("product")]
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: product
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetStringAsync("https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/products.json");
            var productsDict = JsonConvert.DeserializeObject<Dictionary<string, Product>>(response);
            var products = productsDict?.Values.ToList() ?? new List<Product>();
            var isChecked = Check(); // Gọi phương thức Check() để lấy giá trị

            return View(Tuple.Create(products, isChecked)); // Gửi Tuple đến view
        }


        // GET: product/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var response = await _httpClient.GetStringAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/products/{id}.json");
            var product = JsonConvert.DeserializeObject<Product>(response);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: product/create
        [HttpGet("create")]
        public IActionResult Create()
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }

        // POST: product/create
        [HttpPost("create")]
        public async Task<IActionResult> Create(Product product)
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            product.Id = Guid.NewGuid().ToString();
            var response = await _httpClient.PostAsJsonAsync("https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/products.json", product);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var firebaseResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                string firebaseId = firebaseResponse["name"];
                product.Id = firebaseId;
                await Edit(firebaseId,product);
                return RedirectToAction(nameof(Details), new { id = firebaseId });
            }

            ModelState.AddModelError(string.Empty, "Failed to create the product.");
            return View(product);
        }

        // GET: product/edit/{id}
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            var response = await _httpClient.GetStringAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/products/{id}.json");
            var product = JsonConvert.DeserializeObject<Product>(response);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: product/edit/{id}
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(string id, Product product)
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            if (id != product.Id)
            {
                return BadRequest();
            }

            var response = await _httpClient.PutAsJsonAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/products/{id}.json", product);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Details), new { id });
            }

            return NotFound(new { message = "Product not found." });
        }

        // GET: product/delete/{id}
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Check())
            {
                return RedirectToAction("Login", "Login");
            }
            var response = await _httpClient.DeleteAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/products/{id}.json");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound(new { message = "Product not found." });
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