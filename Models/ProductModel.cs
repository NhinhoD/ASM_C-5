using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ASM_C_4.Repository.Validation;

namespace ASM_C_4.Models
{
    public class ProductModel
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mô tả")]
        [MinLength(4, ErrorMessage = "Mô tả phải có ít nhất 4 ký tự")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; } 

        [Required(ErrorMessage = "Chọn một thương hiệu")]
        [Range(1, int.MaxValue, ErrorMessage = "Chọn một thương hiệu hợp lệ")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Chọn một danh mục")]
        [Range(1, int.MaxValue, ErrorMessage = "Chọn một danh mục hợp lệ")]
        public int CategoryId { get; set; }

        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }

        public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
