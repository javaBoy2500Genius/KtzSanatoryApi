using KTZSanatoriumServiceApi.Models;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace KTZSanatoriumServiceApi.Data.Repository
{
    public interface IDbRepository<T> where T : Base
    {

        IQueryable<T> AsQueryable();

        T? FindOne(Expression<Func<T, bool>> filter);
        T? FindById(string id);
        void InsertOne(T obj);

        void InsertMany(ICollection<T> list);

        bool ReplaceOne(T obj);

        bool UpdateOne(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition);

        bool UpdateMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDefinition);

        bool DeleteById(string id);

        bool Delete(Expression<Func<T, bool>> filter);

        bool DeleteMany(Expression<Func<T, bool>> filter);

        bool Contains<P>(Expression<Func<P, bool>> filter);
        bool UpdateMany(FilterDefinition<T> filter, UpdateDefinition<T> updateDefinition);
        IQueryable<T> FindAll(Expression<Func<T, bool>> filter);
    }
}

