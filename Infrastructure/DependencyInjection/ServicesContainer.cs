using Application.Interfaces.Management;
using Application.Interfaces.Setting;
using Domain.Models.Auth;
using Infrastructure.Data;
using Infrastructure.Repositories.Management;
using Infrastructure.Repositories.Setting;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection
{
    public static class ServicesContainer
    {
        public static IServiceCollection ShelkovyPutService(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ShelkobyPutDbContext>(options =>
            {
                var connectString = config.GetConnectionString("Default") ??
                    throw new InvalidOperationException("Thieu đường dẫn đến CSDL rồi kìa :)) Vui long check trong appsetting.json file");
                options.UseSqlServer(connectString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
            });

            services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
              .AddRoles<IdentityRole>()
              .AddRoleManager<RoleManager<IdentityRole>>()
              .AddUserManager<UserManager<User>>()
              .AddEntityFrameworkStores<ShelkobyPutDbContext>()
              .AddSignInManager<SignInManager<User>>()
              .AddDefaultTokenProviders();

            services.AddAuthentication();
            services.AddAuthorization();
            services.AddRazorPages();

            // Services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IInitService, InitService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IWishlistService, WishListService>();
            services.AddSingleton<IVnPayService, VnPayService>();
            services.AddScoped<IExcelFileImportService, ExcelFileImportService>();
            services.AddScoped<ISenderEmailService, SenderEmailService>();
            services.AddSingleton<OrderStatusNameTranslateService>();
            services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            services.AddSingleton<MongoDBContext>();

            return services;
        }
    }
}
