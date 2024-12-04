namespace HandmadeShop.Models
{
    public class Order
    {
        public int OrderID { get; set; } // Primary Key
        public int UserID { get; set; }  // Връзка към User
        public int ProductID { get; set; }  // Връзка към Product
        public int Quantity { get; set; }  // Количество
        public decimal TotalPrice { get; set; }  // Обща цена
        public DateTime OrderDate { get; set; } = DateTime.Now; // Дата на поръчката
    }
}

