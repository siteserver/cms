using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SS.CMS.Data.Utils;

namespace SS.CMS.Data
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual async Task<int> InsertAsync(T dataInfo)
        {
            return await RepositoryUtils.InsertObjectAsync(Database, TableName, TableColumns, dataInfo);
        }

        public virtual async Task BulkInsertAsync(IEnumerable<T> items)
        {
            await RepositoryUtils.BulkInsertAsync(Database, TableName, TableColumns, items);
        }
    }
}
