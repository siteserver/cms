using SS.CMS.Data.Utils;

namespace SS.CMS.Data
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual int Insert(T dataInfo)
        {
            return RepositoryUtils.InsertObject(DatabaseType, ConnectionString, TableName, TableColumns, dataInfo);
        }
    }
}
