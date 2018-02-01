using System.Web.UI.WebControls;
using SiteServer.Plugin;

namespace SiteServer.Utils
{
	public class DataTypeUtils
	{
		public static DataType GetEnumType(string typeStr)
		{
			var retval = DataType.VarChar;

			if (Equals(DataType.Boolean, typeStr))
			{
				retval = DataType.Boolean;
			}
			else if (Equals(DataType.DateTime, typeStr))
			{
				retval = DataType.DateTime;
			}
			else if (Equals(DataType.Decimal, typeStr))
			{
				retval = DataType.Decimal;
			}
			else if (Equals(DataType.Integer, typeStr))
			{
				retval = DataType.Integer;
			}
			else if (Equals(DataType.Text, typeStr))
			{
				retval = DataType.Text;
			}
			else if (Equals(DataType.VarChar, typeStr))
			{
				retval = DataType.VarChar;
			}

			return retval;
		}

		public static bool Equals(DataType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, DataType type)
        {
            return Equals(type, typeStr);
        }

        public static ListItem GetListItem(DataType type, string text)
		{
            return new ListItem(text, type.Value);
		}

		public static void AddListItems(ListControl listControl)
		{
		    if (listControl == null) return;

		    listControl.Items.Add(GetListItem(DataType.VarChar, "文本"));
		    listControl.Items.Add(GetListItem(DataType.Text, "备注"));
		    listControl.Items.Add(GetListItem(DataType.Integer, "整数"));
		    listControl.Items.Add(GetListItem(DataType.DateTime, "日期"));
            listControl.Items.Add(GetListItem(DataType.Decimal, "小数"));
            listControl.Items.Add(GetListItem(DataType.Boolean, "布尔值"));
        }

        public static string GetText(DataType dataType)
        {
            var retval = string.Empty;
            if (dataType == DataType.VarChar)
            {
                retval = "文本";
            }
            else if (dataType == DataType.Text)
            {
                retval = "备注";
            }
            else if (dataType == DataType.Integer)
            {
                retval = "整数";
            }
            else if (dataType == DataType.DateTime)
            {
                retval = "日期";
            }
            else if (dataType == DataType.Decimal)
            {
                retval = "小数";
            }
            else if (dataType == DataType.Boolean)
            {
                retval = "布尔值";
            }
            return retval;
        }
	}
}
