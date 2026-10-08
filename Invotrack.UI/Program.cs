using Invotrack.Application.Services;
using Invotrack.Domain.Entities;
using Invotrack.Domain.Interfaces;
using Invotrack.Infrastructure.Data;
using Invotrack.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Invotrack.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddDbContext<InvotrackDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<InvotrackDbContext>()
            .AddDefaultTokenProviders();

            
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("OwnerOnly", policy => policy.RequireRole("BusinessOwner"));
                options.AddPolicy("ManagementLevel", policy => policy.RequireRole("BusinessOwner", "WarehouseManager"));
            });

           
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            
            builder.Services.AddControllersWithViews();

            
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICsvExporter, CsvExporter>();
            builder.Services.AddScoped<IPdfExporter, PdfExporter>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<IPartnerService, PartnerService>();
            builder.Services.AddScoped<IStockTransactionService, StockTransactionService>();
            builder.Services.AddScoped<ITransferService, TransferService>();
            builder.Services.AddScoped<IStockAlertService, StockAlertService>();
            builder.Services.AddScoped<IStockReportService, StockReportService>();
            builder.Services.AddScoped<IReportExportService, ReportExportService>();
            builder.Services.AddScoped<IUserService, UserService>();

            var app = builder.Build();

            
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

           
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<InvotrackDbContext>();
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                    await context.Database.MigrateAsync();

                    
                    string[] roles = { "BusinessOwner", "WarehouseManager", "Storekeeper" };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                            await roleManager.CreateAsync(new ApplicationRole { Name = role, Description = $"Role for {role}" });
                    }

                    
                    if (await userManager.FindByEmailAsync("admin@invotrack.com") == null)
                    {
                        var adminUser = new ApplicationUser
                        {
                            UserName = "admin@invotrack.com",
                            Email = "admin@invotrack.com",
                            FullName = "مدير النظام الرئيسي",
                            IsActive = true,
                            EmailConfirmed = true
                        };
                        var result = await userManager.CreateAsync(adminUser, "Admin123!");
                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, "BusinessOwner");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "حدث خطأ أثناء تهيئة قاعدة البيانات والأدوار الأولية.");
                }
            }

            app.Run();
        }
    }
}