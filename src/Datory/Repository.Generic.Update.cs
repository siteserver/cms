using System;
using System.Threading.Tasks;
using SqlKata;
using Datory.Utils;

namespace Datory
{
    public partial class Repository<T> where T : Entity, new()
    {
        public virtual async Task<bool> UpdateAsync(T dataInfo, Query query = null)
        {
            if (dataInfo == null || dataInfo.Id <= 0) return false;

            if (!Utilities.IsGuid(dataInfo.Guid))
            {
                dataInfo.Guid = Utilities.GetGuid();
            }
            dataInfo.LastModifiedDate = DateTime.Now;

            var xQuery = RepositoryUtils.NewQuery(TableName, query);
            xQuery.ClearComponent("update");
            xQuery.Where(nameof(Entity.Id), dataInfo.Id);

            foreach (var tableColumn in TableColumns)
            {
                if (Utilities.EqualsIgnoreCase(tableColumn.AttributeName, nameof(Entity.Id))) continue;

                var value = ValueUtils.GetSqlValue(dataInfo, tableColumn);

                xQuery.Set(tableColumn.AttributeName, value);
            }

            return await RepositoryUtils.UpdateAllAsync(Database, TableName, Redis, xQuery) > 0;
        }

        public virtual async Task<bool> UpdateAsync(T dataInfo, string[] columnNames, Query query = null)
        {
            if (dataInfo.Id > 0)
            {
                var xQuery = RepositoryUtils.NewQuery(TableName, query);
                xQuery.ClearComponent("update");
                xQuery.Where(nameof(Entity.Id), dataInfo.Id);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id))) continue;
                    xQuery.Set(columnName, ValueUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}
                return await RepositoryUtils.UpdateAllAsync(Database, TableName, Redis, xQuery) > 0;
            }
            if (Utilities.IsGuid(dataInfo.Guid))
            {
                var xQuery = RepositoryUtils.NewQuery(TableName, query);
                xQuery.ClearComponent("update");
                xQuery.Where(nameof(Entity.Guid), dataInfo.Guid);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id)) ||
                        Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Guid))) continue;
                    xQuery.Set(columnName, ValueUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id), nameof(IEntity.Guid));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}

                return await RepositoryUtils.UpdateAllAsync(Database, TableName, Redis, xQuery) > 0;
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
