using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model.Enumerations
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

            var retVal = string.Empty;
            if (taxisType == ETaxisType.OrderById)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.AddDate)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.AddDate)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.AddDate)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.AddDate)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.Taxis)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                retVal = $"ORDER BY {nameof(IChannelInfo.Taxis)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                if (orderedContentIdList != null && orderedContentIdList.Count > 0)
                {
                    orderedContentIdList.Reverse();
                    retVal =
                        $"ORDER BY CHARINDEX(CONVERT(VARCHAR, {nameof(IChannelInfo.Id)}), '{TranslateUtils.ObjectCollectionToString(orderedContentIdList)}') DESC, {nameof(IChannelInfo.Taxis)} ASC";
                }
                else
                {
                    retVal = $"ORDER BY {nameof(IChannelInfo.Taxis)} ASC";
                }
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                retVal = SqlUtils.GetOrderByRandom();
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
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.Id)} ASC";
            }
            else if (taxisType == ETaxisType.OrderByIdDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelId)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.ChannelId)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByChannelIdDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.ChannelId)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDate)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.AddDate)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByAddDateDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.AddDate)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDate)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.LastEditDate)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.LastEditDate)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxis)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.Taxis)} ASC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByTaxisDesc)
            {
                retVal = $"ORDER BY {nameof(ContentAttribute.IsTop)} DESC, {nameof(IContentInfo.Taxis)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHits)
            {
                retVal = $"ORDER BY {nameof(IContentInfo.Hits)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByDay)
            {
                retVal = $"ORDER BY {nameof(IContentInfo.HitsByDay)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByWeek)
            {
                retVal = $"ORDER BY {nameof(IContentInfo.HitsByWeek)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByHitsByMonth)
            {
                retVal = $"ORDER BY {nameof(IContentInfo.HitsByMonth)} DESC, {nameof(IContentInfo.Id)} DESC";
            }
            else if (taxisType == ETaxisType.OrderByRandom)
            {
                retVal = SqlUtils.GetOrderByRandom();
            }

            return retVal;
        }

        public static string GetContentOrderAttributeName(ETaxisType taxisType)
        {
            var retVal = nameof(IContentInfo.Taxis);

            switch (taxisType)
            {
                case ETaxisType.OrderById:
                case ETaxisType.OrderByIdDesc:
                    retVal = nameof(IContentInfo.Id);
                    break;
                case ETaxisType.OrderByChannelId:
                case ETaxisType.OrderByChannelIdDesc:
                    retVal = nameof(IContentInfo.ChannelId);
                    break;
                case ETaxisType.OrderByAddDate:
                case ETaxisType.OrderByAddDateDesc:
                    retVal = nameof(IContentInfo.AddDate);
                    break;
                case ETaxisType.OrderByLastEditDate:
                case ETaxisType.OrderByLastEditDateDesc:
                    retVal = nameof(IContentInfo.LastEditDate);
                    break;
                case ETaxisType.OrderByHits:
                    retVal = nameof(IContentInfo.Hits);
                    break;
                case ETaxisType.OrderByHitsByDay:
                    retVal = nameof(IContentInfo.HitsByDay);
                    break;
                case ETaxisType.OrderByHitsByWeek:
                    retVal = nameof(IContentInfo.HitsByWeek);
                    break;
                case ETaxisType.OrderByHitsByMonth:
                    retVal = nameof(IContentInfo.HitsByMonth);
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
		        return "默认（升序）";
		    }
		    if (type == ETaxisType.OrderByTaxisDesc)
		    {
		        return "默认（降序）";
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

            listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDate, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByAddDateDesc, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxis, false));
            listControl.Items.Add(GetListItem(ETaxisType.OrderByTaxisDesc, false));
        }

    }
}
