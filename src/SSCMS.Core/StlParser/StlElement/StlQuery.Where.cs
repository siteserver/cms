using System.Linq;
using SqlKata;
using SSCMS.Core.StlParser.Models;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    public static partial class StlQuery
    {
        private static void Where(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                var value = GetValue(queryInfo.DataType, queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.Where(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.Where(queryInfo.Column, value);
                }
            }
            else if (queryInfo.Queries != null && queryInfo.Queries.Count > 0)
            {
                query.Where(q =>
                {
                    foreach (var info in queryInfo.Queries)
                    {
                        q.AddQuery(info);
                    }
                    return q;
                });
            }
        }

        private static void WhereNot(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                var value = GetValue(queryInfo.DataType, queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.WhereNot(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.WhereNot(queryInfo.Column, value);
                }
            }
            else if (queryInfo.Queries != null && queryInfo.Queries.Count > 0)
            {
                query.WhereNot(q =>
                {
                    foreach (var info in queryInfo.Queries)
                    {
                        q.AddQuery(info);
                    }
                    return q;
                });
            }
        }

        private static void OrWhere(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                var value = GetValue(queryInfo.DataType, queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.OrWhere(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.OrWhere(queryInfo.Column, value);
                }
            }
            else if (queryInfo.Queries != null && queryInfo.Queries.Count > 0)
            {
                query.OrWhere(q =>
                {
                    foreach (var info in queryInfo.Queries)
                    {
                        q.AddQuery(info);
                    }
                    return q;
                });
            }
        }

        private static void OrWhereNot(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                var value = GetValue(queryInfo.DataType, queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.OrWhereNot(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.OrWhereNot(queryInfo.Column, value);
                }
            }
            else if (queryInfo.Queries != null && queryInfo.Queries.Count > 0)
            {
                query.OrWhereNot(q =>
                {
                    foreach (var info in queryInfo.Queries)
                    {
                        q.AddQuery(info);
                    }
                    return q;
                });
            }
        }

        private static void WhereColumns(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Op) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereColumns(queryInfo.Column, queryInfo.Op, queryInfo.Value);
            }
        }

        private static void OrWhereColumns(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Op) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereColumns(queryInfo.Column, queryInfo.Op, queryInfo.Value);
            }
        }

        private static void WhereNull(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.WhereNull(queryInfo.Column);
            }
        }

        private static void WhereNotNull(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.WhereNotNull(queryInfo.Column);
            }
        }

        private static void OrWhereNull(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.OrWhereNull(queryInfo.Column);
            }
        }

        private static void OrWhereNotNull(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.OrWhereNotNull(queryInfo.Column);
            }
        }

        private static void WhereTrue(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.WhereTrue(queryInfo.Column);
            }
        }

        private static void OrWhereTrue(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.OrWhereTrue(queryInfo.Column);
            }
        }

        private static void WhereFalse(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.WhereFalse(queryInfo.Column);
            }
        }

        private static void OrWhereFalse(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.OrWhereFalse(queryInfo.Column);
            }
        }

        private static void WhereLike(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereLike(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereNotLike(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereNotLike(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereLike(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereLike(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereNotLike(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereNotLike(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereStarts(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereStarts(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereNotStarts(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereNotStarts(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereStarts(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereStarts(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereNotStarts(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereNotStarts(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereEnds(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereEnds(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereNotEnds(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereNotEnds(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereEnds(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereEnds(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereNotEnds(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereNotEnds(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereContains(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereContains(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereNotContains(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.WhereNotContains(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereContains(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereContains(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void OrWhereNotContains(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                query.OrWhereNotContains(queryInfo.Column, queryInfo.Value);
            }
        }

        private static void WhereBetween(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var pair = ListUtils.GetStringList(queryInfo.Value);
                if (pair != null && pair.Count == 2)
                {
                    var start = GetValue(queryInfo.DataType, pair[0]);
                    var end = GetValue(queryInfo.DataType, pair[1]);
                    query.WhereBetween(queryInfo.Column, start, end);
                }
            }
        }

        private static void OrWhereBetween(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var pair = ListUtils.GetStringList(queryInfo.Value);
                if (pair != null && pair.Count == 2)
                {
                    var start = GetValue(queryInfo.DataType, pair[0]);
                    var end = GetValue(queryInfo.DataType, pair[1]);
                    query.OrWhereBetween(queryInfo.Column, start, end);
                }
            }
        }

        private static void WhereNotBetween(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var pair = ListUtils.GetStringList(queryInfo.Value);
                if (pair != null && pair.Count == 2)
                {
                    var start = GetValue(queryInfo.DataType, pair[0]);
                    var end = GetValue(queryInfo.DataType, pair[1]);
                    query.WhereNotBetween(queryInfo.Column, start, end);
                }
            }
        }

        private static void OrWhereNotBetween(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var pair = ListUtils.GetStringList(queryInfo.Value);
                if (pair != null && pair.Count == 2)
                {
                    var start = GetValue(queryInfo.DataType, pair[0]);
                    var end = GetValue(queryInfo.DataType, pair[1]);
                    query.OrWhereNotBetween(queryInfo.Column, start, end);
                }
            }
        }

        private static void WhereIn(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                if (queryInfo.DataType == Datory.DataType.VarChar)
                {
                    query.WhereIn(queryInfo.Column, ListUtils.GetStringList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.Integer)
                {
                    query.WhereIn(queryInfo.Column, ListUtils.GetIntList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.DateTime)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(TranslateUtils.ToDateTime);
                    query.WhereIn(queryInfo.Column, list);
                }
                else if (queryInfo.DataType == Datory.DataType.Decimal)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(x => TranslateUtils.ToDecimal(x));
                    query.WhereIn(queryInfo.Column, list);
                }
            }
        }

        private static void OrWhereIn(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                if (queryInfo.DataType == Datory.DataType.VarChar)
                {
                    query.OrWhereIn(queryInfo.Column, ListUtils.GetStringList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.Integer)
                {
                    query.OrWhereIn(queryInfo.Column, ListUtils.GetIntList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.DateTime)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(TranslateUtils.ToDateTime);
                    query.OrWhereIn(queryInfo.Column, list);
                }
                else if (queryInfo.DataType == Datory.DataType.Decimal)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(x => TranslateUtils.ToDecimal(x));
                    query.OrWhereIn(queryInfo.Column, list);
                }
            }
        }

        private static void WhereNotIn(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                if (queryInfo.DataType == Datory.DataType.VarChar)
                {
                    query.WhereNotIn(queryInfo.Column, ListUtils.GetStringList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.Integer)
                {
                    query.WhereNotIn(queryInfo.Column, ListUtils.GetIntList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.DateTime)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(TranslateUtils.ToDateTime);
                    query.WhereNotIn(queryInfo.Column, list);
                }
                else if (queryInfo.DataType == Datory.DataType.Decimal)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(x => TranslateUtils.ToDecimal(x));
                    query.WhereNotIn(queryInfo.Column, list);
                }
            }
        }

        private static void OrWhereNotIn(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                if (queryInfo.DataType == Datory.DataType.VarChar)
                {
                    query.OrWhereNotIn(queryInfo.Column, ListUtils.GetStringList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.Integer)
                {
                    query.OrWhereNotIn(queryInfo.Column, ListUtils.GetIntList(queryInfo.Value));
                }
                else if (queryInfo.DataType == Datory.DataType.DateTime)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(TranslateUtils.ToDateTime);
                    query.OrWhereNotIn(queryInfo.Column, list);
                }
                else if (queryInfo.DataType == Datory.DataType.Decimal)
                {
                    var list = ListUtils.GetStringList(queryInfo.Value)
                        .Select(x => TranslateUtils.ToDecimal(x));
                    query.OrWhereNotIn(queryInfo.Column, list);
                }
            }
        }

        private static void WhereDate(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.WhereDate(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.WhereDate(queryInfo.Column, value);
                }
            }
        }

        private static void WhereNotDate(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.WhereNotDate(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.WhereNotDate(queryInfo.Column, value);
                }
            }
        }

        private static void OrWhereDate(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.OrWhereDate(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.OrWhereDate(queryInfo.Column, value);
                }
            }
        }

        private static void OrWhereNotDate(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.OrWhereNotDate(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.OrWhereNotDate(queryInfo.Column, value);
                }
            }
        }

        private static void WhereTime(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.WhereTime(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.WhereTime(queryInfo.Column, value);
                }
            }
        }

        private static void WhereNotTime(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.WhereNotTime(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.WhereNotTime(queryInfo.Column, value);
                }
            }
        }

        private static void OrWhereTime(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.OrWhereTime(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.OrWhereTime(queryInfo.Column, value);
                }
            }
        }

        private static void OrWhereNotTime(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column) && !string.IsNullOrEmpty(queryInfo.Value))
            {
                var value = TranslateUtils.ToDateTime(queryInfo.Value);
                if (!string.IsNullOrEmpty(queryInfo.Op))
                {
                    query.OrWhereNotTime(queryInfo.Column, queryInfo.Op, value);
                }
                else
                {
                    query.OrWhereNotTime(queryInfo.Column, value);
                }
            }
        }
    }
}
