using System;
using SqlKata;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Common
{
    public static class TaxisTypeUtils
    {
        public static string GetValue(TaxisType type)
        {
            if (type == TaxisType.OrderById)
            {
                return nameof(TaxisType.OrderById);
            }
            if (type == TaxisType.OrderByIdDesc)
            {
                return nameof(TaxisType.OrderByIdDesc);
            }
            if (type == TaxisType.OrderByChannelId)
            {
                return nameof(TaxisType.OrderByChannelId);
            }
            if (type == TaxisType.OrderByChannelIdDesc)
            {
                return nameof(TaxisType.OrderByChannelIdDesc);
            }
            if (type == TaxisType.OrderByAddDate)
            {
                return nameof(TaxisType.OrderByAddDate);
            }
            if (type == TaxisType.OrderByAddDateDesc)
            {
                return nameof(TaxisType.OrderByAddDateDesc);
            }
            if (type == TaxisType.OrderByLastModifiedDate)
            {
                return nameof(TaxisType.OrderByLastModifiedDate);
            }
            if (type == TaxisType.OrderByLastModifiedDateDesc)
            {
                return nameof(TaxisType.OrderByLastModifiedDateDesc);
            }
            if (type == TaxisType.OrderByTaxis)
            {
                return nameof(TaxisType.OrderByTaxis);
            }
            if (type == TaxisType.OrderByTaxisDesc)
            {
                return nameof(TaxisType.OrderByTaxisDesc);
            }
            if (type == TaxisType.OrderByHits)
            {
                return nameof(TaxisType.OrderByHits);
            }
            if (type == TaxisType.OrderByHitsByDay)
            {
                return nameof(TaxisType.OrderByHitsByDay);
            }
            if (type == TaxisType.OrderByHitsByWeek)
            {
                return nameof(TaxisType.OrderByHitsByWeek);
            }
            if (type == TaxisType.OrderByHitsByMonth)
            {
                return nameof(TaxisType.OrderByHitsByMonth);
            }
            if (type == TaxisType.OrderByRandom)
            {
                return nameof(TaxisType.OrderByRandom);
            }

            throw new Exception();
        }

        public static Query AddChannelOrderBy(this Query query, TaxisType taxisType, string orderByString = null)
        {
            if (!string.IsNullOrEmpty(orderByString))
            {
                if (orderByString.Trim().ToUpper().StartsWith("ORDER BY "))
                {
                    query.OrderByRaw(orderByString);
                }
                else
                {
                    query.OrderByRaw("ORDER BY " + orderByString);
                }
            }
            else if (taxisType == TaxisType.OrderById)
            {
                query.OrderBy(nameof(ChannelInfo.Id));
            }
            else if (taxisType == TaxisType.OrderByIdDesc)
            {
                query.OrderByDesc(nameof(ChannelInfo.Id));
            }
            else if (taxisType == TaxisType.OrderByChannelId)
            {
                query.OrderBy(nameof(ChannelInfo.Id));
            }
            else if (taxisType == TaxisType.OrderByChannelIdDesc)
            {
                query.OrderByDesc(nameof(ChannelInfo.Id));
            }
            else if (taxisType == TaxisType.OrderByAddDate)
            {
                query.OrderBy(nameof(ChannelInfo.CreationDate));
            }
            else if (taxisType == TaxisType.OrderByAddDateDesc)
            {
                query.OrderByDesc(nameof(ChannelInfo.CreationDate));
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                query.OrderBy(nameof(ChannelInfo.LastModifiedDate));
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                query.OrderByDesc(nameof(ChannelInfo.LastModifiedDate));
            }
            else if (taxisType == TaxisType.OrderByTaxis)
            {
                query.OrderBy(nameof(ChannelInfo.Taxis));
            }
            else if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                query.OrderByDesc(nameof(ChannelInfo.Taxis));
            }
            else if (taxisType == TaxisType.OrderByRandom)
            {
                query.OrderByRandom(StringUtils.GetShortGuid());
            }

            return query;
        }

        public static string GetContentOrderAttributeName(TaxisType taxisType)
        {
            var retval = nameof(ContentInfo.Taxis);

            if (taxisType == TaxisType.OrderById || taxisType == TaxisType.OrderByIdDesc)
                retval = nameof(ContentInfo.Id);
            else if (taxisType == TaxisType.OrderByChannelId || taxisType == TaxisType.OrderByChannelIdDesc)
                retval = nameof(ContentInfo.ChannelId);
            else if (taxisType == TaxisType.OrderByAddDate || taxisType == TaxisType.OrderByAddDateDesc)
                retval = nameof(ContentInfo.AddDate);
            else if (taxisType == TaxisType.OrderByLastModifiedDate || taxisType == TaxisType.OrderByLastModifiedDateDesc)
                retval = nameof(ContentInfo.LastModifiedDate);
            else if (taxisType == TaxisType.OrderByHits)
                retval = nameof(ContentInfo.Hits);
            else if (taxisType == TaxisType.OrderByHitsByDay)
                retval = nameof(ContentInfo.HitsByDay);
            else if (taxisType == TaxisType.OrderByHitsByWeek)
                retval = nameof(ContentInfo.HitsByWeek);
            else if (taxisType == TaxisType.OrderByHitsByMonth) retval = nameof(ContentInfo.HitsByMonth);

            return retval;
        }

        public static string GetText(TaxisType type)
        {
            if (type == TaxisType.OrderById)
            {
                return "内容ID（升序）";
            }
            if (type == TaxisType.OrderByIdDesc)
            {
                return "内容ID（降序）";
            }
            if (type == TaxisType.OrderByChannelId)
            {
                return "栏目ID（升序）";
            }
            if (type == TaxisType.OrderByChannelIdDesc)
            {
                return "栏目ID（降序）";
            }
            if (type == TaxisType.OrderByAddDate)
            {
                return "添加时间（升序）";
            }
            if (type == TaxisType.OrderByAddDateDesc)
            {
                return "添加时间（降序）";
            }
            if (type == TaxisType.OrderByLastModifiedDate)
            {
                return "更新时间（升序）";
            }
            if (type == TaxisType.OrderByLastModifiedDateDesc)
            {
                return "更新时间（降序）";
            }
            if (type == TaxisType.OrderByTaxis)
            {
                return "自定义排序（升序）";
            }
            if (type == TaxisType.OrderByTaxisDesc)
            {
                return "自定义排序（降序）";
            }
            if (type == TaxisType.OrderByHits)
            {
                return "点击量排序";
            }
            if (type == TaxisType.OrderByHitsByDay)
            {
                return "日点击量排序";
            }
            if (type == TaxisType.OrderByHitsByWeek)
            {
                return "周点击量排序";
            }
            if (type == TaxisType.OrderByHitsByMonth)
            {
                return "月点击量排序";
            }
            throw new Exception();
        }



        public static bool Equals(TaxisType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, TaxisType type)
        {
            return Equals(type, typeStr);
        }
    }
}
