using System;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Data.Utils;

namespace SS.CMS.Data
{
    public partial class Repository
    {
        public virtual bool Update<T>(T dataInfo) where T : Entity
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

                var value = tableColumn.IsExtend
                    ? Utilities.JsonSerialize(dataInfo.ToDictionary(dataInfo.GetColumnNames()))
                    : dataInfo.Get(tableColumn.AttributeName);

                query.Set(tableColumn.AttributeName, value);
            }

            return RepositoryUtils.UpdateAll(DbContext, TableName, query) > 0;
        }

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

                var value = tableColumn.IsExtend
                    ? Utilities.JsonSerialize(dataInfo.ToDictionary(dataInfo.GetColumnNames()))
                    : dataInfo.Get(tableColumn.AttributeName);

                query.Set(tableColumn.AttributeName, value);
            }

            return await RepositoryUtils.UpdateAllAsync(DbContext, TableName, query) > 0;
        }

        public virtual bool Update<T>(T dataInfo, params string[] columnNames) where T : Entity
        {
            if (dataInfo.Id > 0)
            {
                var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}
                return RepositoryUtils.UpdateAll(DbContext, TableName, query) > 0;
            }
            if (Utilities.IsGuid(dataInfo.Guid))
            {
                var query = Q.Where(nameof(Entity.Guid), dataInfo.Guid);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id)) ||
                        Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Guid))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id), nameof(IEntity.Guid));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}

                return RepositoryUtils.UpdateAll(DbContext, TableName, query) > 0;
            }

            return false;
        }

        public virtual async Task<bool> UpdateAsync<T>(T dataInfo, params string[] columnNames) where T : Entity
        {
            if (dataInfo.Id > 0)
            {
                var query = Q.Where(nameof(Entity.Id), dataInfo.Id);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                return await RepositoryUtils.UpdateAllAsync(DbContext, TableName, query) > 0;
            }
            if (Utilities.IsGuid(dataInfo.Guid))
            {
                var query = Q.Where(nameof(Entity.Guid), dataInfo.Guid);

                foreach (var columnName in columnNames)
                {
                    if (Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Id)) ||
                        Utilities.EqualsIgnoreCase(columnName, nameof(Entity.Guid))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                return await RepositoryUtils.UpdateAllAsync(DbContext, TableName, query) > 0;
            }

            return false;
        }

        public virtual int Update(Query query)
        {
            return RepositoryUtils.UpdateAll(DbContext, TableName, query);
        }

        public virtual async Task<int> UpdateAsync(Query query)
        {
            return await RepositoryUtils.UpdateAllAsync(DbContext, TableName, query);
        }

        public virtual int Increment(string columnName, Query query, int num = 1)
        {
            return RepositoryUtils.IncrementAll(DbContext, TableName, columnName, query, num);
        }

        public virtual async Task<int> IncrementAsync(string columnName, Query query, int num = 1)
        {
            return await RepositoryUtils.IncrementAllAsync(DbContext, TableName, columnName, query, num);
        }

        public virtual int Decrement(string columnName, Query query, int num = 1)
        {
            return RepositoryUtils.DecrementAll(DbContext, TableName, columnName, query, num);
        }

        public virtual async Task<int> DecrementAsync(string columnName, Query query, int num = 1)
        {
            return await RepositoryUtils.DecrementAllAsync(DbContext, TableName, columnName, query, num);
        }
    }
}
