using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Data.Utils;

namespace SS.CMS.Data
{
    public partial class Repository
    {
        public virtual T Get<T>(int id) where T : Entity
        {
            return id <= 0 ? null : Get<T>(Q.Where(nameof(Entity.Id), id));
        }

        public virtual T Get<T>(string guid) where T : Entity
        {
            return !Utilities.IsGuid(guid) ? null : Get<T>(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual T Get<T>(Query query = null)
        {
            var value = RepositoryUtils.GetValue<T>(DbContext, TableName, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                RepositoryUtils.SyncAndCheckGuid(DbContext, TableName, value as Entity);
            }

            return value;
        }

        public virtual IEnumerable<T> GetAll<T>(Query query = null)
        {
            var list = RepositoryUtils.GetValueList<T>(DbContext, TableName, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                foreach (var value in list)
                {
                    RepositoryUtils.SyncAndCheckGuid(DbContext, TableName, value as Entity);
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
            return Utilities.IsGuid(guid) && Exists(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual bool Exists(Query query = null)
        {
            return RepositoryUtils.Exists(DbContext, TableName, query);
        }

        public virtual int Count(Query query = null)
        {
            return RepositoryUtils.Count(DbContext, TableName, query);
        }

        public virtual int Sum(string columnName, Query query = null)
        {
            return RepositoryUtils.Sum(DbContext, TableName, columnName, query);
        }

        public virtual int? Max(string columnName, Query query = null)
        {
            return RepositoryUtils.Max(DbContext, TableName, columnName, query);
        }

        public virtual async Task<T> GetAsync<T>(int id) where T : Entity
        {
            return id <= 0 ? null : await GetAsync<T>(Q.Where(nameof(Entity.Id), id));
        }

        public virtual async Task<T> GetAsync<T>(string guid) where T : Entity
        {
            return !Utilities.IsGuid(guid) ? null : await GetAsync<T>(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual async Task<T> GetAsync<T>(Query query = null)
        {
            var value = await RepositoryUtils.GetValueAsync<T>(DbContext, TableName, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                await RepositoryUtils.SyncAndCheckGuidAsync(DbContext, TableName, value as Entity);
            }

            return value;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync<T>(Query query = null)
        {
            var list = await RepositoryUtils.GetValueListAsync<T>(DbContext, TableName, query);

            if (typeof(T).IsAssignableFrom(typeof(Entity)))
            {
                foreach (var value in list)
                {
                    await RepositoryUtils.SyncAndCheckGuidAsync(DbContext, TableName, value as Entity);
                }
            }

            return list;
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return id > 0 && await ExistsAsync(Q.Where(nameof(Entity.Id), id));
        }

        public virtual async Task<bool> ExistsAsync(string guid)
        {
            return Utilities.IsGuid(guid) && await ExistsAsync(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual async Task<bool> ExistsAsync(Query query = null)
        {
            return await RepositoryUtils.ExistsAsync(DbContext, TableName, query);
        }

        public virtual async Task<int> CountAsync(Query query = null)
        {
            return await RepositoryUtils.CountAsync(DbContext, TableName, query);
        }

        public virtual async Task<int> SumAsync(string columnName, Query query = null)
        {
            return await RepositoryUtils.SumAsync(DbContext, TableName, columnName, query);
        }

        public virtual async Task<int?> MaxAsync(string columnName, Query query = null)
        {
            return await RepositoryUtils.MaxAsync(DbContext, TableName, columnName, query);
        }
    }
}
