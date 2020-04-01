using System.Web.UI.WebControls;
using Datory;

namespace SiteServer.Utils
{
	public class DataTypeUtils
	{
		public static DataType GetEnumType(string typeStr)
		{
			var retVal = DataType.VarChar;

			if (Equals(DataType.Boolean, typeStr))
			{
				retVal = DataType.Boolean;
			}
			else if (Equals(DataType.DateTime, typeStr))
			{
				retVal = DataType.DateTime;
			}
			else if (Equals(DataType.Decimal, typeStr))
			{
				retVal = DataType.Decimal;
			}
			else if (Equals(DataType.Integer, typeStr))
			{
				retVal = DataType.Integer;
			}
			else if (Equals(DataType.Text, typeStr))
			{
				retVal = DataType.Text;
			}
			else if (Equals(DataType.VarChar, typeStr))
			{
				retVal = DataType.VarChar;
			}

			return retVal;
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
            var retVal = string.Empty;
            if (dataType == DataType.VarChar)
            {
                retVal = "文本";
            }
            else if (dataType == DataType.Text)
            {
                retVal = "备注";
            }
            else if (dataType == DataType.Integer)
            {
                retVal = "整数";
            }
            else if (dataType == DataType.DateTime)
            {
                retVal = "日期";
            }
            else if (dataType == DataType.Decimal)
            {
                retVal = "小数";
            }
            else if (dataType == DataType.Boolean)
            {
                retVal = "布尔值";
            }
            return retVal;
        }
	}
}
