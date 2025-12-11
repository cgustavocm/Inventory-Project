using Inventory_Project.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Item { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // seed de categorías
            modelBuilder.Entity<Category>().HasData(
                new Category { ID = 1, Name = "Electronic" },
                new Category { ID = 2, Name = "Non Electronic" }
            );
        }
    }
}

