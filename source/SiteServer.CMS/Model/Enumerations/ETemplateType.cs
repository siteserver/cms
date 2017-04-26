using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	public enum ETemplateType
	{
		IndexPageTemplate,		//首页模板
		ChannelTemplate,		//栏目模板
		ContentTemplate,		//内容模板
		FileTemplate			//单页模板
	}

	public class ETemplateTypeUtils
	{
		public static string GetValue(ETemplateType type)
		{
			if (type == ETemplateType.IndexPageTemplate)
			{
				return "IndexPageTemplate";
			}
			else if (type == ETemplateType.ChannelTemplate)
			{
				return "ChannelTemplate";
			}
			else if (type == ETemplateType.ContentTemplate)
			{
				return "ContentTemplate";
			}
			else if (type == ETemplateType.FileTemplate)
			{
				return "FileTemplate";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ETemplateType type)
		{
			if (type == ETemplateType.ChannelTemplate)
			{
				return "栏目模板";
			}
			else if (type == ETemplateType.IndexPageTemplate)
			{
				return "首页模板";
			}
			else if (type == ETemplateType.ContentTemplate)
			{
				return "内容模板";
			}
			else if (type == ETemplateType.FileTemplate)
			{
                return "单页模板";
			}
			else
			{
				throw new Exception();
			}
		}

		public static ETemplateType GetEnumType(string typeStr)
		{
			var retval = ETemplateType.IndexPageTemplate;

			if (Equals(ETemplateType.ChannelTemplate, typeStr))
			{
				retval = ETemplateType.ChannelTemplate;
			}
			else if (Equals(ETemplateType.IndexPageTemplate, typeStr))
			{
				retval = ETemplateType.IndexPageTemplate;
			}
			else if (Equals(ETemplateType.ContentTemplate, typeStr))
			{
				retval = ETemplateType.ContentTemplate;
			}
			else if (Equals(ETemplateType.FileTemplate, typeStr))
			{
				retval = ETemplateType.FileTemplate;
			}
			return retval;
		}

		public static bool Equals(ETemplateType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ETemplateType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ETemplateType type, bool selected)
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
				listControl.Items.Add(GetListItem(ETemplateType.IndexPageTemplate, false));
				listControl.Items.Add(GetListItem(ETemplateType.ChannelTemplate, false));
				listControl.Items.Add(GetListItem(ETemplateType.ContentTemplate, false));
				listControl.Items.Add(GetListItem(ETemplateType.FileTemplate, false));
			}
		}

	}
}
