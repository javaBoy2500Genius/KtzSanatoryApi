using KTZSanatoriumServiceApi.Models;
using Microsoft.AspNetCore.HttpLogging;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace KTZSanatoriumServiceApi.Data.Repository
{
    public sealed class DataRepository<T> : IDbRepository<T> where T : Base
    {
        private readonly IMongoCollection<T> _collection;
        private readonly IDbSetting _setting;
        public DataRepository(IDbSetting setting)
        {
            var client = new MongoClient(setting.Url);
            var db = client.GetDatabase(setting.DatabaseName);
            var coll = GetCollectionName(typeof(T)) ?? typeof(T).Name;
            _collection = db.GetCollection<T>(coll);
            _setting = setting;
        }

        private string? GetCollectionName(Type documentType)
        {
            try
            {
                return (documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute), true)
                .FirstOrDefault() as BsonCollectionAttribute)?.CollectionName;
            }
            catch (Exception)
            {

                throw;
            }
        }




        public IQueryable<T> AsQueryable()
        {
            return _collection.AsQueryable();
        }

        public bool Contains<P>(Expression<Func<P, bool>> filter)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var body = new ExpressionParameterReplacer(filter.Parameters[0], parameter).Visit(filter.Body);
            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);

            return _collection.Find(lambda).Any();
        }

        public bool Delete(Expression<Func<T, bool>> filter)
        {
            return _collection.FindOneAndDelete(filter) != null;
        }

        public bool DeleteById(string id)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, id);
            return _collection.FindOneAndDelete(filter) != null;
        }

        public bool DeleteMany(Expression<Func<T, bool>> filter)
        {
            return _collection.DeleteMany(filter) != null;
        }

        public T? FindById(string id)
        {
            return _collection.Find(x=>x.Id==id&&!x.IsDeleted).FirstOrDefault();
        }

        public T? FindOne(Expression<Func<T, bool>> filter)
        {

            return _collection.Find(filter).FirstOrDefault();


        }

        public void InsertMany(ICollection<T> list)
        {
            _collection.InsertMany(list);
        }

        public void InsertOne(T obj)
        {
            _collection.InsertOne(obj);
        }

        public bool ReplaceOne(T obj)
        {
            var filter = Builders<T>.Filter.Eq(doc => doc.Id, obj.Id);
            return _collection.FindOneAndReplace(filter, obj) != null;

        }

        public bool UpdateMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition)
        {
            return _collection.UpdateMany(filter, updateDefinition) != null;
        }

        public bool UpdateOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition)
        {
            return _collection.UpdateOne(filter, updateDefinition) != null;
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> filter)
        {
            return AsQueryable().Where(filter);
        }


        public bool UpdateMany(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition)
        {
            return _collection.UpdateMany(filter, updateDefinition) != null;
        }
    }
}
