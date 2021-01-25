using System.Collections.Generic;
using Datory;
using SqlKata;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Utils;
using SSCMS.Core.StlParser.Models;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "查询条件", Description = "通过 stl:query 标签在模板中设置列表标签查询条件")]
    public static partial class StlQuery
	{
		public const string ElementName = "stl:query";

        [StlAttribute(Title = "类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "字段")]
        private const string Column = nameof(Column);

        [StlAttribute(Title = "操作")]
        private const string Op = nameof(Op);

        [StlAttribute(Title = "值")]
        private const string Value = nameof(Value);

        [StlAttribute(Title = "数据类型")]
        private const string DataType = nameof(DataType);

        private static QueryInfo Parse(string stlElement)
        {
            var (innerHtml, attributes) = ParseUtils.GetInnerHtmlAndAttributes(stlElement);

            var query = new QueryInfo
            {
                Type = nameof(Query.Where),
                Column = string.Empty,
                Op = string.Empty,
                Value = string.Empty,
                DataType = Datory.DataType.VarChar
            };

            foreach (var name in attributes.AllKeys)
            {
                var attributeValue = attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    query.Type = attributeValue;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Column))
                {
                    query.Column = attributeValue;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Op))
                {
                    query.Op = attributeValue;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Value))
                {
                    query.Value = attributeValue;
                }
                else if (StringUtils.EqualsIgnoreCase(name, DataType))
                {
                    if (StringUtils.EqualsIgnoreCase(attributeValue, "int"))
                    {
                        query.DataType = Datory.DataType.Integer;
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeValue, "string"))
                    {
                        query.DataType = Datory.DataType.VarChar;
                    }
                    else if (StringUtils.EqualsIgnoreCase(attributeValue, "bool"))
                    {
                        query.DataType = Datory.DataType.Boolean;
                    }
                    else
                    {
                        query.DataType = TranslateUtils.ToEnum(attributeValue, Datory.DataType.VarChar);
                    }
                }
            }

            if (!string.IsNullOrEmpty(innerHtml))
            {
                var stlElementList = ParseUtils.GetStlElements(innerHtml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (ParseUtils.IsSpecifiedStlElement(theStlElement, ElementName))
                        {
                            if (query.Queries == null)
                            {
                                query.Queries = new List<QueryInfo>();
                            }
                            query.Queries.Add(Parse(theStlElement));
                        }
                    }
                }
            }

            return query;
		}

        private static object GetValue(DataType dataType, string value)
        {
            switch (dataType)
            {
                case Datory.DataType.VarChar:
                case Datory.DataType.Text:
                    return value;
                case Datory.DataType.Integer:
                    return TranslateUtils.ToIntWithNegative(value);
                case Datory.DataType.Boolean:
                    return TranslateUtils.ToBool(value);
                case Datory.DataType.DateTime:
                    return TranslateUtils.ToDateTime(value);
                case Datory.DataType.Decimal:
                    return TranslateUtils.ToDecimal(value);
                default:
                    return null;
            }
        }

        public static Query AddQuery(this Query query, string stlElement)
        {
            var queryInfo = Parse(stlElement);
            return query.AddQuery(queryInfo);
        }

        private static Query AddQuery(this Query query, QueryInfo queryInfo)
        {
            if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.Where)))
            {
                Where(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNot)))
            {
                WhereNot(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhere)))
            {
                OrWhere(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNot)))
            {
                OrWhereNot(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereColumns)))
            {
                WhereColumns(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereColumns)))
            {
                OrWhereColumns(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNull)))
            {
                WhereNull(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotNull)))
            {
                WhereNotNull(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNull)))
            {
                OrWhereNull(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotNull)))
            {
                OrWhereNotNull(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereTrue)))
            {
                WhereTrue(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereTrue)))
            {
                OrWhereTrue(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereFalse)))
            {
                WhereFalse(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereFalse)))
            {
                OrWhereFalse(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereLike)))
            {
                WhereLike(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotLike)))
            {
                WhereNotLike(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereLike)))
            {
                OrWhereLike(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotLike)))
            {
                OrWhereNotLike(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereStarts)))
            {
                WhereStarts(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotStarts)))
            {
                WhereNotStarts(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereStarts)))
            {
                OrWhereStarts(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotStarts)))
            {
                OrWhereNotStarts(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereEnds)))
            {
                WhereEnds(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotEnds)))
            {
                WhereNotEnds(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereEnds)))
            {
                OrWhereEnds(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotEnds)))
            {
                OrWhereNotEnds(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereContains)))
            {
                WhereContains(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotContains)))
            {
                WhereNotContains(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereContains)))
            {
                OrWhereContains(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotContains)))
            {
                OrWhereNotContains(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereBetween)))
            {
                WhereBetween(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereBetween)))
            {
                OrWhereBetween(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotBetween)))
            {
                WhereNotBetween(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotBetween)))
            {
                OrWhereNotBetween(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereIn)))
            {
                WhereIn(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereIn)))
            {
                OrWhereIn(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotIn)))
            {
                WhereNotIn(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotIn)))
            {
                OrWhereNotIn(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereDate)))
            {
                WhereDate(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotDate)))
            {
                WhereNotDate(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereDate)))
            {
                OrWhereDate(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotDate)))
            {
                OrWhereNotDate(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereTime)))
            {
                WhereTime(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.WhereNotTime)))
            {
                WhereNotTime(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereTime)))
            {
                OrWhereTime(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrWhereNotTime)))
            {
                OrWhereNotTime(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.Select)))
            {
                Select(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.From)))
            {
                From(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.Limit)))
            {
                Limit(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.Offset)))
            {
                Offset(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.Take)))
            {
                Take(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.Skip)))
            {
                Skip(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.ForPage)))
            {
                ForPage(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.Distinct)))
            {
                Distinct(query);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrderBy)))
            {
                OrderBy(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrderByDesc)))
            {
                OrderByDesc(query, queryInfo);
            }
            else if (StringUtils.EqualsIgnoreCase(queryInfo.Type, nameof(Query.OrderByRandom)))
            {
                OrderByRandom(query);
            }

            return query;
        }
    }
}
