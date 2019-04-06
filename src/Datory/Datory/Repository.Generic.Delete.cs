using Datory.Utils;
using SqlKata;

namespace Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual bool Delete(int id)
        {
            if (id <= 0) return false;

            return Delete(Q.Where(nameof(Entity.Id), id)) > 0;
        }

        public virtual bool Delete(string guid)
        {
            if (!ConvertUtils.IsGuid(guid)) return false;

            return Delete(Q.Where(nameof(Entity.Guid), guid)) > 0;
        }

        public virtual int Delete(Query query = null)
        {
            return RepositoryUtils.DeleteAll(DatabaseType, ConnectionString, TableName, query);
        }
    }
}
