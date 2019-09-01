using System;
using System.Web.UI.WebControls;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core
{
    public static class TemplateTypeUtils
	{
		public static TemplateType GetEnumType(string typeStr)
		{
			var retVal = TemplateType.IndexPageTemplate;

			if (Equals(TemplateType.ChannelTemplate, typeStr))
			{
				retVal = TemplateType.ChannelTemplate;
			}
			else if (Equals(TemplateType.IndexPageTemplate, typeStr))
			{
				retVal = TemplateType.IndexPageTemplate;
			}
			else if (Equals(TemplateType.ContentTemplate, typeStr))
			{
				retVal = TemplateType.ContentTemplate;
			}
			else if (Equals(TemplateType.FileTemplate, typeStr))
			{
				retVal = TemplateType.FileTemplate;
			}
			return retVal;
		}

		public static bool Equals(TemplateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, TemplateType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(TemplateType type, bool selected)
		{
			var item = new ListItem(GetText(type), type.Value);
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
				listControl.Items.Add(GetListItem(TemplateType.IndexPageTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.ChannelTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.ContentTemplate, false));
				listControl.Items.Add(GetListItem(TemplateType.FileTemplate, false));
			}
		}

        public static string GetText(TemplateType templateType)
        {
            if (templateType == TemplateType.IndexPageTemplate)
            {
                return "首页模板";
            }
            if (templateType == TemplateType.ChannelTemplate)
            {
                return "栏目模板";
            }
            if (templateType == TemplateType.ContentTemplate)
            {
                return "内容模板";
            }
            if (templateType == TemplateType.FileTemplate)
            {
                return "单页模板";
            }

            throw new Exception();
        }
    }
}
