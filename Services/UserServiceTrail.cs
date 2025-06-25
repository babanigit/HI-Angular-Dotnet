

using System.Text.Json;
using todo_web_api.Models;

namespace todo_web_api.Services
{
    public class UserServiceTrail
    {
        private readonly string _filePath = "data.json";

        public List<UserTrail> GetAll()
        {
            if (!File.Exists(_filePath)) return new List<UserTrail>();
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<UserTrail>>(json) ?? new List<UserTrail>();
        }

        public void SaveAll(List<UserTrail> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        public void Add(UserTrail user)
        {
            var users = GetAll();
            user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            SaveAll(users);
        }

        public UserTrail? FindByEmail(string email)
        {
            return GetAll().FirstOrDefault(u => u.Email == email);
        }

    }
}