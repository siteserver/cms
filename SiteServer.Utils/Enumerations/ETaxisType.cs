using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Plugin;

namespace SiteServer.Utils.Enumerations
{
	public enum ETaxisType
	{
		OrderById,				    //内容ID（升序）
		OrderByIdDesc,			    //内容ID（降序）
		OrderByChannelId,				//栏目ID（升序）
		OrderByChannelIdDesc,			//栏目ID（降序）
		OrderByAddDate,			    //添加时间（升序）
		OrderByAddDateDesc,		    //添加时间（降序）
		OrderByLastEditDate,	    //更新时间（升序）
		OrderByLastEditDateDesc,    //更新时间（降序）
		OrderByTaxis,			    //自定义排序（反方向）
		OrderByTaxisDesc,		    //自定义排序
        OrderByHits,                //按点击量排序
        OrderByHitsByDay,           //按日点击量排序
        OrderByHitsByWeek,          //按周点击量排序
        OrderByHitsByMonth,         //按月点击量排序
        OrderByRandom               //随机排序
	}

	public class ETaxisTypeUtils
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

            var retval = string.Empty;
            if (taxisType == ETaxisType.OrderById)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.AddDate)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.AddDate)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.AddDate)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.AddDate)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.Taxis)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                retval = $"ORDER BY {nameof(IChannelInfo.Taxis)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                if (orderedContentIdList != null && orderedContentIdList.Count > 0)
                {
                    orderedContentIdList.Reverse();
                    retval =
                        $"ORDER BY CHARINDEX(CONVERT(VARCHAR, {nameof(IChannelInfo.Id)}), '{TranslateUtils.ObjectCollectionToString(orderedContentIdList)}') DESC, {nameof(IChannelInfo.Taxis)} ASC";
                }
                else
                {
                    retval = $"ORDER BY {nameof(IChannelInfo.Taxis)} ASC";
                }
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                retval = SqlUtils.GetOrderByRandom();
            }

            return retval;
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

            var retval = string.Empty;

            if (taxisType == ETaxisType.OrderById)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.ChannelId)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.ChannelId)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.AddDate)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.AddDate)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.LastEditDate)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.LastEditDate)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.Taxis)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                retval = $"ORDER BY {nameof(IContentInfo.IsTop)} DESC, {nameof(IContentInfo.Taxis)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                retval = $"ORDER BY {nameof(IContentInfo.Hits)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByDay)
            {
                retval = $"ORDER BY {nameof(IContentInfo.HitsByDay)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByWeek)
            {
                retval = $"ORDER BY {nameof(IContentInfo.HitsByWeek)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByMonth)
            {
                retval = $"ORDER BY {nameof(IContentInfo.HitsByMonth)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                retval = SqlUtils.GetOrderByRandom();
            }

            return retval;
        }

        public static string GetContentOrderAttributeName(ETaxisType taxisType)
        {
            var retval = nameof(IContentInfo.Taxis);

            switch (taxisType)
            {
                case ETaxisType.OrderById:
                case ETaxisType.OrderByIdDesc:
                    retval = nameof(IContentInfo.Id);
                    break;
                case ETaxisType.OrderByChannelId:
                case ETaxisType.OrderByChannelIdDesc:
                    retval = nameof(IContentInfo.ChannelId);
                    break;
                case ETaxisType.OrderByAddDate:
                case ETaxisType.OrderByAddDateDesc:
                    retval = nameof(IContentInfo.AddDate);
                    break;
                case ETaxisType.OrderByLastEditDate:
                case ETaxisType.OrderByLastEditDateDesc:
                    retval = nameof(IContentInfo.LastEditDate);
                    break;
                case ETaxisType.OrderByHits:
                    retval = nameof(IContentInfo.Hits);
                    break;
                case ETaxisType.OrderByHitsByDay:
                    retval = nameof(IContentInfo.HitsByDay);
                    break;
                case ETaxisType.OrderByHitsByWeek:
                    retval = nameof(IContentInfo.HitsByWeek);
                    break;
                case ETaxisType.OrderByHitsByMonth:
                    retval = nameof(IContentInfo.HitsByMonth);
                    break;
            }

            return retval;
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
			var retval = ETaxisType.OrderByTaxisDesc;

			if (Equals(ETaxisType.OrderById, typeStr))
			{
				retval = ETaxisType.OrderById;
			}
			else if (Equals(ETaxisType.OrderByIdDesc, typeStr))
			{
				retval = ETaxisType.OrderByIdDesc;
			}
			else if (Equals(ETaxisType.OrderByChannelId, typeStr))
			{
				retval = ETaxisType.OrderByChannelId;
			}
			else if (Equals(ETaxisType.OrderByChannelIdDesc, typeStr))
			{
				retval = ETaxisType.OrderByChannelIdDesc;
			}
			else if (Equals(ETaxisType.OrderByAddDate, typeStr))
			{
				retval = ETaxisType.OrderByAddDate;
			}
			else if (Equals(ETaxisType.OrderByAddDateDesc, typeStr))
			{
				retval = ETaxisType.OrderByAddDateDesc;
			}
			else if (Equals(ETaxisType.OrderByLastEditDate, typeStr))
			{
				retval = ETaxisType.OrderByLastEditDate;
			}
			else if (Equals(ETaxisType.OrderByLastEditDateDesc, typeStr))
			{
				retval = ETaxisType.OrderByLastEditDateDesc;
			}
			else if (Equals(ETaxisType.OrderByTaxis, typeStr))
			{
				retval = ETaxisType.OrderByTaxis;
			}
			else if (Equals(ETaxisType.OrderByTaxisDesc, typeStr))
			{
				retval = ETaxisType.OrderByTaxisDesc;
            }
            else if (Equals(ETaxisType.OrderByHits, typeStr))
            {
                retval = ETaxisType.OrderByHits;
            }
            else if (Equals(ETaxisType.OrderByHitsByDay, typeStr))
            {
                retval = ETaxisType.OrderByHitsByDay;
            }
            else if (Equals(ETaxisType.OrderByHitsByWeek, typeStr))
            {
                retval = ETaxisType.OrderByHitsByWeek;
            }
            else if (Equals(ETaxisType.OrderByHitsByMonth, typeStr))
            {
                retval = ETaxisType.OrderByHitsByMonth;
            }

			return retval;
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

		public static ListItem GetListItem(ETaxisType type, bool selected)
		{
			var item = new ListItem(GetText(type), GetValue(type));
			if (selected)
			{
				item.Selected = true;
			}
			return item;
		}

		public static void AddListItems(ListControl listControl)
		{
		    if (listControl == null) return;

		    listControl.Items.Add(GetListItem(ETaxisType.OrderById, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByIdDesc, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByChannelId, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByChannelIdDesc, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDate, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDateDesc, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDate, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDateDesc, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxis, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxisDesc, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByHits, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByHitsByDay, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByHitsByWeek, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByHitsByMonth, false));
		}

        public static void AddListItemsForChannelEdit(ListControl listControl)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(ETaxisType.OrderById, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByIdDesc, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDate, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDateDesc, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDate, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByLastEditDateDesc, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxis, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxisDesc, false));
        }

    }
}
