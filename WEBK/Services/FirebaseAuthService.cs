using Newtonsoft.Json;
using WEBK.Models;

namespace WEBK.Services
{
    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;

        public FirebaseAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<User> GetUserByAuthKeyAsync(string authKey)
        {
            var response = await _httpClient.GetStringAsync("https://webkk-8336a-default-rtdb.asia-southeast1.firebasedatabase.app/users.json");
            var users = JsonConvert.DeserializeObject<Dictionary<string, User>>(response);

            if (users != null)
            {
                foreach (var user in users.Values)
                {
                    if (user.AuthKey == authKey)
                    {
                        return user;
                    }
                }
            }

            return null; // Nếu không tìm thấy user với authKey tương ứng
        }
    }
}
