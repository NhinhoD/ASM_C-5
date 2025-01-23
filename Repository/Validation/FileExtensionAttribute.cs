using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel.DataAnnotations;

namespace ASM_C_4.Repository.Validation
{
	public class FileExtensionAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validation)
		{
			if (value is IFormFile file)
			{
				var extension = Path.GetExtension(file.FileName);
				string[] extensions = {"jpg", "png", "jpeg"};

				bool result = extension.Any(x => extension.EndsWith(x));

				if (!result)
				{
					return new ValidationResult("Chi cho phep anh jpg, png hoac jpeg");
				}
			}return ValidationResult.Success; 
		}
	}
}
