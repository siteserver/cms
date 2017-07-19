using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core.Data;

namespace BaiRong.Core.Model.Enumerations
{
	public enum ETaxisType
	{
		OrderById,				    //内容ID（升序）
		OrderByIdDesc,			    //内容ID（降序）
		OrderByNodeId,				//栏目ID（升序）
		OrderByNodeIdDesc,			//栏目ID（降序）
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
        OrderByStars,               //按评分排序
        OrderByDigg,                //按Digg数量排序
        OrderByComments,            //按评论排序
        OrderByRandom               //随机排序
	}


	public class ETaxisTypeUtils
	{
		public static string GetValue(ETaxisType type)
		{
		    if (type == ETaxisType.OrderById)
			{
				return "OrderByID";
			}
		    if (type == ETaxisType.OrderByIdDesc)
		    {
		        return "OrderByIDDesc";
		    }
		    if (type == ETaxisType.OrderByNodeId)
		    {
		        return "OrderByNodeId";
		    }
		    if (type == ETaxisType.OrderByNodeIdDesc)
		    {
		        return "OrderByNodeIdDesc";
		    }
		    if (type == ETaxisType.OrderByAddDate)
		    {
		        return "OrderByAddDate";
		    }
		    if (type == ETaxisType.OrderByAddDateDesc)
		    {
		        return "OrderByAddDateDesc";
		    }
		    if (type == ETaxisType.OrderByLastEditDate)
		    {
		        return "OrderByLastEditDate";
		    }
		    if (type == ETaxisType.OrderByLastEditDateDesc)
		    {
		        return "OrderByLastEditDateDesc";
		    }
		    if (type == ETaxisType.OrderByTaxis)
		    {
		        return "OrderByTaxis";
		    }
		    if (type == ETaxisType.OrderByTaxisDesc)
		    {
		        return "OrderByTaxisDesc";
		    }
		    if (type == ETaxisType.OrderByHits)
		    {
		        return "OrderByHits";
		    }
		    if (type == ETaxisType.OrderByHitsByDay)
		    {
		        return "OrderByHitsByDay";
		    }
		    if (type == ETaxisType.OrderByHitsByWeek)
		    {
		        return "OrderByHitsByWeek";
		    }
		    if (type == ETaxisType.OrderByHitsByMonth)
		    {
		        return "OrderByHitsByMonth";
		    }
		    if (type == ETaxisType.OrderByStars)
		    {
		        return "OrderByStars";
		    }
		    if (type == ETaxisType.OrderByDigg)
		    {
		        return "OrderByDigg";
		    }
		    if (type == ETaxisType.OrderByComments)
		    {
		        return "OrderByComments";
		    }
		    if (type == ETaxisType.OrderByRandom)
		    {
		        return "OrderByRandom";
		    }
		    throw new Exception();
		}

        public static string GetContentOrderByString(ETaxisType taxisType)
        {
            return GetOrderByString(ETableStyle.BackgroundContent, taxisType);
        }

        public static string GetInputContentOrderByString(ETaxisType taxisType)
        {
            return GetOrderByString(ETableStyle.InputContent, taxisType);
        }

        public static string GetChannelOrderByString(ETaxisType taxisType)
        {
            return GetOrderByString(ETableStyle.Channel, taxisType);
        }

        public static string GetOrderByString(ETableStyle tableStyle, ETaxisType taxisType)
        {
            return GetOrderByString(tableStyle, taxisType, string.Empty, null);
        }

        public static string GetOrderByString(ETableStyle tableStyle, ETaxisType taxisType, string orderByString, List<int> orderedContentIdList)
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
            if (tableStyle == ETableStyle.Channel)
            {
                if (taxisType == ETaxisType.OrderById)
                {
                    retval = "ORDER BY NodeId ASC";
                }
                else if (taxisType == ETaxisType.OrderByIdDesc)
                {
                    retval = "ORDER BY NodeId DESC";
                }
                else if (taxisType == ETaxisType.OrderByNodeId)
                {
                    retval = "ORDER BY NodeId ASC";
                }
                else if (taxisType == ETaxisType.OrderByNodeIdDesc)
                {
                    retval = "ORDER BY NodeId DESC";
                }
                else if (taxisType == ETaxisType.OrderByAddDate)
                {
                    retval = "ORDER BY AddDate ASC";
                }
                else if (taxisType == ETaxisType.OrderByAddDateDesc)
                {
                    retval = "ORDER BY AddDate DESC";
                }
                else if (taxisType == ETaxisType.OrderByLastEditDate)
                {
                    retval = "ORDER BY AddDate ASC";
                }
                else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
                {
                    retval = "ORDER BY AddDate DESC";
                }
                else if (taxisType == ETaxisType.OrderByTaxis)
                {
                    retval = "ORDER BY Taxis ASC";
                }
                else if (taxisType == ETaxisType.OrderByTaxisDesc)
                {
                    retval = "ORDER BY Taxis DESC";
                }
                else if (taxisType == ETaxisType.OrderByHits)
                {
                    if (orderedContentIdList != null && orderedContentIdList.Count > 0)
                    {
                        orderedContentIdList.Reverse();
                        retval =
                            $"ORDER BY CHARINDEX(CONVERT(VARCHAR,NodeId), '{TranslateUtils.ObjectCollectionToString(orderedContentIdList)}') DESC, Taxis ASC";
                    }
                    else
                    {
                        retval = "ORDER BY Taxis ASC";
                    }
                }
                else if (taxisType == ETaxisType.OrderByRandom)
                {
                    retval = SqlUtils.GetOrderByRandom();
                }
            }
            else if (ETableStyleUtils.IsContent(tableStyle))
            {
                if (taxisType == ETaxisType.OrderById)
                {
                    retval = "ORDER BY IsTop DESC, ID ASC";
                }
                else if (taxisType == ETaxisType.OrderByIdDesc)
                {
                    retval = "ORDER BY IsTop DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByNodeId)
                {
                    retval = "ORDER BY IsTop DESC, NodeId ASC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByNodeIdDesc)
                {
                    retval = "ORDER BY IsTop DESC, NodeId DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByAddDate)
                {
                    retval = "ORDER BY IsTop DESC, AddDate ASC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByAddDateDesc)
                {
                    retval = "ORDER BY IsTop DESC, AddDate DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByLastEditDate)
                {
                    retval = "ORDER BY IsTop DESC, LastEditDate ASC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByLastEditDateDesc)
                {
                    retval = "ORDER BY IsTop DESC, LastEditDate DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByTaxis)
                {
                    retval = "ORDER BY IsTop DESC, Taxis ASC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByTaxisDesc)
                {
                    retval = "ORDER BY IsTop DESC, Taxis DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByHits)
                {
                    retval = "ORDER BY Hits DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByHitsByDay)
                {
                    retval = "ORDER BY HitsByDay DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByHitsByWeek)
                {
                    retval = "ORDER BY HitsByWeek DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByHitsByMonth)
                {
                    retval = "ORDER BY HitsByMonth DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByStars || taxisType == ETaxisType.OrderByDigg || taxisType == ETaxisType.OrderByComments)
                {
                    if (orderedContentIdList != null && orderedContentIdList.Count > 0)
                    {
                        orderedContentIdList.Reverse();
                        retval =
                            $"ORDER BY CHARINDEX(CONVERT(VARCHAR,ID), '{TranslateUtils.ObjectCollectionToString(orderedContentIdList)}') DESC, IsTop DESC, Taxis DESC, ID DESC";
                    }
                    else
                    {
                        retval = "ORDER BY IsTop DESC, Taxis DESC, ID DESC";
                    }
                }
                else if (taxisType == ETaxisType.OrderByRandom)
                {
                    retval = SqlUtils.GetOrderByRandom();
                }
            }
            else if (tableStyle == ETableStyle.InputContent)
            {
                if (taxisType == ETaxisType.OrderById)
                {
                    retval = "ORDER BY ID ASC";
                }
                else if (taxisType == ETaxisType.OrderByIdDesc)
                {
                    retval = "ORDER BY ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByAddDate)
                {
                    retval = "ORDER BY AddDate ASC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByAddDateDesc)
                {
                    retval = "ORDER BY AddDate DESC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByTaxis)
                {
                    retval = "ORDER BY Taxis ASC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByTaxisDesc)
                {
                    retval = "ORDER BY Taxis DESC";
                }
                else if (taxisType == ETaxisType.OrderByRandom)
                {
                    retval = SqlUtils.GetOrderByRandom();
                }
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
		    if (type == ETaxisType.OrderByNodeId)
		    {
		        return "栏目ID（升序）";
		    }
		    if (type == ETaxisType.OrderByNodeIdDesc)
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
			else if (Equals(ETaxisType.OrderByNodeId, typeStr))
			{
				retval = ETaxisType.OrderByNodeId;
			}
			else if (Equals(ETaxisType.OrderByNodeIdDesc, typeStr))
			{
				retval = ETaxisType.OrderByNodeIdDesc;
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
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByNodeId, false));
		    listControl.Items.Add(GetListItem(ETaxisType.OrderByNodeIdDesc, false));
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
