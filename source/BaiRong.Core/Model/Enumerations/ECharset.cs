using System;
using System.Text;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	public enum ECharset
	{
        utf_8,                  //Unicode (UTF-8)
        gb2312,                 //简体中文 (GB2312)
        big5,                   //繁体中文 (Big5)
        iso_8859_1,             //西欧 (iso-8859-1)
        euc_kr,                 //韩文 (euc-kr)
        euc_jp,                 //日文 (euc-jp)
        iso_8859_6,             //阿拉伯文 (iso-8859-6)
        windows_874,            //泰文 (windows-874)
        iso_8859_9,             //土耳其文 (iso-8859-9)
        iso_8859_5,             //西里尔文 (iso-8859-5)
        iso_8859_8,             //希伯来文 (iso-8859-8)
        iso_8859_7,             //希腊文 (iso-8859-7)
        windows_1258,           //越南文 (windows-1258)
        iso_8859_2,             //中欧 (iso-8859-2)
	}

    public class ECharsetUtils
	{
		public static string GetValue(ECharset type)
		{
			if (type == ECharset.utf_8)
			{
                return "utf-8";
			}
            else if (type == ECharset.gb2312)
			{
                return "gb2312";
			}
            else if (type == ECharset.big5)
			{
                return "big5";
			}
            else if (type == ECharset.iso_8859_1)
			{
                return "iso-8859-1";
            }
            else if (type == ECharset.euc_kr)
            {
                return "euc-kr";
            }
            else if (type == ECharset.euc_jp)
            {
                return "euc-jp";
            }
            else if (type == ECharset.iso_8859_6)
            {
                return "iso-8859-6";
            }
            else if (type == ECharset.windows_874)
            {
                return "windows-874";
            }
            else if (type == ECharset.iso_8859_9)
			{
                return "iso-8859-9";
			}
            else if (type == ECharset.iso_8859_5)
			{
                return "iso-8859-5";
			}
            else if (type == ECharset.iso_8859_8)
			{
                return "iso-8859-8";
            }
            else if (type == ECharset.iso_8859_7)
            {
                return "iso-8859-7";
            }
            else if (type == ECharset.windows_1258)
            {
                return "windows-1258";
            }
            else if (type == ECharset.iso_8859_2)
            {
                return "iso-8859-2";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ECharset type)
		{
            if (type == ECharset.utf_8)
            {
                return "Unicode (UTF-8)";
            }
            else if (type == ECharset.gb2312)
            {
                return "简体中文 (GB2312)";
            }
            else if (type == ECharset.big5)
            {
                return "繁体中文 (Big5)";
            }
            else if (type == ECharset.iso_8859_1)
            {
                return "西欧 (iso-8859-1)";
            }
            else if (type == ECharset.euc_kr)
            {
                return "韩文 (euc-kr)";
            }
            else if (type == ECharset.euc_jp)
            {
                return "日文 (euc-jp)";
            }
            else if (type == ECharset.iso_8859_6)
            {
                return "阿拉伯文 (iso-8859-6)";
            }
            else if (type == ECharset.windows_874)
            {
                return "泰文 (windows-874)";
            }
            else if (type == ECharset.iso_8859_9)
            {
                return "土耳其文 (iso-8859-9)";
            }
            else if (type == ECharset.iso_8859_5)
            {
                return "西里尔文 (iso-8859-5)";
            }
            else if (type == ECharset.iso_8859_8)
            {
                return "希伯来文 (iso-8859-8)";
            }
            else if (type == ECharset.iso_8859_7)
            {
                return "希腊文 (iso-8859-7)";
            }
            else if (type == ECharset.windows_1258)
            {
                return "越南文 (windows-1258)";
            }
            else if (type == ECharset.iso_8859_2)
            {
                return "中欧 (iso-8859-2)";
            }
            else
            {
                throw new Exception();
            }
		}

		public static ECharset GetEnumType(string typeStr)
		{
			var retval = ECharset.gb2312;

            if (Equals(ECharset.utf_8, typeStr))
            {
                return ECharset.utf_8;
            }
            else if (Equals(ECharset.gb2312, typeStr))
            {
                return ECharset.gb2312;
            }
            else if (Equals(ECharset.big5, typeStr))
            {
                return ECharset.big5;
            }
            else if (Equals(ECharset.iso_8859_1, typeStr))
            {
                return ECharset.iso_8859_1;
            }
            else if (Equals(ECharset.euc_kr, typeStr))
            {
                return ECharset.euc_kr;
            }
            else if (Equals(ECharset.euc_jp, typeStr))
            {
                return ECharset.euc_jp;
            }
            else if (Equals(ECharset.iso_8859_6, typeStr))
            {
                return ECharset.iso_8859_6;
            }
            else if (Equals(ECharset.windows_874, typeStr))
            {
                return ECharset.windows_874;
            }
            else if (Equals(ECharset.iso_8859_9, typeStr))
            {
                return ECharset.iso_8859_9;
            }
            else if (Equals(ECharset.iso_8859_5, typeStr))
            {
                return ECharset.iso_8859_5;
            }
            else if (Equals(ECharset.iso_8859_8, typeStr))
            {
                return ECharset.iso_8859_8;
            }
            else if (Equals(ECharset.iso_8859_7, typeStr))
            {
                return ECharset.iso_8859_7;
            }
            else if (Equals(ECharset.windows_1258, typeStr))
            {
                return ECharset.windows_1258;
            }
            else if (Equals(ECharset.iso_8859_2, typeStr))
            {
                return ECharset.iso_8859_2;
            }

			return retval;
		}

		public static bool Equals(ECharset type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ECharset type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ECharset type, bool selected)
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
            return Encoding.GetEncoding(GetValue(type));
        }

        public static Encoding GetEncoding(string typeStr)
        {
            if (StringUtils.EqualsIgnoreCase("utf-8", typeStr))
            {
                return new UTF8Encoding(false);
            }
            return Encoding.GetEncoding(typeStr);
        }

        private static readonly Encoding gb2312 = Encoding.GetEncoding("gb2312");
        public static Encoding GB2312 => gb2312;
	}
}
