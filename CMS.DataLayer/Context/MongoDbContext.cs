using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CMS.Entities.Sample;
using CMS.Entities.Sample2;
using CMS.ViewModel.Settings;
using MongoDB.Driver;

namespace CMS.DataLayer.Context
{
    public interface IMongoDbContext
    {
        IMongoCollection<SampleModel> Sample { get; }
        IMongoCollection<Sample2Model> Sample2 { get; }
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
            return _mongoDatabase.GetCollection<T>(TableName<T>());
        }

        internal string TableName<T>()
        {
            var entity = typeof(T);
            var tableName = entity.Name;

            var customAttributes = entity.GetCustomAttributes(typeof(TableAttribute),false);
            if (customAttributes.Any())
                tableName = (customAttributes.First() as TableAttribute)?.Name ?? entity.Name;

            return tableName;
        }

        #endregion

        #region Entities

        public IMongoCollection<SampleModel> Sample => Collection<SampleModel>();
        public IMongoCollection<Sample2Model> Sample2 => Collection<Sample2Model>();

        #endregion
    }
}
