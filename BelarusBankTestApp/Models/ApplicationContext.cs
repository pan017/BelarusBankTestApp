using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;

namespace BelarusBankTestApp.Models
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Role> tbRole { get; set; }
        public DbSet<AppUser> tbAppUser { get; set; }
        public DbSet<ProductCategory> tbProductCategory { get; set; }
        public DbSet<Product> tbProduct { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}