using System.Collections.Generic;
using Datory.Utils;
using SqlKata;

namespace Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual T Get(int id)
        {
            return id <= 0 ? null : Get(Q.Where(nameof(Entity.Id), id));
        }

        public virtual T Get(string guid)
        {
            return !ConvertUtils.IsGuid(guid) ? null : Get(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual T Get(Query query = null)
        {
            return RepositoryUtils.GetObject<T>(DatabaseType, ConnectionString, TableName, query);
        }

        public virtual IList<T> GetAll(Query query = null)
        {
            return RepositoryUtils.GetObjectList<T>(DatabaseType, ConnectionString, TableName, query);
        }

        public virtual TValue Get<TValue>(Query query)
        {
            return RepositoryUtils.GetValue<TValue>(DatabaseType, ConnectionString, TableName, query);
        }

        public virtual IList<TValue> GetAll<TValue>(Query query = null)
        {
            return RepositoryUtils.GetValueList<TValue>(DatabaseType, ConnectionString, TableName, query);
        }

        public virtual bool Exists(int id)
        {
            return id > 0 && Exists(Q.Where(nameof(Entity.Id), id));
        }

        public virtual bool Exists(string guid)
        {
            return ConvertUtils.IsGuid(guid) && Exists(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual bool Exists(Query query = null)
        {
            return RepositoryUtils.Exists(DatabaseType, ConnectionString, TableName, query);
        }

        public virtual int Count(Query query = null)
        {
            return RepositoryUtils.Count(DatabaseType, ConnectionString, TableName, query);
        }

        public virtual int Sum(Query query = null)
        {
            return RepositoryUtils.Sum(DatabaseType, ConnectionString, TableName, query);
        }

        public virtual int? Max(Query query = null)
        {
            return RepositoryUtils.Max(DatabaseType, ConnectionString, TableName, query);
        }
    }
}
