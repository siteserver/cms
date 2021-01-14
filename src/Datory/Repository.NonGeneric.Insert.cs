using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Datory.Utils;

namespace Datory
{
    public partial class Repository
    {
        public virtual async Task<int> InsertAsync<T>(T dataInfo) where T : Entity
        {
            return await RepositoryUtils.InsertObjectAsync(Database, TableName, TableColumns, Redis, dataInfo);
        }

        public virtual async Task BulkInsertAsync<T>(IEnumerable<T> items) where T : Entity
        {
            await RepositoryUtils.BulkInsertAsync<T>(Database, TableName, TableColumns, items);
        }

        public virtual async Task BulkInsertAsync(IEnumerable<JObject> items)
        {
            await RepositoryUtils.BulkInsertAsync(Database, TableName, TableColumns, items);
        }

        public virtual async Task BulkInsertAsync(IEnumerable<IDictionary<string, object>> items)
        {
            await RepositoryUtils.BulkInsertAsync(Database, TableName, TableColumns, items);
        }
    }
}
