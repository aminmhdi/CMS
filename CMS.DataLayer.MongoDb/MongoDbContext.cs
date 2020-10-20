using CMS.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace CMS.DataLayer.MongoDb
{
    public class MongoDbContext : ApplicationDbContext
    {
        private readonly IMongoDatabase _db;

        public MongoDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}