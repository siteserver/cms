using System;
using System.Collections.Generic;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.CMS.Database.Core
{
    public abstract class GenericRepositoryAbstract: IRepository
    {
        public abstract string TableName { get; }
        public abstract List<TableColumn> TableColumns { get; }

        protected Query Q => new Query();

        protected bool Exists(int id)
        {
            return id > 0 && Exists(Q.Where(nameof(DynamicEntity.Id), id));
        }

        protected bool Exists(string guid)
        {
            return StringUtils.IsGuid(guid) && Exists(Q.Where(nameof(DynamicEntity.Guid), guid));
        }

        protected bool Exists(Query query = null)
        {
            return RepositoryHelper.Exists(TableName, query);
        }

        protected int Count(Query query = null)
        {
            return RepositoryHelper.Count(TableName, query);
        }

        protected int Sum(string column, Query query = null)
        {
            return RepositoryHelper.Sum(TableName, column, query);
        }

        protected TValue GetValue<TValue>(Query query)
        {
            return RepositoryHelper.GetValue<TValue>(TableName, query);
        }

        protected IList<TValue> GetValueList<TValue>(Query query = null)
        {
            return RepositoryHelper.GetValueList<TValue>(TableName, query);
        }

        protected int Max(string columnName, Query query = null)
        {
            return RepositoryHelper.Max(TableName, columnName, query);
        }

        protected T GetObjectById<T>(int id) where T : DynamicEntity
        {
            return id <= 0 ? null : GetObject<T>(Q.Where(nameof(DynamicEntity.Id), id));
        }

        protected T GetObjectByGuid<T>(string guid) where T : DynamicEntity
        {
            return !StringUtils.IsGuid(guid) ? null : GetObject<T>(Q.Where(nameof(DynamicEntity.Guid), guid));
        }

        protected T GetObject<T>(Query query = null) where T : DynamicEntity
        {
            return RepositoryHelper.GetObject<T>(TableName, query);
        }

        protected IList<T> GetObjectList<T>(Query query = null) where T : DynamicEntity
        {
            return RepositoryHelper.GetObjectList<T>(TableName, query);
        }

        protected int InsertObject<T>(T dataInfo) where T : DynamicEntity
        {
            return RepositoryHelper.InsertObject(TableName, TableColumns, dataInfo);
        }

        protected bool UpdateObject<T>(T dataInfo) where T : DynamicEntity
        {
            if (dataInfo == null || dataInfo.Id <= 0) return false;

            if (!StringUtils.IsGuid(dataInfo.Guid))
            {
                dataInfo.Guid = StringUtils.GetGuid();
            }
            dataInfo.LastModifiedDate = DateTime.Now;

            var query = Q.Where(nameof(DynamicEntity.Id), dataInfo.Id);
            
            foreach (var tableColumn in TableColumns)
            {
                if (StringUtils.EqualsIgnoreCase(tableColumn.AttributeName, nameof(DynamicEntity.Id))) continue;

                var value = tableColumn.IsExtend
                    ? TranslateUtils.JsonSerialize(dataInfo.ToDictionary(dataInfo.GetColumnNames()))
                    : dataInfo.Get(tableColumn.AttributeName);

                query.Set(tableColumn.AttributeName, value);
            }

            //var values = RepositoryUtils.ObjToDict(dataInfo, TableColumns.Select(x => x.AttributeName).ToList(), nameof(IEntity.Id));
            //foreach (var value in values)
            //{
            //    query.Set(value.Key, value.Value);
            //}

            return RepositoryHelper.UpdateAll(TableName, query) > 0;

            //using (var connection = GetConnection())
            //{
            //    updated = connection.Update(values);
            //}

            //return updated;
        }

        //protected bool UpdateById(IDictionary<string, object> values, int id)
        //{
        //    if (id <= 0) return false;

        //    return UpdateValue(values, Q.Where(nameof(IDataInfo.Id), id)) > 0;
        //}

        //protected bool UpdateByGuid(IDictionary<string, object> values, string guid)
        //{
        //    if (!StringUtils.IsGuid(guid)) return false;

        //    return UpdateValue(values, Q.Where(nameof(IDataInfo.Guid), guid)) > 0;
        //}

        protected bool UpdateObject<T>(T dataInfo, params string[] columnNames) where T : DynamicEntity
        {
            if (dataInfo.Id > 0)
            {
                var query = Q.Where(nameof(DynamicEntity.Id), dataInfo.Id);

                foreach (var columnName in columnNames)
                {
                    if (StringUtils.EqualsIgnoreCase(columnName, nameof(DynamicEntity.Id))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}
                return RepositoryHelper.UpdateAll(TableName, query) > 0;
            }
            if (StringUtils.IsGuid(dataInfo.Guid))
            {
                var query = Q.Where(nameof(DynamicEntity.Guid), dataInfo.Guid);

                foreach (var columnName in columnNames)
                {
                    if (StringUtils.EqualsIgnoreCase(columnName, nameof(DynamicEntity.Id)) || 
                        StringUtils.EqualsIgnoreCase(columnName, nameof(DynamicEntity.Guid))) continue;
                    query.Set(columnName, ReflectionUtils.GetValue(dataInfo, columnName));
                }

                //var values = RepositoryUtils.ObjToDict(dataInfo, columnNames, nameof(IEntity.Id), nameof(IEntity.Guid));
                //foreach (var value in values)
                //{
                //    query.Set(value.Key, value.Value);
                //}
                
                return RepositoryHelper.UpdateAll(TableName, query) > 0;
            }

            return false;
        }

        protected int UpdateAll(Query query)
        {
            return RepositoryHelper.UpdateAll(TableName, query);
        }

        protected bool DeleteById(int id)
        {
            if (id <= 0) return false;

            return DeleteAll(Q.Where(nameof(DynamicEntity.Id), id)) > 0;
        }

        protected bool DeleteByGuid(string guid)
        {
            if (!StringUtils.IsGuid(guid)) return false;

            return DeleteAll(Q.Where(nameof(DynamicEntity.Guid), guid)) > 0;
        }

        protected int DeleteAll(Query query = null)
        {
            return RepositoryHelper.DeleteAll(TableName, query);
        }

        protected int IncrementAll(string columnName, Query query, int num = 1)
        {
            return RepositoryHelper.IncrementAll(TableName, columnName, query, num);
        }

        protected int DecrementAll(string columnName, Query query, int num = 1)
        {
            return RepositoryHelper.DecrementAll(TableName, columnName, query, num);
        }
    }
}
