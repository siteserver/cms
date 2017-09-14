using System;
using System.Web.UI.WebControls;
using SiteServer.Plugin.Models;

namespace BaiRong.Core
{
	public class InputTypeUtils
	{
		public static string GetValue(InputType type)
		{
		    if (type == InputType.CheckBox)
            {
                return "CheckBox";
            }
		    if (type == InputType.Radio)
		    {
		        return "Radio";
		    }
		    if (type == InputType.SelectOne)
		    {
		        return "SelectOne";
		    }
		    if (type == InputType.SelectMultiple)
		    {
		        return "SelectMultiple";
		    }
		    if (type == InputType.Date)
		    {
		        return "Date";
		    }
		    if (type == InputType.DateTime)
		    {
		        return "DateTime";
		    }
		    if (type == InputType.Image)
		    {
		        return "Image";
		    }
		    if (type == InputType.Video)
		    {
		        return "Video";
		    }
		    if (type == InputType.File)
		    {
		        return "File";
		    }
		    if (type == InputType.Text)
		    {
		        return "Text";
		    }
		    if (type == InputType.TextArea)
		    {
		        return "TextArea";
		    }
		    if (type == InputType.TextEditor)
		    {
		        return "TextEditor";
		    }
		    if (type == InputType.RelatedField)
		    {
		        return "RelatedField";
		    }
		    throw new Exception();
		}

		public static string GetText(InputType type)
		{
		    if (type == InputType.CheckBox)
            {
                return "复选列表(checkbox)";
            }
		    if (type == InputType.Radio)
		    {
		        return "单选列表(radio)";
		    }
		    if (type == InputType.SelectOne)
		    {
		        return "下拉列表(select单选)";
		    }
		    if (type == InputType.SelectMultiple)
		    {
		        return "下拉列表(select多选)";
		    }
		    if (type == InputType.Date)
		    {
		        return "日期选择框";
		    }
		    if (type == InputType.DateTime)
		    {
		        return "日期时间选择框";
		    }
		    if (type == InputType.Image)
		    {
		        return "图片";
		    }
		    if (type == InputType.Video)
		    {
		        return "视频";
		    }
		    if (type == InputType.File)
		    {
		        return "附件";
		    }
		    if (type == InputType.Text)
		    {
		        return "文本框(单行)";
		    }
		    if (type == InputType.TextArea)
		    {
		        return "文本框(多行)";
		    }
		    if (type == InputType.TextEditor)
		    {
		        return "内容编辑器";
		    }
		    if (type == InputType.RelatedField)
		    {
		        return "联动字段";
		    }
		    throw new Exception();
		}

		public static InputType GetEnumType(string typeStr)
		{
			var retval = InputType.Text;

			if (Equals(InputType.CheckBox, typeStr))
            {
                retval = InputType.CheckBox;
            }
			else if (Equals(InputType.Radio, typeStr))
			{
				retval = InputType.Radio;
			}
			else if (Equals(InputType.SelectOne, typeStr))
			{
				retval = InputType.SelectOne;
			}
			else if (Equals(InputType.SelectMultiple, typeStr))
			{
				retval = InputType.SelectMultiple;
            }
            else if (Equals(InputType.Date, typeStr))
            {
                retval = InputType.Date;
            }
            else if (Equals(InputType.DateTime, typeStr))
            {
                retval = InputType.DateTime;
            }
            else if (Equals(InputType.Image, typeStr))
            {
                retval = InputType.Image;
            }
            else if (Equals(InputType.Video, typeStr))
            {
                retval = InputType.Video;
            }
            else if (Equals(InputType.File, typeStr))
            {
                retval = InputType.File;
            }
			else if (Equals(InputType.Text, typeStr))
			{
				retval = InputType.Text;
			}
			else if (Equals(InputType.TextArea, typeStr))
			{
				retval = InputType.TextArea;
			}
			else if (Equals(InputType.TextEditor, typeStr))
			{
				retval = InputType.TextEditor;
            }
            else if (Equals(InputType.RelatedField, typeStr))
            {
                retval = InputType.RelatedField;
            }

			return retval;
		}

		public static bool Equals(InputType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, InputType type)
        {
            return Equals(type, typeStr);
        }

        public static bool EqualsAny(string typeStr, params InputType[] types)
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

        public static ListItem GetListItem(InputType type, bool selected)
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
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                listControl.Items.Add(GetListItem(InputType.Radio, false));
                listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(InputType.Date, false));
                listControl.Items.Add(GetListItem(InputType.DateTime, false));
                listControl.Items.Add(GetListItem(InputType.Image, false));
                listControl.Items.Add(GetListItem(InputType.Video, false));
                listControl.Items.Add(GetListItem(InputType.File, false));
                listControl.Items.Add(GetListItem(InputType.RelatedField, false));
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
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                listControl.Items.Add(GetListItem(InputType.Radio, false));
                listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(InputType.Date, false));
                listControl.Items.Add(GetListItem(InputType.DateTime, false));
                //listControl.Items.Add(GetListItem(InputType.Image, false));
                //listControl.Items.Add(GetListItem(InputType.Video, false));
                //listControl.Items.Add(GetListItem(InputType.File, false));
                //listControl.Items.Add(GetListItem(InputType.RelatedField, false));
                //listControl.Items.Add(GetListItem(InputType.SpecifiedValue, false));
            }
        }

        public static void AddListItems(ListControl listControl, DataType dataType)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                if (dataType == DataType.NText || dataType == DataType.Text)
                {
                    listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                }
                listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                listControl.Items.Add(GetListItem(InputType.Radio, false));
                listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(InputType.Date, false));
                listControl.Items.Add(GetListItem(InputType.DateTime, false));
                listControl.Items.Add(GetListItem(InputType.Image, false));
                listControl.Items.Add(GetListItem(InputType.Video, false));
                listControl.Items.Add(GetListItem(InputType.File, false));
                listControl.Items.Add(GetListItem(InputType.RelatedField, false));
            }
        }

        public static void AddListItemsToText(ListControl listControl)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                listControl.Items.Add(GetListItem(InputType.TextEditor, false));
            }
        }

        public static bool IsWithStyleItems(InputType type)
        {
            if (type == InputType.CheckBox || type == InputType.Radio || type == InputType.SelectMultiple || type == InputType.SelectOne || type == InputType.RelatedField)
            {
                return true;
            }
            return false;
        }

        public static bool IsPureString(InputType type)
        {
            if (type == InputType.Date || type == InputType.DateTime || type == InputType.CheckBox || type == InputType.Radio || type == InputType.SelectMultiple || type == InputType.SelectOne || type == InputType.Image || type == InputType.Video || type == InputType.File || type == InputType.RelatedField)
            {
                return false;
            }
            return true;
        }
	}
}
