namespace BelarusBankTestApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        public decimal Price { get; set; }
        public string Notes { get; set; }
        public string NotesPrivate { get; set; }

        //public int? ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public Product(string name, string description, ProductCategory productCategory, decimal price, string notes, string notesPrivate)
        {
            Name = name;
            Description = description;
            ProductCategory = productCategory;
            Price = price;
            Notes = notes;
            NotesPrivate = notesPrivate;
        }

        public Product() { }
    }
}
