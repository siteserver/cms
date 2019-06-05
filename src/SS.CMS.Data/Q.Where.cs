using System;
using System.Collections.Generic;
using SqlKata;

namespace SS.CMS.Data
{
    public static partial class Q
    {
        public static Query Where(string column, string op, object value)
        {
            return NewQuery().Where(column, op, value);
        }

        public static Query WhereNot(string column, string op, object value)
        {
            return NewQuery().WhereNot(column, op, value);
        }

        public static Query OrWhere(string column, string op, object value)
        {
            return NewQuery().OrWhere(column, op, value);
        }

        public static Query OrWhereNot(string column, string op, object value)
        {
            return NewQuery().OrWhereNot(column, op, value);
        }

        public static Query Where(string column, object value)
        {
            return NewQuery().Where(column, value);
        }

        public static Query WhereNot(string column, object value)
        {
            return NewQuery().WhereNot(column, value);
        }

        public static Query OrWhere(string column, object value)
        {
            return NewQuery().OrWhere(column, value);
        }

        public static Query OrWhereNot(string column, object value)
        {
            return NewQuery().OrWhereNot(column, value);
        }

        public static Query Where(object constraints)
        {
            return NewQuery().Where(constraints);
        }

        public static Query Where(IReadOnlyDictionary<string, object> values)
        {
            return NewQuery().Where(values);
        }

        public static Query WhereRaw(string sql, params object[] bindings)
        {
            return NewQuery().WhereRaw(sql, bindings);
        }

        public static Query OrWhereRaw(string sql, params object[] bindings)
        {
            return NewQuery().OrWhereRaw(sql, bindings);
        }

        public static Query Where(Func<Query, Query> callback)
        {
            return NewQuery().Where(callback);
        }

        public static Query WhereNot(Func<Query, Query> callback)
        {
            return NewQuery().WhereNot(callback);
        }

        public static Query OrWhere(Func<Query, Query> callback)
        {
            return NewQuery().OrWhere(callback);
        }

        public static Query OrWhereNot(Func<Query, Query> callback)
        {
            return NewQuery().OrWhereNot(callback);
        }

        public static Query WhereColumns(string first, string op, string second)
        {
            return NewQuery().WhereColumns(first, op, second);
        }

        public static Query OrWhereColumns(string first, string op, string second)
        {
            return NewQuery().OrWhereColumns(first, op, second);
        }

        public static Query WhereNull(string column)
        {
            return NewQuery().WhereNull(column);
        }

        public static Query WhereNotNull(string column)
        {
            return NewQuery().WhereNotNull(column);
        }

        public static Query OrWhereNull(string column)
        {
            return NewQuery().OrWhereNull(column);
        }

        public static Query OrWhereNotNull(string column)
        {
            return NewQuery().OrWhereNotNull(column);
        }

        public static Query WhereTrue(string column)
        {
            return NewQuery().WhereTrue(column);
        }

        public static Query OrWhereTrue(string column)
        {
            return NewQuery().OrWhereTrue(column);
        }

        public static Query WhereFalse(string column)
        {
            return NewQuery().WhereFalse(column);
        }

        public static Query OrWhereFalse(string column)
        {
            return NewQuery().OrWhereFalse(column);
        }

        public static Query WhereLike(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereLike(column, value, caseSensitive);
        }

        public static Query WhereNotLike(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereNotLike(column, value, caseSensitive);
        }

        public static Query OrWhereLike(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereLike(column, value, caseSensitive);
        }

        public static Query OrWhereNotLike(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereNotLike(column, value, caseSensitive);
        }
        public static Query WhereStarts(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereStarts(column, value, caseSensitive);
        }

        public static Query WhereNotStarts(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereNotStarts(column, value, caseSensitive);
        }

        public static Query OrWhereStarts(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereStarts(column, value, caseSensitive);
        }

        public static Query OrWhereNotStarts(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereNotStarts(column, value, caseSensitive);
        }

        public static Query WhereEnds(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereEnds(column, value, caseSensitive);
        }

        public static Query WhereNotEnds(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereNotEnds(column, value, caseSensitive);
        }

        public static Query OrWhereEnds(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereEnds(column, value, caseSensitive);
        }

        public static Query OrWhereNotEnds(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereNotEnds(column, value, caseSensitive);
        }

        public static Query WhereContains(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereContains(column, value, caseSensitive);
        }

        public static Query WhereNotContains(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().WhereNotContains(column, value, caseSensitive);
        }

        public static Query OrWhereContains(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereContains(column, value, caseSensitive);
        }

        public static Query OrWhereNotContains(string column, string value, bool caseSensitive = false)
        {
            return NewQuery().OrWhereNotContains(column, value, caseSensitive);
        }

        public static Query WhereBetween<T>(string column, T lower, T higher)
        {
            return NewQuery().WhereBetween(column, lower, higher);
        }

        public static Query OrWhereBetween<T>(string column, T lower, T higher)
        {
            return NewQuery().OrWhereBetween(column, lower, higher);
        }
        public static Query WhereNotBetween<T>(string column, T lower, T higher)
        {
            return NewQuery().WhereNotBetween(column, lower, higher);
        }
        public static Query OrWhereNotBetween<T>(string column, T lower, T higher)
        {
            return NewQuery().OrWhereNotBetween(column, lower, higher);
        }

        public static Query WhereIn<T>(string column, IEnumerable<T> values)
        {
            return NewQuery().WhereIn(column, values);
        }

        public static Query OrWhereIn<T>(string column, IEnumerable<T> values)
        {
            return NewQuery().OrWhereIn(column, values);
        }

        public static Query WhereNotIn<T>(string column, IEnumerable<T> values)
        {
            return NewQuery().WhereNotIn(column, values);
        }

        public static Query OrWhereNotIn<T>(string column, IEnumerable<T> values)
        {
            return NewQuery().OrWhereNotIn(column, values);
        }

        public static Query WhereIn(string column, Query query2)
        {
            return NewQuery().WhereIn(column, query2);
        }

        public static Query WhereIn(string column, Func<Query, Query> callback)
        {
            return NewQuery().WhereIn(column, callback);
        }

        public static Query OrWhereIn(string column, Query query2)
        {
            return NewQuery().OrWhereIn(column, query2);
        }

        public static Query OrWhereIn(string column, Func<Query, Query> callback)
        {
            return NewQuery().OrWhereIn(column, callback);
        }

        public static Query WhereNotIn(string column, Query query2)
        {
            return NewQuery().WhereNotIn(column, query2);
        }

        public static Query WhereNotIn(string column, Func<Query, Query> callback)
        {
            return NewQuery().WhereNotIn(column, callback);
        }

        public static Query OrWhereNotIn(string column, Query query2)
        {
            return NewQuery().OrWhereNotIn(column, query2);
        }

        public static Query OrWhereNotIn(string column, Func<Query, Query> callback)
        {
            return NewQuery().OrWhereNotIn(column, callback);
        }

        public static Query Where(string column, string op, Func<Query, Query> callback)
        {
            return NewQuery().Where(column, op, callback);
        }

        public static Query Where(string column, string op, Query query2)
        {
            return NewQuery().Where(column, op, query2);
        }

        public static Query OrWhere(string column, string op, Query query2)
        {
            return NewQuery().OrWhere(column, op, query2);
        }

        public static Query OrWhere(string column, string op, Func<Query, Query> callback)
        {
            return NewQuery().OrWhere(column, op, callback);
        }

        public static Query WhereExists(Query query2)
        {
            return NewQuery().WhereExists(query2);
        }

        public static Query WhereExists(Func<Query, Query> callback)
        {
            return NewQuery().WhereExists(callback);
        }

        public static Query WhereNotExists(Query query2)
        {
            return NewQuery().WhereNotExists(query2);
        }

        public static Query WhereNotExists(Func<Query, Query> callback)
        {
            return NewQuery().WhereNotExists(callback);
        }

        public static Query OrWhereExists(Query query2)
        {
            return NewQuery().OrWhereExists(query2);
        }

        public static Query OrWhereExists(Func<Query, Query> callback)
        {
            return NewQuery().OrWhereExists(callback);
        }

        public static Query OrWhereNotExists(Query query2)
        {
            return NewQuery().OrWhereNotExists(query2);
        }

        public static Query OrWhereNotExists(Func<Query, Query> callback)
        {
            return NewQuery().OrWhereNotExists(callback);
        }

        public static Query WhereDatePart(string part, string column, string op, object value)
        {
            return NewQuery().WhereDatePart(part, column, op, value);
        }

        public static Query WhereNotDatePart(string part, string column, string op, object value)
        {
            return NewQuery().WhereNotDatePart(part, column, op, value);
        }

        public static Query OrWhereDatePart(string part, string column, string op, object value)
        {
            return NewQuery().OrWhereDatePart(part, column, op, value);
        }

        public static Query OrWhereNotDatePart(string part, string column, string op, object value)
        {
            return NewQuery().OrWhereNotDatePart(part, column, op, value);
        }

        public static Query WhereDate(string column, string op, object value)
        {
            return NewQuery().WhereDate(column, op, value);
        }

        public static Query WhereNotDate(string column, string op, object value)
        {
            return NewQuery().WhereNotDate(column, op, value);
        }

        public static Query OrWhereDate(string column, string op, object value)
        {
            return NewQuery().OrWhereDate(column, op, value);
        }

        public static Query OrWhereNotDate(string column, string op, object value)
        {
            return NewQuery().OrWhereNotDate(column, op, value);
        }

        public static Query WhereTime(string column, string op, object value)
        {
            return NewQuery().WhereTime(column, op, value);
        }

        public static Query WhereNotTime(string column, string op, object value)
        {
            return NewQuery().WhereNotTime(column, op, value);
        }

        public static Query OrWhereTime(string column, string op, object value)
        {
            return NewQuery().OrWhereTime(column, op, value);
        }

        public static Query OrWhereNotTime(string column, string op, object value)
        {
            return NewQuery().OrWhereNotTime(column, op, value);
        }

        public static Query WhereDatePart(string part, string column, object value)
        {
            return NewQuery().WhereDatePart(part, column, value);
        }

        public static Query WhereNotDatePart(string part, string column, object value)
        {
            return NewQuery().WhereNotDatePart(part, column, value);
        }

        public static Query OrWhereDatePart(string part, string column, object value)
        {
            return NewQuery().OrWhereDatePart(part, column, value);
        }

        public static Query OrWhereNotDatePart(string part, string column, object value)
        {
            return NewQuery().OrWhereNotDatePart(part, column, value);
        }

        public static Query WhereDate(string column, object value)
        {
            return NewQuery().WhereDate(column, value);
        }

        public static Query WhereNotDate(string column, object value)
        {
            return NewQuery().WhereNotDate(column, value);
        }

        public static Query OrWhereDate(string column, object value)
        {
            return NewQuery().OrWhereDate(column, value);
        }

        public static Query OrWhereNotDate(string column, object value)
        {
            return NewQuery().OrWhereNotDate(column, value);
        }

        public static Query WhereTime(string column, object value)
        {
            return NewQuery().WhereTime(column, value);
        }

        public static Query WhereNotTime(string column, object value)
        {
            return NewQuery().WhereNotTime(column, value);
        }

        public static Query OrWhereTime(string column, object value)
        {
            return NewQuery().OrWhereTime(column, value);
        }

        public static Query OrWhereNotTime(string column, object value)
        {
            return NewQuery().OrWhereNotTime(column, value);
        }
    }
}
