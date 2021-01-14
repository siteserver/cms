using System;
using System.Threading.Tasks;
using SqlKata;
using Datory.Utils;

namespace Datory
{
    public partial class Repository
    {
        public virtual async Task<bool> UpdateAsync<T>(T dataInfo) where T : Entity
        {
            if (dataInfo == null || dataInfo.Id <= 0) return false;

            if (!Utilities.IsGuid(dataInfo.Guid))
            {
                dataInfo.Guid = Utilities.GetGuid();
            }
            dataInfo.LastModifiedDate = DateTime.Now;

            var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

            foreach (var tableColumn in TableColumns)
            {
                if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id))) continue;

                var value = ValueUtils.GetSqlValue(dataInfo, tableColumn);

                query.Set(tableColumn.AttributeName, value);
            }

            return await RepositoryUtils.UpdateAllAsync(Database, TableName, Redis, query) > 0;
        }

        public virtual async Task<bool> UpdateAsync<T>(T dataInfo, params string[] columnNames) where T : Entity
        {
            if (dataInfo.Id > 0)
            {
                var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id))) continue;
                    query.Set(columnName, ValueUtils.GetValue(dataInfo, columnName));
                }

                return await RepositoryUtils.UpdateAllAsync(Database, TableName, Redis, query) > 0;
            }
            if (Utilities.IsGuid(dataInfo.Guid))
            {
                var query = Q.Where(nameof(Entity.Guid), dataInfo.Guid);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id)) ||
                        Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Guid))) continue;
                    query.Set(columnName, ValueUtils.GetValue(dataInfo, columnName));
                }

                return await RepositoryUtils.UpdateAllAsync(Database, TableName, Redis, query) > 0;
            }

            return false;
        }

        public virtual async Task<int> UpdateAsync(Query query)
        {
            return await RepositoryUtils.UpdateAllAsync(Database, TableName, Redis, query);
        }

        public virtual async Task<int> IncrementAsync(string columnName, Query query, int num = 1)
        {
            return await RepositoryUtils.IncrementAllAsync(Database, TableName, Redis, columnName, query, num);
        }

        public virtual async Task<int> DecrementAsync(string columnName, Query query, int num = 1)
        {
            return await RepositoryUtils.DecrementAllAsync(Database, TableName, Redis, columnName, query, num);
        }
    }
}
