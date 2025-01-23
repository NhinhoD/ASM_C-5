using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_4.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OrderCode { get; set; }
        public long ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsCombo { get; set; } // Đánh dấu đây là sản phẩm combo

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
