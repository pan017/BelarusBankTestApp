using System.Numerics;

namespace BelarusBankTestApp.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //public ICollection<ProductCategory> ProductCategorys { get; set; }

        public ProductCategory() 
        {
            //ProductCategorys = new List<ProductCategory>();
        }
        public ProductCategory(string name, string description)
        {
            Name = name;
            Description = description;
            //ProductCategorys = new List<ProductCategory>();
        }
    }
}
