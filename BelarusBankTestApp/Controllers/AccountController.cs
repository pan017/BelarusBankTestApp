using BelarusBankTestApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BelarusBankTestApp.Controllers
{

    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private ApplicationContext _db;

        public AccountController(ILogger<AccountController> logger, ApplicationContext context)
        {
             _logger = logger;
            _db = context;
        }

        [HttpPost("/login")]
        public IActionResult Login(string username, string password)
        {
            if (!_db.tbProductCategory.Any())
            {
                _db.tbProductCategory.AddRange(
                    new ProductCategory { Name = "Еда", Description = "" },
                    new ProductCategory { Name = "Вкусности", Description = "" },
                    new ProductCategory { Name = "Вода", Description = "" }
                 );
                _db.SaveChanges();
            }

            if (!_db.tbProduct.Any())
            {
                _db.tbProduct.AddRange(
                    new Product { Name = "Селедка", ProductCategory = _db.tbProductCategory.FirstOrDefault(x => x.Id == 1), Description = "Селедка соленая", Price = 10, Notes = "Акция", NotesPrivate = "Пересоленая" },
                    new Product { Name = "Тушенка", ProductCategory = _db.tbProductCategory.FirstOrDefault(x => x.Id == 1), Description = "Тушенка говяжья", Price = 20, Notes = "Вкусная", NotesPrivate = "Жилы" },
                    new Product { Name = "Сгущенка", ProductCategory = _db.tbProductCategory.FirstOrDefault(x => x.Id == 2), Description = "В банках", Price = 30, Notes = "С ключем", NotesPrivate = "Вкусная" },
                    new Product { Name = "Квас", ProductCategory = _db.tbProductCategory.FirstOrDefault(x => x.Id == 2), Description = "В бутылках", Price = 15, Notes = "Вятский", NotesPrivate = "Теплый" }
                );
            }
            if (!_db.tbRole.Any())
            {
                _db.tbRole.AddRange(
                    new Role { Name = "admin" },
                    new Role { Name = "superuser" },
                    new Role { Name = "user" }
                );
                _db.SaveChanges();
            }

            if (!_db.tbAppUser.Any())
            {
                _db.tbAppUser.AddRange(
                    new AppUser { Email = "admin@mail.by", Password = "admin", Role = _db.tbRole.FirstOrDefault(x => x.Id == 1) },
                    new AppUser { Email = "superuser@mail.by", Password = "superuser", Role = _db.tbRole.FirstOrDefault(x => x.Id == 2) },
                    new AppUser { Email = "user@mail.by", Password = "user", Role = _db.tbRole.FirstOrDefault(x => x.Id == 3) }
                );
            }
            _db.SaveChanges();

            var identity = GetIdentity(username, password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Неверный логин или пароль" });
            }

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name
            };

            HttpContext.Session.SetString("currentUserRole", identity.FindFirst(ClaimTypes.Role).Value);
            HttpContext.Session.SetString("token", encodedJwt);

            _logger.LogInformation("login user: " + username);
            return Json(response);
        }

        private ClaimsIdentity GetIdentity(string username, string password)
        {
            AppUser user = _db.tbAppUser.Include(u => u.Role).FirstOrDefault(x => x.Email == username && x.Password == password);
          
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Name)
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            return null;
        }
    }
}
