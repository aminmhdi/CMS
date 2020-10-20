using System;
using CMS.Entities.Sample;
using CMS.ViewModel.Settings;
using MongoDB.Driver;

namespace CMS.DataLayer.Context
{
    public interface IMongoDbContext
    {
        IMongoCollection<Sample> Sample { get; }
    }

    public class MongoDbContext : IMongoDbContext
    {
        #region Constructor

        private readonly IMongoDatabase _mongoDatabase;

        public MongoDbContext(AppSettings appSettings)
        {
            var client = new MongoClient(appSettings.ConnectionStrings.MongoDb.Connection);
            var mongodbName = appSettings.ConnectionStrings.MongoDb.Connection.Split('?')[0].Split('/')[3];
            _mongoDatabase = client.GetDatabase(mongodbName);
        }

        #endregion

        #region Collections

        private IMongoCollection<T> Collection<T>() where T : new()
        {
            return _mongoDatabase.GetCollection<T>(typeof(T).Name);
        }

        #endregion

        #region Entities

        public IMongoCollection<Sample> Sample => Collection<Sample>();

        #endregion
    }
}
