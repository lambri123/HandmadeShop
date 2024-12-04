using System.ComponentModel.DataAnnotations;

namespace HandmadeShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]  // Името на продукта е задължително
        [MaxLength(100)]  // Максимална дължина 100 символа
        public string Name { get; set; }

        [Required]  // Описание на продукта е задължително
        [MaxLength(500)]  // Максимална дължина 500 символа
        public string Description { get; set; }

        [Required]  // Цената е задължителна
        [Range(0.01, 10000.00)]  // Обхват на цената
        public decimal Price { get; set; }

        [Required]  // Наличността  е задължителна
        [Range(0, 1000)]  // Максимум 1000 налични броя
        public int Stock { get; set; }

        [Required]  // Категорията е задължителна
        [MaxLength(100)]  // Максимална дължина 100 символа
        public string Category { get; set; }
    }
}
