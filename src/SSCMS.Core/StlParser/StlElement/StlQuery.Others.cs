using SqlKata;
using SSCMS.Core.StlParser.Models;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    public static partial class StlQuery
    {
        private static void Select(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Value))
            {
                query.Select(ListUtils.GetStringList(queryInfo.Value).ToArray());
            }
        }

        private static void From(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Value))
            {
                query.From(queryInfo.Value);
            }
        }

        private static void Limit(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Value))
            {
                query.Limit(TranslateUtils.ToInt(queryInfo.Value));
            }
        }

        private static void Offset(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Value))
            {
                query.Offset(TranslateUtils.ToInt(queryInfo.Value));
            }
        }

        private static void Take(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Value))
            {
                query.Take(TranslateUtils.ToInt(queryInfo.Value));
            }
        }

        private static void Skip(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Value))
            {
                query.Skip(TranslateUtils.ToInt(queryInfo.Value));
            }
        }

        private static void ForPage(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Value))
            {
                var pair = ListUtils.GetIntList(queryInfo.Value);
                if (pair != null && pair.Count == 2)
                {
                    query.ForPage(pair[0], pair[1]);
                }
                else
                {
                    query.ForPage(TranslateUtils.ToInt(queryInfo.Value));
                }
            }
        }

        private static void Distinct(Query query)
        {
            query.Distinct();
        }

        private static void OrderBy(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.OrderBy(queryInfo.Column);
            }
        }

        private static void OrderByDesc(Query query, QueryInfo queryInfo)
        {
            if (!string.IsNullOrEmpty(queryInfo.Column))
            {
                query.OrderByDesc(queryInfo.Column);
            }
        }

        private static void OrderByRandom(Query query)
        {
            query.OrderByRandom(StringUtils.GetShortGuid(false));
        }
    }
}
