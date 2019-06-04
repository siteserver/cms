using SqlKata;
using SS.CMS.Plugin.Data.Utils;

namespace SS.CMS.Plugin.Data
{
    public partial class Repository
    {
        public virtual bool Delete(int id)
        {
            if (id <= 0) return false;

            return Delete(Q.Where(nameof(Entity.Id), id)) > 0;
        }

        public virtual bool Delete(string guid)
        {
            if (!Utilities.IsGuid(guid)) return false;

            return Delete(Q.Where(nameof(Entity.Guid), guid)) > 0;
        }

        public virtual int Delete(Query query = null)
        {
            return RepositoryUtils.DeleteAll(DatabaseType, ConnectionString, TableName, query);
        }
    }
}
