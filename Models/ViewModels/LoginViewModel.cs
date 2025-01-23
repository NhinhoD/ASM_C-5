using System.ComponentModel.DataAnnotations;

namespace ASM_C_4.Models.ViewModels
{
	public class LoginViewModel
	{
		public int Id { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
		public string Username { get; set; }
		[DataType(DataType.Password), Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		public string Password { get; set; }
		
		public string returnUrl { get; set; }
	}
}
