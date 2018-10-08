using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
	public class InputTypeUtils
	{
		public static string GetText(InputType type)
		{
		    if (type == InputType.CheckBox)
            {
                return "复选框";
            }
		    if (type == InputType.Radio)
		    {
		        return "单选框";
		    }
		    if (type == InputType.SelectOne)
		    {
		        return "下拉列表(单选)";
		    }
		    if (type == InputType.SelectMultiple)
		    {
		        return "下拉列表(多选)";
		    }
            if (type == InputType.SelectCascading)
            {
                return "下拉列表(级联)";
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
            if (type == InputType.Customize)
            {
                return "自定义";
            }
            if (type == InputType.Hidden)
            {
                return "隐藏";
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
            else if (Equals(InputType.SelectCascading, typeStr))
            {
                retval = InputType.SelectCascading;
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
            else if (Equals(InputType.Customize, typeStr))
            {
                retval = InputType.Customize;
            }
            else if (Equals(InputType.Hidden, typeStr))
            {
                retval = InputType.Hidden;
            }

            return retval;
		}

		public static bool Equals(InputType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(type.Value.ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static bool Equals(string typeStr, InputType type)
        {
            return Equals(type, typeStr);
        }

        public static bool EqualsAny(InputType type, params InputType[] types)
        {
            foreach (var theType in types)
            {
                if (type == theType)
                {
                    return true;
                }
            }
            return false;
        }

        public static ListItem GetListItem(InputType type, bool selected)
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
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                listControl.Items.Add(GetListItem(InputType.Radio, false));
                listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(InputType.SelectCascading, false));
                listControl.Items.Add(GetListItem(InputType.Date, false));
                listControl.Items.Add(GetListItem(InputType.DateTime, false));
                listControl.Items.Add(GetListItem(InputType.Image, false));
                listControl.Items.Add(GetListItem(InputType.Video, false));
                listControl.Items.Add(GetListItem(InputType.File, false));
                listControl.Items.Add(GetListItem(InputType.Customize, false));
                listControl.Items.Add(GetListItem(InputType.Hidden, false));
            }
        }

        public static void AddListItems(ListControl listControl, DataType dataType)
        {
            if (listControl != null)
            {
                listControl.Items.Add(GetListItem(InputType.Text, false));
                listControl.Items.Add(GetListItem(InputType.TextArea, false));
                if (dataType == DataType.Text)
                {
                    listControl.Items.Add(GetListItem(InputType.TextEditor, false));
                }
                listControl.Items.Add(GetListItem(InputType.CheckBox, false));
                listControl.Items.Add(GetListItem(InputType.Radio, false));
                listControl.Items.Add(GetListItem(InputType.SelectOne, false));
                listControl.Items.Add(GetListItem(InputType.SelectMultiple, false));
                listControl.Items.Add(GetListItem(InputType.SelectCascading, false));
                listControl.Items.Add(GetListItem(InputType.Date, false));
                listControl.Items.Add(GetListItem(InputType.DateTime, false));
                listControl.Items.Add(GetListItem(InputType.Image, false));
                listControl.Items.Add(GetListItem(InputType.Video, false));
                listControl.Items.Add(GetListItem(InputType.File, false));
                listControl.Items.Add(GetListItem(InputType.Customize, false));
                listControl.Items.Add(GetListItem(InputType.Hidden, false));
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
            if (type == InputType.CheckBox || type == InputType.Radio || type == InputType.SelectMultiple || type == InputType.SelectOne || type == InputType.SelectCascading)
            {
                return true;
            }
            return false;
        }

        public static bool IsPureString(InputType type)
        {
            if (type == InputType.Date || type == InputType.DateTime || type == InputType.CheckBox || type == InputType.Radio || type == InputType.SelectMultiple || type == InputType.SelectOne || type == InputType.Image || type == InputType.Video || type == InputType.File || type == InputType.SelectCascading)
            {
                return false;
            }
            return true;
        }

	    public static List<KeyValuePair<InputType, string>> GetInputTypes(string tableName)
	    {
	        if (tableName == DataProvider.UserDao.TableName)
	        {
	            return new List<KeyValuePair<InputType, string>>
	            {
	                new KeyValuePair<InputType, string>(InputType.Text, GetText(InputType.Text)),
	                new KeyValuePair<InputType, string>(InputType.TextArea, GetText(InputType.TextArea)),
	                new KeyValuePair<InputType, string>(InputType.CheckBox, GetText(InputType.CheckBox)),
	                new KeyValuePair<InputType, string>(InputType.Radio, GetText(InputType.Radio)),
	                new KeyValuePair<InputType, string>(InputType.SelectOne, GetText(InputType.SelectOne)),
	                new KeyValuePair<InputType, string>(InputType.SelectMultiple, GetText(InputType.SelectMultiple)),
	                new KeyValuePair<InputType, string>(InputType.Date, GetText(InputType.Date)),
	                new KeyValuePair<InputType, string>(InputType.DateTime, GetText(InputType.DateTime)),
	                new KeyValuePair<InputType, string>(InputType.Image, GetText(InputType.Image)),
	                new KeyValuePair<InputType, string>(InputType.Video, GetText(InputType.Video)),
	                new KeyValuePair<InputType, string>(InputType.File, GetText(InputType.File)),
	                new KeyValuePair<InputType, string>(InputType.Hidden, GetText(InputType.Hidden))
	            };
            }

	        return new List<KeyValuePair<InputType, string>>
	        {
	            new KeyValuePair<InputType, string>(InputType.Text, GetText(InputType.Text)),
	            new KeyValuePair<InputType, string>(InputType.TextArea, GetText(InputType.TextArea)),
	            new KeyValuePair<InputType, string>(InputType.TextEditor, GetText(InputType.TextEditor)),
	            new KeyValuePair<InputType, string>(InputType.CheckBox, GetText(InputType.CheckBox)),
	            new KeyValuePair<InputType, string>(InputType.Radio, GetText(InputType.Radio)),
	            new KeyValuePair<InputType, string>(InputType.SelectOne, GetText(InputType.SelectOne)),
	            new KeyValuePair<InputType, string>(InputType.SelectMultiple, GetText(InputType.SelectMultiple)),
	            new KeyValuePair<InputType, string>(InputType.SelectCascading, GetText(InputType.SelectCascading)),
	            new KeyValuePair<InputType, string>(InputType.Date, GetText(InputType.Date)),
	            new KeyValuePair<InputType, string>(InputType.DateTime, GetText(InputType.DateTime)),
	            new KeyValuePair<InputType, string>(InputType.Image, GetText(InputType.Image)),
	            new KeyValuePair<InputType, string>(InputType.Video, GetText(InputType.Video)),
	            new KeyValuePair<InputType, string>(InputType.File, GetText(InputType.File)),
	            new KeyValuePair<InputType, string>(InputType.Customize, GetText(InputType.Customize)),
	            new KeyValuePair<InputType, string>(InputType.Hidden, GetText(InputType.Hidden))
	        };
        }

        public static string ParseString(InputType inputType, string content, string replace, string to, int startIndex, int length, int wordNum, string ellipsis, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string formatString)
        {
            return IsPureString(inputType) ? ParseString(content, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString) : content;
        }

        private static string ParseString(string content, string replace, string to, int startIndex, int length, int wordNum, string ellipsis, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string formatString)
        {
            var parsedContent = content;

            if (!string.IsNullOrEmpty(replace))
            {
                parsedContent = StringUtils.ParseReplace(parsedContent, replace, to);
            }

            if (isClearTags)
            {
                parsedContent = StringUtils.StripTags(parsedContent);
            }

            if (!string.IsNullOrEmpty(parsedContent))
            {
                if (startIndex > 0 || length > 0)
                {
                    try
                    {
                        parsedContent = length > 0 ? parsedContent.Substring(startIndex, length) : parsedContent.Substring(startIndex);
                    }
                    catch
                    {
                        // ignored
                    }
                }

                if (wordNum > 0)
                {
                    parsedContent = StringUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
                }

                if (isReturnToBr)
                {
                    parsedContent = StringUtils.ReplaceNewlineToBr(parsedContent);
                }

                if (!string.IsNullOrEmpty(formatString))
                {
                    parsedContent = string.Format(formatString, parsedContent);
                }

                if (isLower)
                {
                    parsedContent = parsedContent.ToLower();
                }
                if (isUpper)
                {
                    parsedContent = parsedContent.ToUpper();
                }
            }

            return parsedContent;
        }
    }
}
