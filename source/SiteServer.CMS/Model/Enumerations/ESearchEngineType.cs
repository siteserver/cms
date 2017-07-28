using System;
using System.Collections;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
	
	public enum ESearchEngineType
	{
		Baidu,						//百度
		Google_CN,					//Google(简体中文)
		Google,						//Google
		Yahoo,						//Yahoo
		Live,						//Live
		Sogou,						//搜狗
	}

	public class ESearchEngineTypeUtils
	{
		public static string GetValue(ESearchEngineType type)
		{
			if (type == ESearchEngineType.Baidu)
			{
				return "Baidu";
			}
			else if (type == ESearchEngineType.Google)
			{
				return "Google";
			}
			else if (type == ESearchEngineType.Google_CN)
			{
				return "Google_CN";
			}
			else if (type == ESearchEngineType.Live)
			{
				return "Live";
			}
			else if (type == ESearchEngineType.Sogou)
			{
				return "Sogou";
			}
			else if (type == ESearchEngineType.Yahoo)
			{
				return "Yahoo";
			}
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(ESearchEngineType type)
		{
			if (type == ESearchEngineType.Baidu)
			{
				return "百度";
			}
			else if (type == ESearchEngineType.Google_CN)
			{
				return "Google(简体中文)";
			}
			else if (type == ESearchEngineType.Google)
			{
				return "Google(全部语言)";
			}
			else if (type == ESearchEngineType.Yahoo)
			{
				return "Yahoo";
			}
			else if (type == ESearchEngineType.Live)
			{
				return "Live 搜索";
			}
			else if (type == ESearchEngineType.Sogou)
			{
				return "搜狗";
			}
			else
			{
				throw new Exception();
			}
		}

		public static ESearchEngineType GetEnumType(string typeStr)
		{
			var retval = ESearchEngineType.Baidu;

			if (Equals(ESearchEngineType.Baidu, typeStr))
			{
				retval = ESearchEngineType.Baidu;
			}
			else if (Equals(ESearchEngineType.Google, typeStr))
			{
				retval = ESearchEngineType.Google;
			}
			else if (Equals(ESearchEngineType.Google_CN, typeStr))
			{
				retval = ESearchEngineType.Google_CN;
			}
			else if (Equals(ESearchEngineType.Live, typeStr))
			{
				retval = ESearchEngineType.Live;
			}
			else if (Equals(ESearchEngineType.Sogou, typeStr))
			{
				retval = ESearchEngineType.Sogou;
			}
			else if (Equals(ESearchEngineType.Yahoo, typeStr))
			{
				retval = ESearchEngineType.Yahoo;
			}

			return retval;
		}

		public static bool Equals(ESearchEngineType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, ESearchEngineType type)
        {
            return Equals(type, typeStr);
        }

		public static ListItem GetListItem(ESearchEngineType type, bool selected)
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
				listControl.Items.Add(GetListItem(ESearchEngineType.Baidu, false));
				listControl.Items.Add(GetListItem(ESearchEngineType.Google_CN, false));
				listControl.Items.Add(GetListItem(ESearchEngineType.Google, false));
				listControl.Items.Add(GetListItem(ESearchEngineType.Yahoo, false));
				listControl.Items.Add(GetListItem(ESearchEngineType.Live, false));
				listControl.Items.Add(GetListItem(ESearchEngineType.Sogou, false));
			}
		}

		public static ArrayList GetSearchEngineTypeArrayList()
		{
			var arraylist = new ArrayList();
			arraylist.Add(ESearchEngineType.Baidu);
			arraylist.Add(ESearchEngineType.Google_CN);
			arraylist.Add(ESearchEngineType.Google);
			arraylist.Add(ESearchEngineType.Yahoo);
			arraylist.Add(ESearchEngineType.Live);
			arraylist.Add(ESearchEngineType.Sogou);
			return arraylist;
		}

	}
}
