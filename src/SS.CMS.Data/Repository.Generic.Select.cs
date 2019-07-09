using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Data.Utils;

namespace SS.CMS.Data
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual async Task<T> GetAsync(int id)
        {
            return id <= 0 ? null : await GetAsync(Q.Where(nameof(Entity.Id), id));
        }

        public virtual async Task<T> GetAsync(string guid)
        {
            return !Utilities.IsGuid(guid) ? null : await GetAsync(Q.Where(nameof(Entity.Guid), guid));
        }

        public virtual async Task<T> GetAsync(Query query = null)
        {
            return await RepositoryUtils.GetObjectAsync<T>(Database, TableName, query);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Query query = null)
        {
            return await RepositoryUtils.GetObjectListAsync<T>(Database, TableName, query);
        }

        public virtual async Task<TValue> GetAsync<TValue>(Query query)
        {
            return await RepositoryUtils.GetValueAsync<TValue>(Database, TableName, query);
        }

        public virtual async Task<IEnumerable<TValue>> GetAllAsync<TValue>(Query query = null)
        {
            return await RepositoryUtils.GetValueListAsync<TValue>(Database, TableName, query);
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
            return await RepositoryUtils.ExistsAsync(Database, TableName, query);
        }

        public virtual async Task<int> CountAsync(Query query = null)
        {
            return await RepositoryUtils.CountAsync(Database, TableName, query);
        }

        public virtual async Task<int> SumAsync(string columnName, Query query = null)
        {
            return await RepositoryUtils.SumAsync(Database, TableName, columnName, query);
        }

        public virtual async Task<int?> MaxAsync(string columnName, Query query = null)
        {
            return await RepositoryUtils.MaxAsync(Database, TableName, columnName, query);
        }
    }
}
