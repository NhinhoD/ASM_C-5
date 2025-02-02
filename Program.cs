﻿using ASM_C_4.Areas.Admin.Repository;
using ASM_C_4.Models;
using ASM_C_4.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace ASM_C_4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDb"]);
                });

            // add email render
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddDistributedMemoryCache();

            // cau hinh cho session
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true;
            });


            // khai bao Identity
			builder.Services.AddIdentity<AppUserModel,IdentityRole>()
	        .AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Password settings.
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 4;

				// User settings.
				options.User.RequireUniqueEmail = true;
			});

			var app = builder.Build();



            app.UseStatusCodePagesWithRedirects("/Home/Error?statuscode={0}");

            // use session
            app.UseSession();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

			app.UseAuthentication(); // dang nhap
			app.UseAuthorization(); // khiem tra quyen


			//Seed Data
			/*var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
			SeedData.SeedingData(context);*/

            // routing cho admin
			app.MapControllerRoute(
				name: "Areas",
				pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

            //làm đẹp routing category
            app.MapControllerRoute(
                name: "category",
                pattern: "/category/{Slug?}",
                defaults: new { controller = "Category", action = "Index" });

            //làm đẹp routing Brand
            app.MapControllerRoute(
                name: "brand",
                pattern: "/brand/{Slug?}",
                defaults: new { controller = "Brand", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
        }
    }
}
