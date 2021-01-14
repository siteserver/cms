using System;
using System.Collections.Generic;
using SqlKata;
using Datory.Utils;

namespace Datory
{
    public static partial class Q
    {
        public static Query NewQuery()
        {
            return new Query();
        }

        public static Query Set(string column, Enum value)
        {
            return Set(column, value.GetValue());
        }

        public static Query Set(string column, List<int> list)
        {
            return Set(column, Utilities.ToString(list));
        }

        public static Query Set(string column, List<string> list)
        {
            return Set(column, Utilities.ToString(list));
        }

        public static Query Set( string column, object value)
        {
            return NewQuery().Set(column, value);
        }

        public static Query Set(this Query query, string column, Enum value)
        {
            return Set(query, column, value.GetValue());
        }

        public static Query Set(this Query query, string column, List<int> list)
        {
            return Set(query, column, Utilities.ToString(list));
        }

        public static Query Set(this Query query, string column, List<string> list)
        {
            return Set(query, column, Utilities.ToString(list));
        }

        public static Query Set(this Query query, string column, object value)
        {
            if (Utilities.EqualsIgnoreCase(column, nameof(Entity.Id)) ||
                Utilities.EqualsIgnoreCase(column, nameof(Entity.LastModifiedDate))) return query;

            query.AddComponent("update", new BasicCondition
            {
                Column = column,
                Operator = "=",
                Value = value
            });

            return query;
        }

        public static Query SetRaw(string sql, params object[] bindings)
        {
            return NewQuery().SetRaw(sql, bindings);
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

        public static Query Limit(int value)
        {
            return NewQuery().Limit(value);
        }

        public static Query Offset(int value)
        {
            return NewQuery().Offset(value);
        }

        /// <summary>
        /// Alias for Limit
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static Query Take(int limit)
        {
            return NewQuery().Take(limit);
        }

        /// <summary>
        /// Alias for Offset
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static Query Skip(int offset)
        {
            return NewQuery().Skip(offset);
        }

        /// <summary>
        /// Set the limit and offset for a given page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public static Query ForPage(int page, int perPage = 15)
        {
            return NewQuery().ForPage(page, perPage);
        }

        public static Query Distinct()
        {
            return NewQuery().Distinct();
        }

        /// <summary>
        /// Apply the callback's query changes if the given "condition" is true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="whenTrue">Invoked when the condition is true</param>
        /// <param name="whenFalse">Optional, invoked when the condition is false</param>
        /// <returns></returns>
        public static Query When(bool condition, Func<Query, Query> whenTrue, Func<Query, Query> whenFalse = null)
        {
            return NewQuery().When(condition, whenTrue, whenFalse);
        }

        /// <summary>
        /// Apply the callback's query changes if the given "condition" is false.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static Query WhenNot(bool condition, Func<Query, Query> callback)
        {
            return NewQuery().WhenNot(condition, callback);
        }

        public static Query OrderBy(params string[] columns)
        {
            return NewQuery().OrderBy(columns);
        }

        public static Query OrderByDesc(params string[] columns)
        {
            return NewQuery().OrderByDesc(columns);
        }

        public static Query OrderByRaw(string expression, params object[] bindings)
        {
            return NewQuery().OrderByRaw(expression, bindings);
        }

        public static Query OrderByRandom(string seed)
        {
            return NewQuery().OrderByRandom(seed);
        }

        public static Query GroupBy(params string[] columns)
        {
            return NewQuery().GroupBy(columns);
        }

        public static Query GroupByRaw(string expression, params object[] bindings)
        {
            return NewQuery().GroupByRaw(expression, bindings);
        }

        /// <summary>
        /// Set the next boolean operator to "or" for the "where" clause.
        /// </summary>
        /// <returns></returns>
        public static Query Or()
        {
            return NewQuery().Or();
        }

        /// <summary>
        /// Set the next "not" operator for the "where" clause.
        /// </summary>
        /// <returns></returns>
        public static Query Not(bool flag = true)
        {
            return NewQuery().Not(flag);
        }


        /// <summary>
        /// Add a from Clause
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static Query From(string table)
        {
            return NewQuery().From(table);
        }

        public static Query From(Query query, string alias = null)
        {
            return NewQuery().From(query, alias);
        }

        public static Query FromRaw(string sql, params object[] bindings)
        {
            return NewQuery().FromRaw(sql, bindings);
        }

        public static Query From(Func<Query, Query> callback, string alias = null)
        {
            return NewQuery().From(callback, alias);
        }

        public static Query CachingGet(string cacheKey)
        {
            return NewQuery().CachingGet(cacheKey);
        }

        public static Query CachingRemove(params string[] cacheKeys)
        {
            return NewQuery().CachingRemove(cacheKeys);
        }

        public static Query AllowIdentityInsert()
        {
            return NewQuery().AllowIdentityInsert();
        }
    }
}
