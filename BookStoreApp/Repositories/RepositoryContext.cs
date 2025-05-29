using BookStoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Repositories
{
    public class RepositoryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
    }
}
