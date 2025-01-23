namespace ASM_C_4.Models
{
    public class ComboProduct
    {
        public long ComboId { get; set; }
        public Combo Combo { get; set; }

        public long ProductId { get; set; }
        public ProductModel? Product { get; set; }
    }
}
