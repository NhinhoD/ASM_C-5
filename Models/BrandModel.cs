using System.ComponentModel.DataAnnotations;

namespace ASM_C_4.Models
{
	public class BrandModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập tên thương hiệu")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập mô tả")]
		public string Description { get; set; }
		public string Slug { get; set; }
		public int Status { get; set; }

	}
}
