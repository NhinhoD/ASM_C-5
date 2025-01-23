namespace ASM_C_4.Models
{
	public class CartItemModel
	{
		public long ProductId { get; set; }
		public string ProductName { get; set; }
		public string Image {  get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
        public bool IsCombo { get; set; } // Đánh dấu đây là sản phẩm combo
        public decimal Total
		{
			get { return Quantity * Price; }
		}

		public CartItemModel() {

		}

		public CartItemModel(ProductModel product)
		{
			ProductId = product.Id;
			ProductName = product.Name;
			Price = product.Price;
			Quantity = 1;
            IsCombo = false;
            Image = product.Image;

		}

        public CartItemModel(Combo combo)
        {
            ProductId = combo.Id;
            ProductName = combo.Name;
            Price = combo.Price;
            IsCombo = true;
            Quantity = 1;
            Image = combo.Image;

        }
    }
}
