using System.Collections.Generic;
using System.Linq;
using Datory;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
	public static class InputTypeUtils
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

        private static bool IsPureString(InputType type)
        {
            if (type == InputType.Date || type == InputType.DateTime || type == InputType.CheckBox || type == InputType.Radio || type == InputType.SelectMultiple || type == InputType.SelectOne || type == InputType.Image || type == InputType.Video || type == InputType.File || type == InputType.SelectCascading)
            {
                return false;
            }
            return true;
        }

	    public static IEnumerable<KeyValuePair<InputType, string>> GetInputTypes()
	    {
            return ListUtils.GetEnums<InputType>().Select(inputType =>
                new KeyValuePair<InputType, string>(inputType, inputType.GetDisplayName()));
        }

        public static string ParseString(InputType inputType, string content, string replace, string to, int startIndex, int length, int wordNum, string ellipsis, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string formatString)
        {
            return IsPureString(inputType) ? StringUtils.ParseString(content, replace, to, startIndex, length, wordNum, ellipsis, isClearTags, isReturnToBr, isLower, isUpper, formatString) : content;
        }
    }
}
