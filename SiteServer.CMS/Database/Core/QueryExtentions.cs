using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.CMS.Database.Core
{
    public static class QueryExtentions
    {
        public static Query Set(this Query query, string column, object value)
        {
            if (StringUtils.EqualsIgnoreCase(column, nameof(DynamicEntity.Id)) ||
                StringUtils.EqualsIgnoreCase(column, nameof(DynamicEntity.LastModifiedDate))) return query;

            query.AddComponent("update", new BasicCondition
            {
                Column = column,
                Operator = "=",
                Value = value
            });

            return query;
        }

        public static Query SetRaw(this Query query, string sql, params object[] bindings)
        {
            if (bindings == null)
            {
                query.AddComponent("update", new RawCondition
                {
                    Expression = sql,
                    Bindings = bindings
                });
            }
            else
            {
                query.AddComponent("update", new RawCondition
                {
                    Expression = sql,
                    Bindings = null 
                });
            }

            return query;
        }

        //public static Query AsUpdateNew(this Query query, IDictionary<string, object> values = null)
        //{
        //    query.Method = "update";

        //    if (values != null)
        //    {
        //        foreach (var key in values.Keys)
        //        {
        //            if (StringUtils.EqualsIgnoreCase(key, nameof(IDataInfo.Id)) ||
        //                StringUtils.EqualsIgnoreCase(key, nameof(IDataInfo.LastModifiedDate))) continue;

        //            query.Set(key, values[key]);
        //        }
        //    }

        //    query.Set(nameof(IDataInfo.LastModifiedDate), DateTime.Now);

        //    return query;
        //}
    }
}
