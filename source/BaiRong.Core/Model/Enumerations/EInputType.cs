using System;
using System.Web.UI.WebControls;

namespace BaiRong.Core.Model.Enumerations
{
	/// <summary>
	/// 表单提交类型
	/// </summary>
	public enum EInputType
	{
		CheckBox,
        Radio,
        SelectOne,
        SelectMultiple,
		Date,
        DateTime,
        Image,
        Video,
        File,
		Text,
		TextArea,
		TextEditor,
        RelatedField,
        SpecifiedValue
	}

	public class EInputTypeUtils
	{
		public static string GetValue(EInputType type)
		{
            if (type == EInputType.CheckBox)
            {
                return "CheckBox";
            }
			else if (type == EInputType.Radio)
			{
				return "Radio";
			}
			else if (type == EInputType.SelectOne)
			{
				return "SelectOne";
			}
			else if (type == EInputType.SelectMultiple)
			{
				return "SelectMultiple";
            }
            else if (type == EInputType.Date)
            {
                return "Date";
            }
            else if (type == EInputType.DateTime)
            {
                return "DateTime";
            }
            else if (type == EInputType.Image)
            {
                return "Image";
            }
            else if (type == EInputType.Video)
            {
                return "Video";
            }
            else if (type == EInputType.File)
            {
                return "File";
            }
			else if (type == EInputType.Text)
			{
				return "Text";
			}
			else if (type == EInputType.TextArea)
			{
				return "TextArea";
			}
			else if (type == EInputType.TextEditor)
			{
				return "TextEditor";
            }
            else if (type == EInputType.RelatedField)
            {
                return "RelatedField";
            }
            else if (type == EInputType.SpecifiedValue)
            {
                return "SpecifiedValue";
            }
			else
			{
				throw new Exception();
			}
		}

		public static string GetText(EInputType type)
		{
			if (type == EInputType.CheckBox)
            {
                return "复选列表(checkbox)";
            }
			else if (type == EInputType.Radio)
			{
                return "单选列表(radio)";
			}
			else if (type == EInputType.SelectOne)
			{
                return "下拉列表(select单选)";
			}
			else if (type == EInputType.SelectMultiple)
			{
                return "下拉列表(select多选)";
            }
            else if (type == EInputType.Date)
            {
                return "日期选择框";
            }
            else if (type == EInputType.DateTime)
            {
                return "日期时间选择框";
            }
            else if (type == EInputType.Image)
            {
                return "图片";
            }
            else if (type == EInputType.Video)
            {
                return "视频";
            }
            else if (type == EInputType.File)
            {
                return "附件";
            }
			else if (type == EInputType.Text)
			{
                return "文本框(单行)";
			}
			else if (type == EInputType.TextArea)
			{
                return "文本框(多行)";
			}
			else if (type == EInputType.TextEditor)
			{
				return "内容编辑器";
            }
            else if (type == EInputType.RelatedField)
            {
                return "联动字段";
            }
            else if (type == EInputType.SpecifiedValue)
            {
                return "系统指定值";
            }
			else
			{
				throw new Exception();
			}
		}

		public static EInputType GetEnumType(string typeStr)
		{
			var retval = EInputType.Text;

			if (Equals(EInputType.CheckBox, typeStr))
            {
                retval = EInputType.CheckBox;
            }
			else if (Equals(EInputType.Radio, typeStr))
			{
				retval = EInputType.Radio;
			}
			else if (Equals(EInputType.SelectOne, typeStr))
			{
				retval = EInputType.SelectOne;
			}
			else if (Equals(EInputType.SelectMultiple, typeStr))
			{
				retval = EInputType.SelectMultiple;
            }
            else if (Equals(EInputType.Date, typeStr))
            {
                retval = EInputType.Date;
            }
            else if (Equals(EInputType.DateTime, typeStr))
            {
                retval = EInputType.DateTime;
            }
            else if (Equals(EInputType.Image, typeStr))
            {
                retval = EInputType.Image;
            }
            else if (Equals(EInputType.Video, typeStr))
            {
                retval = EInputType.Video;
            }
            else if (Equals(EInputType.File, typeStr))
            {
                retval = EInputType.File;
            }
			else if (Equals(EInputType.Text, typeStr))
			{
				retval = EInputType.Text;
			}
			else if (Equals(EInputType.TextArea, typeStr))
			{
				retval = EInputType.TextArea;
			}
			else if (Equals(EInputType.TextEditor, typeStr))
			{
				retval = EInputType.TextEditor;
            }
            else if (Equals(EInputType.RelatedField, typeStr))
            {
                retval = EInputType.RelatedField;
            }
            else if (Equals(EInputType.SpecifiedValue, typeStr))
            {
                retval = EInputType.SpecifiedValue;
            }

			return retval;
		}

		public static bool Equals(EInputType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, EInputType type)
        {
            return Equals(type, typeStr);
        }

        public static bool EqualsAny(string typeStr, params EInputType[] types)
        {
            foreach (var type in types)
            {
                if (Equals(type, typeStr))
                {
                    return true;
                }
            }
            return false;
        }

        public static ListItem GetListItem(EInputType type, bool selected)
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
                listControl.Items.Add(GetListItem(EInputType.Text, false));
                listControl.Items.Add(GetListItem(EInputType.TextArea, false));
                listControl.Items.Add(GetListItem(EInputType.TextEditor, false));
                listControl.Items.Add(GetListItem(EInputType.CheckBox, false));
                listControl.Items.Add(GetListItem(EInputType.Radio, false));
                listControl.Items.Add(GetListItem(EInputType.SelectOne, false));
                listControl.Items.Add(GetListItem(EInputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(EInputType.Date, false));
                listControl.Items.Add(GetListItem(EInputType.DateTime, false));
                listControl.Items.Add(GetListItem(EInputType.Image, false));
                listControl.Items.Add(GetListItem(EInputType.Video, false));
                listControl.Items.Add(GetListItem(EInputType.File, false));
                listControl.Items.Add(GetListItem(EInputType.RelatedField, false));
                listControl.Items.Add(GetListItem(EInputType.SpecifiedValue, false));
            }
        }

        /// <summary>
        /// 用户字段类型
        /// </summary>
        /// <param name="listControl"></param>
        public static void AddListItemsForUser(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EInputType.Text, false));
                listControl.Items.Add(GetListItem(EInputType.TextArea, false));
                listControl.Items.Add(GetListItem(EInputType.TextEditor, false));
                listControl.Items.Add(GetListItem(EInputType.CheckBox, false));
                listControl.Items.Add(GetListItem(EInputType.Radio, false));
                listControl.Items.Add(GetListItem(EInputType.SelectOne, false));
                listControl.Items.Add(GetListItem(EInputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(EInputType.Date, false));
                listControl.Items.Add(GetListItem(EInputType.DateTime, false));
                //listControl.Items.Add(GetListItem(EInputType.Image, false));
                //listControl.Items.Add(GetListItem(EInputType.Video, false));
                //listControl.Items.Add(GetListItem(EInputType.File, false));
                //listControl.Items.Add(GetListItem(EInputType.RelatedField, false));
                //listControl.Items.Add(GetListItem(EInputType.SpecifiedValue, false));
            }
        }

        public static void AddListItems(ListControl listControl, EDataType dataType)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EInputType.Text, false));
                listControl.Items.Add(GetListItem(EInputType.TextArea, false));
                if (dataType == EDataType.NText || dataType == EDataType.Text)
                {
                    listControl.Items.Add(GetListItem(EInputType.TextEditor, false));
                }
                listControl.Items.Add(GetListItem(EInputType.CheckBox, false));
                listControl.Items.Add(GetListItem(EInputType.Radio, false));
                listControl.Items.Add(GetListItem(EInputType.SelectOne, false));
                listControl.Items.Add(GetListItem(EInputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(EInputType.Date, false));
                listControl.Items.Add(GetListItem(EInputType.DateTime, false));
                listControl.Items.Add(GetListItem(EInputType.Image, false));
                listControl.Items.Add(GetListItem(EInputType.Video, false));
                listControl.Items.Add(GetListItem(EInputType.File, false));
                listControl.Items.Add(GetListItem(EInputType.RelatedField, false));
            }
        }

        public static void AddListItemsToText(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(EInputType.Text, false));
                listControl.Items.Add(GetListItem(EInputType.TextArea, false));
                listControl.Items.Add(GetListItem(EInputType.TextEditor, false));
            }
        }

        public static bool IsWithStyleItems(EInputType type)
        {
            if (type == EInputType.CheckBox || type == EInputType.Radio || type == EInputType.SelectMultiple || type == EInputType.SelectOne || type == EInputType.RelatedField)
            {
                return true;
            }
            return false;
        }

        public static bool IsPureString(EInputType type)
        {
            if (type == EInputType.Date || type == EInputType.DateTime || type == EInputType.CheckBox || type == EInputType.Radio || type == EInputType.SelectMultiple || type == EInputType.SelectOne || type == EInputType.Image || type == EInputType.Video || type == EInputType.File || type == EInputType.RelatedField)
            {
                return false;
            }
            return true;
        }
	}
}
