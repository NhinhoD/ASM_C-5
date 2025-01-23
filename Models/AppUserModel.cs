using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ASM_C_4.Models
{
	public class AppUserModel : IdentityUser
	{
		[MaxLength(int.MaxValue)]
		public string RoleId { get; set; }
		
	}
}
