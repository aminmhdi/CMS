using CMS.DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace CMS.DataLayer.InMemoryDatabase
{
    public class InMemoryDatabaseContext : ApplicationDbContext
    {
        public InMemoryDatabaseContext(DbContextOptions options) : base(options)
        {
        }
    }
}