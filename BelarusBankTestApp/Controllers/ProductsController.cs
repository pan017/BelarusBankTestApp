using BelarusBankTestApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Data;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Security.Principal;

namespace BelarusBankTestApp.Controllers
{
    //[Route("api/[controller]")]
    // [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private ApplicationContext _db;
        private readonly ILogger<ProductsController> _logger;
        public ProductsController(ILogger<ProductsController> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        [Authorize]
        [Route("api/GetUserRole")]
        public string GetUserRole()
        {
            ClaimsIdentity claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.Role).Value;
        }

        #region Products
        [Authorize]
        [Route("api/GetProducts")]
        public IActionResult GetProducts()
        {
            var result = _db.tbProduct.Include(u => u.ProductCategory).ToList();
            if (GetUserRole() == "user")
                result.ForEach(x => x.NotesPrivate = "");
            return new ObjectResult(result);
        }


        [Authorize]
        [Route("api/GetProduct/{id}")]
        public IActionResult GetProduct(int id)
        {
            var result = _db.tbProduct.First(x => x.Id == id);
            if (GetUserRole() == "user")
                result.NotesPrivate = "";
            return new ObjectResult(result);
        }

        [Authorize]
        [HttpPost("api/EditProduct")]
        public IActionResult EditProduct(int Id, string Name, string Description, int Category, decimal Price, string Notes, string NotesPrivate) 
        {
            try
            {
                string errorMessage = "";
                if (String.IsNullOrEmpty(Name))
                    errorMessage = "Поле \"Наименование продукта\" должно быть заполнено";

                if (String.IsNullOrEmpty(Description))
                    errorMessage = "Поле \"Описание\" должно быть заполнено";

                if (String.IsNullOrEmpty(Notes))
                    errorMessage = "Поле \"Примечания общее\" должно быть заполнено";

                if (String.IsNullOrEmpty(NotesPrivate) && GetUserRole() != "user")
                    errorMessage =  "Поле \"Примечание специальное\" должно быть заполнено";

                if (Price == null || Price == 0)
                    errorMessage = "Поле \"Стоимость в рублях\" должно быть заполнено и отличаться от 0";

                if (!String.IsNullOrEmpty(errorMessage))
                {
                    _logger.LogInformation(errorMessage);
                    return BadRequest(errorMessage);
                }
                Product product = _db.tbProduct.FirstOrDefault(x => x.Id == Id);

                product.ProductCategory = _db.tbProductCategory.FirstOrDefault(x => x.Id == Category);
                product.Name = Name;
                product.Description = Description;
                product.Price = Price;
                product.Notes = Notes;

                if (GetUserRole() != "user")
                    product.NotesPrivate = NotesPrivate;

                _db.SaveChanges();

                _logger.LogInformation("Edit product: " + product.Name);
            }
            catch (Exception ex) 
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("api/AddProduct")]
        public IActionResult AddProduct(string Name, string Description, int Category, decimal Price, string Notes, string NotesPrivate)
        {
            try
            {
                string errorMessage = "";

                if (String.IsNullOrEmpty(Name))
                    errorMessage = "Поле \"Наименование продукта\" должно быть заполнено";

                if (String.IsNullOrEmpty(Description))
                    errorMessage = "Поле \"Описание\" должно быть заполнено";

                if (String.IsNullOrEmpty(Notes))
                    errorMessage = "Поле \"Примечания общее\" должно быть заполнено";

                if (String.IsNullOrEmpty(NotesPrivate) && GetUserRole() != "user")
                    errorMessage = "Поле \"Примечание специальное\" должно быть заполнено";

                if (Price == null || Price == 0)
                    errorMessage = "Поле \"Стоимость в рублях\" должно быть заполнено и отличаться от 0";

                if (!String.IsNullOrEmpty(errorMessage))
                {
                    _logger.LogInformation(errorMessage);
                    return BadRequest(errorMessage);
                }


                Product product = new Product();

                product.ProductCategory = _db.tbProductCategory.FirstOrDefault(x => x.Id == Category);
                product.Name = Name;
                product.Description = Description;
                product.Price = Price;
                product.Notes = Notes;
                product.NotesPrivate = NotesPrivate;
                _db.tbProduct.Add(product);
                _db.SaveChanges();

                _logger.LogInformation("Add product: " + product.Name);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "admin, superuser")]
        [HttpPost("api/DeleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Delete product " + _db.tbProduct.FirstOrDefault(x => x.Id == id).Name);
                _db.tbProduct.Remove(_db.tbProduct.FirstOrDefault(x => x.Id == id));
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        #endregion


        #region Category
        [HttpGet("api/GetCategories")]
        public IActionResult GetCategories()
        {
            return new ObjectResult(_db.tbProductCategory.ToList());
        }

        [Authorize(Roles = "admin, superuser")]
        [Route("api/GetCategory/{id}")]
        public IActionResult GetCategory(int id)
        {

            return new ObjectResult(_db.tbProductCategory.First(x => x.Id == id));
        }

        [Authorize(Roles = "admin, superuser")]
        [HttpPost("api/EditCategory")]
        public IActionResult EditCategory(int Id, string Name, string Description)
        {
            try
            {
                ProductCategory productCategory = _db.tbProductCategory.FirstOrDefault(x => x.Id == Id);

                productCategory.Name = Name;
                productCategory.Description = Description;
                _db.SaveChanges();

                _logger.LogInformation("Edit category " + productCategory.Name);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "admin, superuser")]
        [HttpPost("api/AddCategory")]
        public IActionResult AddCategory(string Name, string Description)
        {
            try
            {
                ProductCategory productCategory = new ProductCategory();

                productCategory.Name = Name;
                productCategory.Description = Description;
               
                _db.tbProductCategory.Add(productCategory);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "admin, superuser")]
        [HttpPost("api/DeleteCategory/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                List<Product> productList = _db.tbProduct.Include(u => u.ProductCategory).Where(x => x.ProductCategory.Id == id).ToList();
                _db.tbProduct.RemoveRange(productList);
                _logger.LogInformation("Delete category " + _db.tbProductCategory.FirstOrDefault(x => x.Id == id).Name);
                _db.tbProductCategory.Remove(_db.tbProductCategory.FirstOrDefault(x => x.Id == id));
                _db.SaveChanges();

                
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.Message);
                return BadRequest(ex.Message);
            }
            return Ok();
        }
        #endregion
    }
}
