using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Datory;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.Core
{
	public class InputTypeUtils
	{
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
			var item = new ListItem(type.GetDisplayName(), type.GetValue());
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

        public static bool IsPureString(InputType type)
        {
            if (type == InputType.Date || type == InputType.DateTime || type == InputType.CheckBox || type == InputType.Radio || type == InputType.SelectMultiple || type == InputType.SelectOne || type == InputType.Image || type == InputType.Video || type == InputType.File || type == InputType.SelectCascading)
            {
                return false;
            }
            return true;
        }

	    public static IEnumerable<KeyValuePair<InputType, string>> GetInputTypes()
	    {
            return TranslateUtils.GetEnums<InputType>().Select(inputType =>
                new KeyValuePair<InputType, string>(inputType, inputType.GetDisplayName()));
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
                    parsedContent = WebUtils.MaxLengthText(parsedContent, wordNum, ellipsis);
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
