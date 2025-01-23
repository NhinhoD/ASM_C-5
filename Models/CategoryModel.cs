using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASM_C_4.Repository.Validation;

namespace ASM_C_4.Models
{
	public class CategoryModel
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
		public string Name { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập mô tả danh mục")]
		public string Description { get; set; }
		public string Slug { get; set; }
		public int Status { get; set; }

	}
}
