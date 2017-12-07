using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	public enum ECrossSiteTransType
	{
        None,
        SelfSite,
        SpecifiedSite,
        ParentSite,
        AllParentSite,
		AllSite
	}

    public class ECrossSiteTransTypeUtils
	{
		public static string GetValue(ECrossSiteTransType type)
		{
		    if (type == ECrossSiteTransType.None)
            {
                return "None";
            }
		    if (type == ECrossSiteTransType.SelfSite)
		    {
		        return "SelfSite";
		    }
		    if (type == ECrossSiteTransType.SpecifiedSite)
		    {
		        return "SpecifiedSite";
		    }
		    if (type == ECrossSiteTransType.ParentSite)
		    {
		        return "ParentSite";
		    }
		    if (type == ECrossSiteTransType.AllParentSite)
		    {
		        return "AllParentSite";
		    }
		    if (type == ECrossSiteTransType.AllSite)
		    {
		        return "AllSite";
		    }
		    throw new Exception();
		}

		public static string GetText(ECrossSiteTransType type)
		{
		    if (type == ECrossSiteTransType.None)
            {
                return "不转发";
            }
		    if (type == ECrossSiteTransType.SelfSite)
		    {
		        return "可向本站转发";
		    }
		    if (type == ECrossSiteTransType.SpecifiedSite)
		    {
		        return "可向指定站点转发";
		    }
		    if (type == ECrossSiteTransType.ParentSite)
		    {
		        return "可向上一级站点转发";
		    }
		    if (type == ECrossSiteTransType.AllParentSite)
		    {
		        return "可向所有上级站点转发";
		    }
		    if (type == ECrossSiteTransType.AllSite)
		    {
		        return "可向所有站点转发";
		    }
		    throw new Exception();
		}

		public static ECrossSiteTransType GetEnumType(string typeStr)
		{
            var retval = ECrossSiteTransType.AllSite;

            if (Equals(ECrossSiteTransType.None, typeStr))
            {
                retval = ECrossSiteTransType.None;
            }
            else if (Equals(ECrossSiteTransType.SelfSite, typeStr))
            {
                retval = ECrossSiteTransType.SelfSite;
            }
            else if (Equals(ECrossSiteTransType.SpecifiedSite, typeStr))
            {
                retval = ECrossSiteTransType.SpecifiedSite;
            }
            else if (Equals(ECrossSiteTransType.ParentSite, typeStr))
            {
                retval = ECrossSiteTransType.ParentSite;
            }
            else if (Equals(ECrossSiteTransType.AllParentSite, typeStr))
			{
                retval = ECrossSiteTransType.AllParentSite;
            }
            else if (Equals(ECrossSiteTransType.AllSite, typeStr))
            {
                retval = ECrossSiteTransType.AllSite;
            }

			return retval;
		}

		public static bool Equals(ECrossSiteTransType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ECrossSiteTransType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(ECrossSiteTransType type, bool selected)
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

            listControl.Items.Add(GetListItem(ECrossSiteTransType.None, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SelfSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.ParentSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.AllParentSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.AllSite, false));
        }

        public static void AddAllListItems(ListControl listControl, bool isParentSite)
        {
            if (listControl == null) return;

            listControl.Items.Add(GetListItem(ECrossSiteTransType.None, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SelfSite, false));
            listControl.Items.Add(GetListItem(ECrossSiteTransType.SpecifiedSite, false));
            if (isParentSite)
            {
                listControl.Items.Add(GetListItem(ECrossSiteTransType.ParentSite, false));
                listControl.Items.Add(GetListItem(ECrossSiteTransType.AllParentSite, false));
            }
            listControl.Items.Add(GetListItem(ECrossSiteTransType.AllSite, false));
        }
	}
}
