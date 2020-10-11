using CMS.DataLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace CMS.DataLayer.Mssql
{
    public class MsSqlDbContext : ApplicationDbContext
    {
        public MsSqlDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}