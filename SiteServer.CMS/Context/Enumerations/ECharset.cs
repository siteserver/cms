using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Abstractions;

namespace SiteServer.CMS.Context.Enumerations
{

    public static class ECharsetUtilsExtensions
	{
        public static ListItem GetListItem(ECharset type, bool selected)
		{
			var item = new ListItem(ECharsetUtils.GetText(type), ECharsetUtils.GetValue(type));
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
                listControl.Items.Add(GetListItem(ECharset.utf_8, false));
                listControl.Items.Add(GetListItem(ECharset.gb2312, false));
                listControl.Items.Add(GetListItem(ECharset.big5, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_1, false));
                listControl.Items.Add(GetListItem(ECharset.euc_kr, false));
                listControl.Items.Add(GetListItem(ECharset.euc_jp, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_6, false));
                listControl.Items.Add(GetListItem(ECharset.windows_874, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_9, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_5, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_8, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_7, false));
                listControl.Items.Add(GetListItem(ECharset.windows_1258, false));
                listControl.Items.Add(GetListItem(ECharset.iso_8859_2, false));
            }
        }

        public static Encoding GetEncoding(ECharset type)
        {
            if (type == ECharset.utf_8)
            {
                return new UTF8Encoding(false);
            }
            return Encoding.GetEncoding(ECharsetUtils.GetValue(type));
        }

        public static Encoding GetEncoding(string typeStr)
        {
            if (StringUtils.EqualsIgnoreCase("utf-8", typeStr))
            {
                return new UTF8Encoding(false);
            }
            return Encoding.GetEncoding(typeStr);
        }

	    public static Encoding Gb2312 { get; } = Encoding.GetEncoding("gb2312");
	}
}
