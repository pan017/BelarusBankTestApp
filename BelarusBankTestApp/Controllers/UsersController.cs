using BelarusBankTestApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace BelarusBankTestApp.Controllers
{
    
    public class UsersController : Controller
    {
        private ApplicationContext _db;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }
        [HttpGet]
        public ActionResult Index()
        {
            if (HttpContext.Session.GetString("currentUserRole") == "admin")
            {
                var result = _db.tbAppUser.Include(u => u.Role).ToList();
                return View(result);
            }
            else
            {
                return Redirect("products.html");
            }
        }

        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("currentUserRole") == "admin")
            {
                ViewBag.RoleList = new SelectList(_db.tbRole, "Id", "Name");
                return View();
            }
            else
            {
                return Redirect("products.html");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AppUser newUser)
        {
            if (HttpContext.Session.GetString("currentUserRole") == "admin")
            {
                try
                {
                    if (_db.tbAppUser.Any(x => x.Email == newUser.Email) ) 
                    {
                        ModelState.AddModelError("Email", "Пользователь с таким E-mail уже существует");
                        return View(newUser);
                    }
               
                    newUser.Role = _db.tbRole.FirstOrDefault(x => x.Id == newUser.Role.Id);
                    _db.tbAppUser.Add(newUser);
                    _db.SaveChanges();

                    _logger.LogInformation("Create user " + newUser.Email);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(newUser);
                }
            }
            else
            {
                return Redirect("products.html");
            }
        }

        public ActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("currentUserRole") == "admin")
            {
                SelectList rolesList = new SelectList(_db.tbRole, "Id", "Name");
                var selected = rolesList.Where(x => x.Value == _db.tbAppUser.Include(u => u.Role).FirstOrDefault(x => x.Id == id).Role.Id.ToString()).First();
                selected.Selected = true;

                ViewBag.RoleList = rolesList;

                

                return View(_db.tbAppUser.Include(u => u.Role).FirstOrDefault(x => x.Id == id));
            }
            else
            {
                return Redirect("products.html");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AppUser appUser)
        {
            if (HttpContext.Session.GetString("currentUserRole") == "admin")
            {
                try
                {
                    if (_db.tbAppUser.Any(x => x.Email == appUser.Email && x.Id != appUser.Id))
                    {
                        SelectList rolesList = new SelectList(_db.tbRole, "Id", "Name");
                        var selected = rolesList.Where(x => x.Value == _db.tbAppUser.Include(u => u.Role).FirstOrDefault(x => x.Id == id).Role.Id.ToString()).First();
                        selected.Selected = true;

                        ViewBag.RoleList = rolesList;

                        ModelState.AddModelError("Email", "Пользователь с таким E-mail уже существует");
                        return View(appUser);
                    }

                    AppUser editUser = _db.tbAppUser.Include(u => u.Role).FirstOrDefault(x => x.Id == id);
                    editUser.Email = appUser.Email;
                    editUser.Role = _db.tbRole.FirstOrDefault(x => x.Id == appUser.Role.Id);
                    editUser.Password = appUser.Password;
                    _db.SaveChanges();

                    _logger.LogInformation("Edit user " + editUser.Email);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View(appUser);
                }
            }
            else
            {
                return Redirect("products.html");
            }
        }

        public ActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("currentUserRole") == "admin")
            {
                _logger.LogInformation("Delte user " + _db.tbAppUser.FirstOrDefault(x => x.Id == id).Email);
                _db.tbAppUser.Remove(_db.tbAppUser.FirstOrDefault(x => x.Id == id));
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return Redirect("products.html");
            }
        }
    }
}
