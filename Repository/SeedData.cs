using ASM_C_4.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_4.Repository
{
	public class SeedData
	{
		public static void SeedingData(DataContext _context)
		{
			/*_context.Database.Migrate();
			if (!_context.Products.Any())
			{
				CategoryModel annhe = new CategoryModel { Name = "An Nhe", Slug = "an nhe", Description = "Thuc an ngon", Status = 1 };
				CategoryModel anno = new CategoryModel { Name = "An No", Slug = "an no", Description = "Thuc an no bung", Status = 1 };
				BrandModel KFC = new BrandModel { Name = "KFC", Slug = "kfc", Description = "Do an no bung", Status = 1 };
				BrandModel Macdonal = new BrandModel { Name = "Macdonal", Slug = "macdonal", Description = "Do an no bung", Status = 1 };
				_context.Products.AddRange(
					new ProductModel { Name = "Ga ran", Slug = "ga ran", Description = "Ga ran", Image = "Hinh1.jpg", Category = annhe, Brand = KFC, Price = 1123 },
					new ProductModel { Name = "Hambergirl", Slug = "hambergirl", Description = "Hambergirl ???", Image = "Hinh2.jpg", Category = anno, Brand = Macdonal, Price = 20000 },
					new ProductModel { Name = "Banh Mi VN", Slug = "banh mi vn", Description = "banh mi mua ngoai cho", Image = "Hinh3.jpg", Category = annhe, Brand = KFC, Price = 10000 },
					new ProductModel { Name = "Goi Tom", Slug = "goi tom", Description = "Goi hai san", Image = "Hinh4.jpg", Category = anno, Brand = Macdonal, Price = 80332 },
					new ProductModel { Name = "Mi Tom", Slug = "mi tom", Description = "Mi & Tom", Image = "Hinh5.jpg", Category = annhe, Brand = KFC, Price = 5000 },
					new ProductModel { Name = "Pizzazz", Slug = "pizzazz", Description = "Chua an bao gio", Image = "Hinh6.jpg", Category = anno, Brand = Macdonal, Price = 99000 }
				);
				_context.SaveChanges();
			}*/
		}
	}
}
