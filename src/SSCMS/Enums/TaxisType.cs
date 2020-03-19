using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SSCMS
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TaxisType
	{
        [DataEnum(DisplayName = "内容ID（升序）")] OrderById,
        [DataEnum(DisplayName = "内容ID（降序）")] OrderByIdDesc,
        [DataEnum(DisplayName = "栏目ID（升序）")] OrderByChannelId,
        [DataEnum(DisplayName = "栏目ID（降序）")] OrderByChannelIdDesc,
        [DataEnum(DisplayName = "添加时间（升序）")] OrderByAddDate,
        [DataEnum(DisplayName = "添加时间（降序）")] OrderByAddDateDesc,
        [DataEnum(DisplayName = "更新时间（升序）")] OrderByLastModifiedDate,
        [DataEnum(DisplayName = "更新时间（降序）")] OrderByLastModifiedDateDesc,
        [DataEnum(DisplayName = "默认排序（升序）")] OrderByTaxis,
        [DataEnum(DisplayName = "默认排序（降序）")] OrderByTaxisDesc,
        [DataEnum(DisplayName = "按点击量排序")] OrderByHits,
        [DataEnum(DisplayName = "按日点击量排序")] OrderByHitsByDay,
        [DataEnum(DisplayName = "按周点击量排序")] OrderByHitsByWeek,
        [DataEnum(DisplayName = "按月点击量排序")] OrderByHitsByMonth,
        [DataEnum(DisplayName = "随机排序")] OrderByRandom
    }

    public static class ETaxisTypeUtils
	{

        

        public static string GetContentOrderByString(TaxisType taxisType)
        {
            return GetContentOrderByString(taxisType, string.Empty);
        }

        public static string GetContentOrderByString(TaxisType taxisType, string orderByString)
        {
            if (!string.IsNullOrEmpty(orderByString))
            {
                if (orderByString.Trim().ToUpper().StartsWith("ORDER BY "))
                {
                    return orderByString;
                }
                return "ORDER BY " + orderByString;
            }

            var retVal = string.Empty;

            if (taxisType == TaxisType.OrderById)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.Id)} ASC";
            }
            else if (taxisType == TaxisType.OrderByIdDesc)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByChannelId)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.ChannelId)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByChannelIdDesc)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.ChannelId)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByAddDate)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.AddDate)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByAddDateDesc)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.AddDate)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.LastModifiedDate)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.LastModifiedDate)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByTaxis)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.Taxis)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                retVal = $"ORDER BY {nameof(Content.Top)} DESC, {nameof(Content.Taxis)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByHits)
            {
                retVal = $"ORDER BY {nameof(Content.Hits)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByHitsByDay)
            {
                retVal = $"ORDER BY {nameof(Content.HitsByDay)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByHitsByWeek)
            {
                retVal = $"ORDER BY {nameof(Content.HitsByWeek)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByHitsByMonth)
            {
                retVal = $"ORDER BY {nameof(Content.HitsByMonth)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == TaxisType.OrderByRandom)
            {
                //retVal = SqlUtils.GetOrderByRandom();
            }

            return retVal;
        }

        public static string GetContentOrderAttributeName(TaxisType taxisType)
        {
            var retVal = nameof(Content.Taxis);

            switch (taxisType)
            {
                case TaxisType.OrderById:
                case TaxisType.OrderByIdDesc:
                    retVal = nameof(Content.Id);
                    break;
                case TaxisType.OrderByChannelId:
                case TaxisType.OrderByChannelIdDesc:
                    retVal = nameof(Content.ChannelId);
                    break;
                case TaxisType.OrderByAddDate:
                case TaxisType.OrderByAddDateDesc:
                    retVal = nameof(Content.AddDate);
                    break;
                case TaxisType.OrderByLastModifiedDate:
                case TaxisType.OrderByLastModifiedDateDesc:
                    retVal = nameof(Content.LastModifiedDate);
                    break;
                case TaxisType.OrderByHits:
                    retVal = nameof(Content.Hits);
                    break;
                case TaxisType.OrderByHitsByDay:
                    retVal = nameof(Content.HitsByDay);
                    break;
                case TaxisType.OrderByHitsByWeek:
                    retVal = nameof(Content.HitsByWeek);
                    break;
                case TaxisType.OrderByHitsByMonth:
                    retVal = nameof(Content.HitsByMonth);
                    break;
            }

            return retVal;
        }

    }
}
