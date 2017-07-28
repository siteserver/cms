using System;
using System.Web.UI.WebControls;

namespace SiteServer.CMS.Model.Enumerations
{
    /// <summary>
    /// by 20160225 sofuny 可统计分析的表单类型
    /// </summary>
    public enum EStatisticsInputType
    {
        Text,
        CheckBox,
        Radio,
        SelectOne,
	}

    public class EStatisticsInputTypeUtils
    {
        public static string GetValue(EStatisticsInputType type)
        {
            if (type == EStatisticsInputType.Text)
            {
                return "Text";
            }
            else if (type == EStatisticsInputType.CheckBox)
            {
                return "CheckBox";
            }
            else if (type == EStatisticsInputType.Radio)
            {
                return "Radio";
            }
            else if (type == EStatisticsInputType.SelectOne)
            {
                return "SelectOne";
            }
            else
            {
                throw new Exception();
            }
        }

        public static string GetText(EStatisticsInputType type)
        {
            if (type == EStatisticsInputType.Text)
            {
                return "文本框(单行)";
            }
            if (type == EStatisticsInputType.CheckBox)
            {
                return "复选列表(checkbox)";
            }
            else if (type == EStatisticsInputType.Radio)
            {
                return "单选列表(radio)";
            }
            else if (type == EStatisticsInputType.SelectOne)
            {
                return "下拉列表(select单选)";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EStatisticsInputType GetEnumType(string typeStr)
        {
            var retval = EStatisticsInputType.Text;

            if (Equals(EStatisticsInputType.Text, typeStr))
            {
                retval = EStatisticsInputType.Text;
            }
            else if (Equals(EStatisticsInputType.CheckBox, typeStr))
            {
                retval = EStatisticsInputType.CheckBox;
            }
            else if (Equals(EStatisticsInputType.Radio, typeStr))
            {
                retval = EStatisticsInputType.Radio;
            }
            else if (Equals(EStatisticsInputType.SelectOne, typeStr))
            {
                retval = EStatisticsInputType.SelectOne;
            }

            return retval;
        }

        public static bool Equals(EStatisticsInputType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EStatisticsInputType type)
        {
            return Equals(type, typeStr);
        }

        public static void AddListItems(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EStatisticsInputType.Text, false));
                listControl.Items.Add(GetListItem(EStatisticsInputType.CheckBox, false));
                listControl.Items.Add(GetListItem(EStatisticsInputType.Radio, false));
                listControl.Items.Add(GetListItem(EStatisticsInputType.SelectOne, false));
            }
        }

        public static ListItem GetListItem(EStatisticsInputType type, bool selected)
        {
            var item = new ListItem(GetText(type), GetValue(type));
            if (selected)
            {
                item.Selected = true;
            }
            return item;
        }

    }
}
