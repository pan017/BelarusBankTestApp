using BelarusBankTestApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace BelarusBankTestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(connection);
                
            });
                builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.RequireHttpsMetadata = false;
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuer = true,
                           ValidIssuer = AuthOptions.ISSUER,

                           ValidateAudience = true,
                           ValidAudience = AuthOptions.AUDIENCE,
                           ValidateLifetime = true,

                           IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                           ValidateIssuerSigningKey = true,
                       };
                   });
            builder.Services.AddControllersWithViews();

            builder.Logging.ClearProviders();   
            builder.Logging.AddConsole();  

            var app = builder.Build();

            app.UseDeveloperExceptionPage();
            app.UseSession();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers(); 
                
            });

            app.Run();
        }
    }
}