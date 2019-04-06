using System.Collections.Generic;
using Datory.Utils;
using SqlKata;

namespace Datory
{
    public partial class Repository
    {
        public virtual T Get<T>(int id) where T : Entity
        {
            return id <= 0 ? null : Get<T>(Q.Where(nameof(Entity.Id), id));
        }

        public virtual T Get<T>(string guid) where T : Entity
        {
            return !ConvertUtils.IsGuid(guid) ? null : Get<T>(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual T Get<T>(Query query = null)
        {
            var value = RepositoryUtils.GetValue<T>(DatabaseType, ConnectionString, TableName, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                RepositoryUtils.SyncAndCheckGuid(DatabaseType, ConnectionString, TableName, value as Entity);
            }

            return value;
        }

        public virtual IList<T> GetAll<T>(Query query = null)
        {
            var list = RepositoryUtils.GetValueList<T>(DatabaseType, ConnectionString, TableName, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                foreach (var value in list)
                {
                    RepositoryUtils.SyncAndCheckGuid(DatabaseType, ConnectionString, TableName, value as Entity);
                }
            }

            return list;
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
