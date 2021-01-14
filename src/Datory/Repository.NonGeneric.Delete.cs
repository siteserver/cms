using System.Threading.Tasks;
using SqlKata;
using Datory.Utils;

namespace Datory
{
    public partial class Repository
    {
        // public virtual bool Delete(int id)
        // {
        //     if (id <= 0) return false;

        //     return Delete(Q.Where(nameof(Entity.Id), id)) > 0;
        // }

        // public virtual bool Delete(string guid)
        // {
        //     if (!Utilities.IsGuid(guid)) return false;

        //     return Delete(Q.Where(nameof(Entity.Guid), guid)) > 0;
        // }

        // public virtual int Delete(Query query = null)
        // {
        //     return RepositoryUtils.DeleteAll(Database, TableName, query);
        // }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0) return false;

            return await DeleteAsync(Q.Where(nameof(Entity.Id), id)) > 0;
        }

        public virtual async Task<bool> DeleteAsync(string guid)
        {
            if (!Utilities.IsGuid(guid)) return false;

            return await DeleteAsync(Q.Where(nameof(Entity.Guid), guid)) > 0;
        }

        public virtual async Task<int> DeleteAsync(Query query = null)
        {
            return await RepositoryUtils.DeleteAllAsync(Database, TableName, Redis, query);
        }
    }
}
