using BookStoreApp.Models;
using BookStoreApp.Repositories.Config;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Repositories
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options) : 
            base(options)
        {
            
        }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookConfig());
        }
    }
}
