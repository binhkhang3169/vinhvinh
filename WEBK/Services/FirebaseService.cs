using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WEBK.Models;

namespace WEBK.Services
{
    public class FirebaseService
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseService(string basePath, string secret)
        {
            _firebaseClient = new FirebaseClient(basePath, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(secret)
            });
        }

        // Thêm mới Form đăng ký rác thải
        public async Task AddFormAsync(Form form)
        {
            await _firebaseClient.Child("Forms").PostAsync(form);
        }

        // Lấy tất cả các Form đã đăng ký
        public async Task<List<Form>> GetAllFormsAsync()
        {
            var forms = await _firebaseClient.Child("Forms").OnceAsync<Form>();
            return forms.Select(f => new Form
            {
                Id = f.Key,
                FullName = f.Object.FullName,
                Address = f.Object.Address,
                WasteType = f.Object.WasteType,
                Quantity = f.Object.Quantity
            }).ToList();
        }

        // Cập nhật Form dựa trên ID
        public async Task UpdateFormAsync(string id, Form form)
        {
            await _firebaseClient.Child("Forms").Child(id).PutAsync(form);
        }

        // Xóa Form dựa trên ID
        public async Task DeleteFormAsync(string id)
        {
            await _firebaseClient.Child("Forms").Child(id).DeleteAsync();
        }
    }
}
