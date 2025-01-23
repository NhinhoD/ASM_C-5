using System.ComponentModel.DataAnnotations;

namespace ASM_C_4.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập UserName")]
        public string Username { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập PassWord")]
		public string Password { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Lỗi Email")]
		public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string address { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }
    }
}
