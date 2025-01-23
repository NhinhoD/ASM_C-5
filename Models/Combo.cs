using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ASM_C_4.Repository.Validation;

namespace ASM_C_4.Models
{
    public class Combo
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "Tên Combo là bắt buộc")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Giá Combo là bắt buộc")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(8, 2)")]
        public decimal Price { get; set; } // Giá ưu đãi của combo

        public string Image { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }


        // Quan hệ nhiều-nhiều giữa Combo và Product
        public ICollection<ComboProduct>? ComboProducts { get; set; }
    }
}
