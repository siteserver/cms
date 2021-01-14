using System;
using SqlKata;

namespace Datory
{
    public static partial class Q
    {
        public static Query Select(params string[] columns)
        {
            return NewQuery().Select(columns);
        }

        public static Query SelectRaw(string sql, params object[] bindings)
        {
            return NewQuery().SelectRaw(sql, bindings);
        }

        public static Query Select(Query query, string alias)
        {
            return NewQuery().Select(query, alias);
        }

        public static Query Select(Func<Query, Query> callback, string alias)
        {
            return NewQuery().Select(callback, alias);
        }
    }
}
