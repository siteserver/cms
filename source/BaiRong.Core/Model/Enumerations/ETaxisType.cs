using System;
using System.Collections;
using System.Web.UI.WebControls;

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
			else if (type == ETaxisType.OrderByIdDesc)
			{
				return "OrderByIDDesc";
			}
			else if (type == ETaxisType.OrderByNodeId)
			{
				return "OrderByNodeID";
			}
			else if (type == ETaxisType.OrderByNodeIdDesc)
			{
				return "OrderByNodeIDDesc";
			}
			else if (type == ETaxisType.OrderByAddDate)
			{
				return "OrderByAddDate";
			}
			else if (type == ETaxisType.OrderByAddDateDesc)
			{
				return "OrderByAddDateDesc";
			}
			else if (type == ETaxisType.OrderByLastEditDate)
			{
				return "OrderByLastEditDate";
			}
			else if (type == ETaxisType.OrderByLastEditDateDesc)
			{
				return "OrderByLastEditDateDesc";
			}
			else if (type == ETaxisType.OrderByTaxis)
			{
				return "OrderByTaxis";
			}
			else if (type == ETaxisType.OrderByTaxisDesc)
			{
				return "OrderByTaxisDesc";
            }
            else if (type == ETaxisType.OrderByHits)
            {
                return "OrderByHits";
            }
            else if (type == ETaxisType.OrderByHitsByDay)
            {
                return "OrderByHitsByDay";
            }
            else if (type == ETaxisType.OrderByHitsByWeek)
            {
                return "OrderByHitsByWeek";
            }
            else if (type == ETaxisType.OrderByHitsByMonth)
            {
                return "OrderByHitsByMonth";
            }
            else if (type == ETaxisType.OrderByStars)
            {
                return "OrderByStars";
            }
            else if (type == ETaxisType.OrderByDigg)
            {
                return "OrderByDigg";
            }
            else if (type == ETaxisType.OrderByComments)
            {
                return "OrderByComments";
            }
            else if (type == ETaxisType.OrderByRandom)
            {
                return "OrderByRandom";
            }
			else
			{
				throw new Exception();
			}
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

        public static string GetOrderByString(ETableStyle tableStyle, ETaxisType taxisType, string orderByString, ArrayList orderedContentIdArrayList)
        {
            if (!string.IsNullOrEmpty(orderByString))
            {
                if (orderByString.Trim().ToUpper().StartsWith("ORDER BY "))
                {
                    return orderByString;
                }
                else
                {
                    return "ORDER BY " + orderByString;
                }
            }

            var retval = string.Empty;
            if (tableStyle == ETableStyle.Channel)
            {
                if (taxisType == ETaxisType.OrderById)
                {
                    retval = "ORDER BY NodeID ASC";
                }
                else if (taxisType == ETaxisType.OrderByIdDesc)
                {
                    retval = "ORDER BY NodeID DESC";
                }
                else if (taxisType == ETaxisType.OrderByNodeId)
                {
                    retval = "ORDER BY NodeID ASC";
                }
                else if (taxisType == ETaxisType.OrderByNodeIdDesc)
                {
                    retval = "ORDER BY NodeID DESC";
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
                    if (orderedContentIdArrayList != null && orderedContentIdArrayList.Count > 0)
                    {
                        orderedContentIdArrayList.Reverse();
                        retval =
                            $"ORDER BY CHARINDEX(CONVERT(VARCHAR,NodeID), '{TranslateUtils.ObjectCollectionToString(orderedContentIdArrayList)}') DESC, Taxis ASC";
                    }
                    else
                    {
                        retval = "ORDER BY Taxis ASC";
                    }
                }
                else if (taxisType == ETaxisType.OrderByRandom)
                {
                    retval = "ORDER BY NEWID() DESC";
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
                    retval = "ORDER BY IsTop DESC, NodeID ASC, ID DESC";
                }
                else if (taxisType == ETaxisType.OrderByNodeIdDesc)
                {
                    retval = "ORDER BY IsTop DESC, NodeID DESC, ID DESC";
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
                    if (orderedContentIdArrayList != null && orderedContentIdArrayList.Count > 0)
                    {
                        orderedContentIdArrayList.Reverse();
                        retval =
                            $"ORDER BY CHARINDEX(CONVERT(VARCHAR,ID), '{TranslateUtils.ObjectCollectionToString(orderedContentIdArrayList)}') DESC, IsTop DESC, Taxis DESC, ID DESC";
                    }
                    else
                    {
                        retval = "ORDER BY IsTop DESC, Taxis DESC, ID DESC";
                    }
                }
                else if (taxisType == ETaxisType.OrderByRandom)
                {
                    retval = "ORDER BY NEWID() DESC";
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
                    retval = "ORDER BY NEWID() DESC";
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
			else if (type == ETaxisType.OrderByIdDesc)
			{
				return "内容ID（降序）";
			}
			else if (type == ETaxisType.OrderByNodeId)
			{
				return "栏目ID（升序）";
			}
			else if (type == ETaxisType.OrderByNodeIdDesc)
			{
				return "栏目ID（降序）";
			}
			else if (type == ETaxisType.OrderByAddDate)
			{
				return "添加时间（升序）";
			}
			else if (type == ETaxisType.OrderByAddDateDesc)
			{
				return "添加时间（降序）";
			}
			else if (type == ETaxisType.OrderByLastEditDate)
			{
				return "更新时间（升序）";
			}
			else if (type == ETaxisType.OrderByLastEditDateDesc)
			{
				return "更新时间（降序）";
			}
			else if (type == ETaxisType.OrderByTaxis)
			{
				return "自定义排序（反方向）";
			}
			else if (type == ETaxisType.OrderByTaxisDesc)
			{
				return "自定义排序";
            }
            else if (type == ETaxisType.OrderByHits)
            {
                return "点击量排序";
            }
            else if (type == ETaxisType.OrderByHitsByDay)
            {
                return "日点击量排序";
            }
            else if (type == ETaxisType.OrderByHitsByWeek)
            {
                return "周点击量排序";
            }
            else if (type == ETaxisType.OrderByHitsByMonth)
            {
                return "月点击量排序";
            }
			else
			{
				throw new Exception();
			}
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
			if (listControl != null)
			{
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
		}

	}
}
