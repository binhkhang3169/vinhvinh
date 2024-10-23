using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WEBK.Models;

namespace WEBK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormApiController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public FormApiController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: api/formapi
        [HttpGet]
        public async Task<ActionResult<List<Form>>> GetAllForms()
        {
            var response = await _httpClient.GetStringAsync("https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/.json");
            var formsDict = JsonConvert.DeserializeObject<Dictionary<string, Form>>(response);
            return Ok(formsDict?.Values.ToList() ?? new List<Form>());
        }

        // GET: api/formapi/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Form>> GetForm(string id)
        {
            var response = await _httpClient.GetStringAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/{id}.json");
            var form = JsonConvert.DeserializeObject<Form>(response);
            if (form == null)
            {
                return NotFound(new { message = "Form not found." });
            }
            return Ok(form);
        }

        // POST: api/formapi
        [HttpPost]
        public async Task<ActionResult<Form>> CreateForm(Form form)
        {
            // Tạo một ID cục bộ cho form
            form.Id = Guid.NewGuid().ToString();

            // Gửi yêu cầu POST đến Firebase
            var response = await _httpClient.PostAsJsonAsync("https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/.json", form);

            // Kiểm tra xem yêu cầu có thành công không
            if (response.IsSuccessStatusCode)
            {
                // Lấy ID được tạo ra bởi Firebase
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var firebaseResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                string firebaseId = firebaseResponse["name"]; // ID do Firebase tạo ra

                // Cập nhật FirebaseId trong form
                form.Id = firebaseId;
                await UpdateForm(firebaseId, form);

                // Trả về 201 Created cùng với thông tin form
                return CreatedAtAction(nameof(GetForm), new { id = firebaseId }, form);
            }

            // Trả về lỗi nếu không thành công
            return BadRequest(new { message = "Failed to create the form." });
        }


        // PUT: api/formapi/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateForm(string id, Form form)
        {
            form.Id = id; // Đảm bảo ID của form được thiết lập đúng
            var response = await _httpClient.PutAsJsonAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/{id}.json", form);
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            return NotFound(new { message = "Form not found." });
        }

        // DELETE: api/formapi/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForm(string id)
        {
            var response = await _httpClient.DeleteAsync($"https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/{id}.json");
            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }
            return NotFound(new { message = "Form not found." });
        }
    }
}
