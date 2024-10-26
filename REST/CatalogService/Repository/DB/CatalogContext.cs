using CatalogAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogAPI.Repository.DB
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}
