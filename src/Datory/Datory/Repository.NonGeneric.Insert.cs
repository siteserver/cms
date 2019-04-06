using Datory.Utils;

namespace Datory
{
    public partial class Repository
    {
        public virtual int Insert<T>(T dataInfo) where T : Entity
        {
            return RepositoryUtils.InsertObject(DatabaseType, ConnectionString, TableName, TableColumns, dataInfo);
        }
    }
}
