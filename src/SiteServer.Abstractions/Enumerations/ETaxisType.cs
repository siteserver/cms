using System;
using System.Collections.Generic;
using Datory.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SiteServer.Abstractions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ETaxisType
	{
        [DataEnum(DisplayName = "内容ID（升序）")] OrderById,
        [DataEnum(DisplayName = "内容ID（降序）")] OrderByIdDesc,
        [DataEnum(DisplayName = "栏目ID（升序）")] OrderByChannelId,
        [DataEnum(DisplayName = "栏目ID（降序）")] OrderByChannelIdDesc,
        [DataEnum(DisplayName = "添加时间（升序）")] OrderByAddDate,
        [DataEnum(DisplayName = "添加时间（降序）")] OrderByAddDateDesc,
        [DataEnum(DisplayName = "更新时间（升序）")] OrderByLastEditDate,
        [DataEnum(DisplayName = "更新时间（降序）")] OrderByLastEditDateDesc,
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
		public static string GetValue(ETaxisType type)
		{
		    if (type == ETaxisType.OrderById)
			{
				return nameof(ETaxisType.OrderById);
			}
		    if (type == ETaxisType.OrderByIdDesc)
		    {
		        return nameof(ETaxisType.OrderByIdDesc);
		    }
		    if (type == ETaxisType.OrderByChannelId)
		    {
		        return nameof(ETaxisType.OrderByChannelId);
            }
		    if (type == ETaxisType.OrderByChannelIdDesc)
		    {
		        return nameof(ETaxisType.OrderByChannelIdDesc);
            }
		    if (type == ETaxisType.OrderByAddDate)
		    {
		        return nameof(ETaxisType.OrderByAddDate);
            }
		    if (type == ETaxisType.OrderByAddDateDesc)
		    {
		        return nameof(ETaxisType.OrderByAddDateDesc);
            }
		    if (type == ETaxisType.OrderByLastEditDate)
		    {
		        return nameof(ETaxisType.OrderByLastEditDate);
            }
		    if (type == ETaxisType.OrderByLastEditDateDesc)
		    {
		        return nameof(ETaxisType.OrderByLastEditDateDesc);
            }
		    if (type == ETaxisType.OrderByTaxis)
		    {
		        return nameof(ETaxisType.OrderByTaxis);
            }
		    if (type == ETaxisType.OrderByTaxisDesc)
		    {
		        return nameof(ETaxisType.OrderByTaxisDesc);
            }
		    if (type == ETaxisType.OrderByHits)
		    {
		        return nameof(ETaxisType.OrderByHits);
            }
		    if (type == ETaxisType.OrderByHitsByDay)
		    {
		        return nameof(ETaxisType.OrderByHitsByDay);
            }
		    if (type == ETaxisType.OrderByHitsByWeek)
		    {
		        return nameof(ETaxisType.OrderByHitsByWeek);
            }
		    if (type == ETaxisType.OrderByHitsByMonth)
		    {
		        return nameof(ETaxisType.OrderByHitsByMonth);
            }
		    if (type == ETaxisType.OrderByRandom)
		    {
		        return nameof(ETaxisType.OrderByRandom);
            }

		    throw new Exception();
		}

        public static string GetChannelOrderByString(ETaxisType taxisType)
        {
            return GetChannelOrderByString(taxisType, string.Empty, null);
        }

        public static string GetChannelOrderByString(ETaxisType taxisType, string orderByString, List<int> orderedContentIdList)
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
            if (taxisType == ETaxisType.OrderById)
            {
                retVal = $"ORDER BY {nameof(Channel.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                retVal = $"ORDER BY {nameof(Channel.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                retVal = $"ORDER BY {nameof(Channel.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                retVal = $"ORDER BY {nameof(Channel.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                retVal = $"ORDER BY {nameof(Channel.AddDate)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                retVal = $"ORDER BY {nameof(Channel.AddDate)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                retVal = $"ORDER BY {nameof(Channel.AddDate)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                retVal = $"ORDER BY {nameof(Channel.AddDate)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                retVal = $"ORDER BY {nameof(Channel.Taxis)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                retVal = $"ORDER BY {nameof(Channel.Taxis)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                if (orderedContentIdList != null && orderedContentIdList.Count > 0)
                {
                    orderedContentIdList.Reverse();
                    retVal =
                        $"ORDER BY CHARINDEX(CONVERT(VARCHAR, {nameof(Channel.Id)}), '{TranslateUtils.ObjectCollectionToString(orderedContentIdList)}') DESC, {nameof(Channel.Taxis)} ASC";
                }
                else
                {
                    retVal = $"ORDER BY {nameof(Channel.Taxis)} ASC";
                }
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                //retVal = SqlUtils.GetOrderByRandom();
            }

            return retVal;
        }

        public static string GetContentOrderByString(ETaxisType taxisType)
        {
            return GetContentOrderByString(taxisType, string.Empty);
        }

        public static string GetContentOrderByString(ETaxisType taxisType, string orderByString)
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

            if (taxisType == ETaxisType.OrderById)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.ChannelId)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.ChannelId)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.AddDate)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.AddDate)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.LastEditDate)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.LastEditDate)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.Taxis)} ASC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(Content.Taxis)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                retVal = $"ORDER BY {nameof(Content.Hits)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByDay)
            {
                retVal = $"ORDER BY {nameof(Content.HitsByDay)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByWeek)
            {
                retVal = $"ORDER BY {nameof(Content.HitsByWeek)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByMonth)
            {
                retVal = $"ORDER BY {nameof(Content.HitsByMonth)} DESC, {nameof(Content.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                //retVal = SqlUtils.GetOrderByRandom();
            }

            return retVal;
        }

        public static string GetContentOrderAttributeName(ETaxisType taxisType)
        {
            var retVal = nameof(Content.Taxis);

            switch (taxisType)
            {
                case ETaxisType.OrderById:
                case ETaxisType.OrderByIdDesc:
                    retVal = nameof(Content.Id);
                    break;
                case ETaxisType.OrderByChannelId:
                case ETaxisType.OrderByChannelIdDesc:
                    retVal = nameof(Content.ChannelId);
                    break;
                case ETaxisType.OrderByAddDate:
                case ETaxisType.OrderByAddDateDesc:
                    retVal = nameof(Content.AddDate);
                    break;
                case ETaxisType.OrderByLastEditDate:
                case ETaxisType.OrderByLastEditDateDesc:
                    retVal = nameof(Content.LastEditDate);
                    break;
                case ETaxisType.OrderByHits:
                    retVal = nameof(Content.Hits);
                    break;
                case ETaxisType.OrderByHitsByDay:
                    retVal = nameof(Content.HitsByDay);
                    break;
                case ETaxisType.OrderByHitsByWeek:
                    retVal = nameof(Content.HitsByWeek);
                    break;
                case ETaxisType.OrderByHitsByMonth:
                    retVal = nameof(Content.HitsByMonth);
                    break;
            }

            return retVal;
        }

        public static string GetText(ETaxisType type)
		{
		    if (type == ETaxisType.OrderById)
			{
				return "内容ID（升序）";
			}
		    if (type == ETaxisType.OrderByIdDesc)
		    {
		        return "内容ID（降序）";
		    }
		    if (type == ETaxisType.OrderByChannelId)
		    {
		        return "栏目ID（升序）";
		    }
		    if (type == ETaxisType.OrderByChannelIdDesc)
		    {
		        return "栏目ID（降序）";
		    }
		    if (type == ETaxisType.OrderByAddDate)
		    {
		        return "添加时间（升序）";
		    }
		    if (type == ETaxisType.OrderByAddDateDesc)
		    {
		        return "添加时间（降序）";
		    }
		    if (type == ETaxisType.OrderByLastEditDate)
		    {
		        return "更新时间（升序）";
		    }
		    if (type == ETaxisType.OrderByLastEditDateDesc)
		    {
		        return "更新时间（降序）";
		    }
		    if (type == ETaxisType.OrderByTaxis)
		    {
		        return "自定义排序（升序）";
		    }
		    if (type == ETaxisType.OrderByTaxisDesc)
		    {
		        return "自定义排序（降序）";
		    }
		    if (type == ETaxisType.OrderByHits)
		    {
		        return "点击量排序";
		    }
		    if (type == ETaxisType.OrderByHitsByDay)
		    {
		        return "日点击量排序";
		    }
		    if (type == ETaxisType.OrderByHitsByWeek)
		    {
		        return "周点击量排序";
		    }
		    if (type == ETaxisType.OrderByHitsByMonth)
		    {
		        return "月点击量排序";
		    }
		    throw new Exception();
		}

		public static ETaxisType GetEnumType(string typeStr)
		{
			var retVal = ETaxisType.OrderByTaxisDesc;

			if (Equals(ETaxisType.OrderById, typeStr))
			{
				retVal = ETaxisType.OrderById;
			}
			else if (Equals(ETaxisType.OrderByIdDesc, typeStr))
			{
				retVal = ETaxisType.OrderByIdDesc;
			}
			else if (Equals(ETaxisType.OrderByChannelId, typeStr))
			{
				retVal = ETaxisType.OrderByChannelId;
			}
			else if (Equals(ETaxisType.OrderByChannelIdDesc, typeStr))
			{
				retVal = ETaxisType.OrderByChannelIdDesc;
			}
			else if (Equals(ETaxisType.OrderByAddDate, typeStr))
			{
				retVal = ETaxisType.OrderByAddDate;
			}
			else if (Equals(ETaxisType.OrderByAddDateDesc, typeStr))
			{
				retVal = ETaxisType.OrderByAddDateDesc;
			}
			else if (Equals(ETaxisType.OrderByLastEditDate, typeStr))
			{
				retVal = ETaxisType.OrderByLastEditDate;
			}
			else if (Equals(ETaxisType.OrderByLastEditDateDesc, typeStr))
			{
				retVal = ETaxisType.OrderByLastEditDateDesc;
			}
			else if (Equals(ETaxisType.OrderByTaxis, typeStr))
			{
				retVal = ETaxisType.OrderByTaxis;
			}
			else if (Equals(ETaxisType.OrderByTaxisDesc, typeStr))
			{
				retVal = ETaxisType.OrderByTaxisDesc;
            }
            else if (Equals(ETaxisType.OrderByHits, typeStr))
            {
                retVal = ETaxisType.OrderByHits;
            }
            else if (Equals(ETaxisType.OrderByHitsByDay, typeStr))
            {
                retVal = ETaxisType.OrderByHitsByDay;
            }
            else if (Equals(ETaxisType.OrderByHitsByWeek, typeStr))
            {
                retVal = ETaxisType.OrderByHitsByWeek;
            }
            else if (Equals(ETaxisType.OrderByHitsByMonth, typeStr))
            {
                retVal = ETaxisType.OrderByHitsByMonth;
            }

			return retVal;
		}

		public static bool Equals(ETaxisType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ETaxisType type)
        {
            return Equals(type, typeStr);
        }
    }
}
