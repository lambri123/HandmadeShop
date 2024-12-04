namespace HandmadeShop.Models
{
    public class User
    {
        public int UserID { get; set; } // Primary Key
        public string Username { get; set; } // Потребителско име
        public string PasswordHash { get; set; } // Хеширана парола
        public string Email { get; set; } // Имейл адрес
        public string Role { get; set; } // Роля: Guest, User, Admin
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Дата на регистрация
    }
}
