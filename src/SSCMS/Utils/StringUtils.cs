using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SSCMS.Configuration;

namespace SSCMS.Utils
{
    public static class StringUtils
    {
        public static bool IsMobile(string val)
        {
            return !string.IsNullOrEmpty(val) && Regex.IsMatch(val, @"^1[3456789]\d{9}$", RegexOptions.IgnoreCase);
        }

        public static bool IsEmail(string val)
        {
            return !string.IsNullOrEmpty(val) && Regex.IsMatch(val, @"^\w+([-_+.]\w+)*@\w+([-_.]\w+)*\.\w+([-_.]\w+)*$", RegexOptions.IgnoreCase);
        }

        public static bool IsNumber(string val)
        {
            return !string.IsNullOrEmpty(val) && Regex.IsMatch(val, "^[0-9]+$");
        }

        public static bool IsDateTime(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;

            const string formatDate = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$";
            const string formatDateTime = @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$";

            return Regex.IsMatch(val, formatDate) || Regex.IsMatch(val, formatDateTime);
        }

        public static string Trim(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.Trim();
        }

        public static string Trim(string text, char trimChar)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.Trim().Trim(trimChar);
        }

        public static string TrimSlash(string text)
        {
            return Trim(text, '/');
        }

        public static string TrimEndSlash(string text)
        {
            if (string.IsNullOrEmpty(text) || text.Length == 1) return text;
            return TrimEnd(text, "/");
        }

        public static string TrimEnd(string text, string end)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (string.IsNullOrEmpty(end)) return text;
            return EndsWithIgnoreCase(text, end) ? text.Substring(0, text.Length - end.Length) : text;
        }

        public static string TrimAndToLower(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.ToLower().Trim();
        }

        public static string ToLower(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.ToLower();
        }

        public static string ToUpper(string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.ToUpper();
        }

        public static string ToCamelCase(string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }
            return str;
        }

        public static string Remove(string text, int startIndex)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (startIndex >= text.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            return text.Substring(0, startIndex);
        }

        public static string Guid()
        {
            return System.Guid.NewGuid().ToString();
        }

        public static string GetShortGuid()
        {
            long i = 1;
            foreach (var b in System.Guid.NewGuid().ToByteArray())
            {
                i *= b + 1;
            }
            return $"{i - DateTime.Now.Ticks:x}";
        }

        public static string GetShortGuid(bool isUppercase)
        {
            long i = 1;
            foreach (var b in System.Guid.NewGuid().ToByteArray())
            {
                i *= b + 1;
            }
            string retVal = $"{i - DateTime.Now.Ticks:x}";
            return isUppercase ? retVal.ToUpper() : retVal.ToLower();
        }

        public static string GetRandomString(int length)
        {
            var str = string.Empty;
            return GetRandomString(str, length);
        }

        private static string GetRandomString(string str, int length)
        {
            str += GetShortGuid();
            if (str.Length >= length)
            {
                return str.Substring(0, length);
            }
            return GetRandomString(str, length);
        }

        public static bool EqualsIgnoreCase(string a, string b)
        {
            if (a == b) return true;
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;

            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Contains(string content, string val)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(val)) return false;
            return content == val || content.Contains(val);
        }

        public static bool ContainsIgnoreCase(string content, string val)
        {
            if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(val)) return false;
            return content == val || content.Contains(val, StringComparison.OrdinalIgnoreCase);
        }

        public static bool Equals(string a, string b)
        {
            return string.IsNullOrEmpty(a) ? string.IsNullOrEmpty(b) : string.Equals(a, b);
        }

        public static bool StartsWithIgnoreCase(string text, string startString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(startString)) return false;
            return text.Trim().ToLower().StartsWith(startString.Trim().ToLower()) || string.Equals(text.Trim(), startString.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(string text, string endString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(endString)) return false;
            return text.Trim().ToLower().EndsWith(endString.Trim().ToLower());
        }

        public static bool StartsWith(string text, string startString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(startString)) return false;
            return text.StartsWith(startString);
        }

        public static bool EndsWith(string text, string endString)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(endString)) return false;
            return text.EndsWith(endString);
        }

        public static void InsertBefore(string[] insertBeforeArray, StringBuilder contentBuilder, string insertContent)
        {
            if (contentBuilder == null) return;
            foreach (var insertBefore in insertBeforeArray)
            {
                if (contentBuilder.ToString().IndexOf(insertBefore, StringComparison.Ordinal) != -1)
                {
                    InsertBefore(insertBefore, contentBuilder, insertContent);
                    return;
                }
            }
        }

        private static void InsertBefore(string insertBefore, StringBuilder contentBuilder, string insertContent)
        {
            if (string.IsNullOrEmpty(insertBefore) || contentBuilder == null) return;
            var startIndex = contentBuilder.ToString().IndexOf(insertBefore, StringComparison.Ordinal);
            if (startIndex != -1)
            {
                contentBuilder.Insert(startIndex, insertContent);
            }
        }

        public static void InsertAfter(string[] insertAfterArray, StringBuilder contentBuilder, string insertContent)
        {
            if (contentBuilder != null)
            {
                foreach (var insertAfter in insertAfterArray)
                {
                    if (contentBuilder.ToString().IndexOf(insertAfter, StringComparison.Ordinal) != -1)
                    {
                        InsertAfter(insertAfter, contentBuilder, insertContent);
                        return;
                    }
                }
            }
        }

        private static void InsertAfter(string insertAfter, StringBuilder contentBuilder, string insertContent)
        {
            if (string.IsNullOrEmpty(insertAfter) || contentBuilder == null) return;
            var startIndex = contentBuilder.ToString().IndexOf(insertAfter, StringComparison.Ordinal);
            if (startIndex == -1) return;
            if (startIndex != -1)
            {
                contentBuilder.Insert(startIndex + insertAfter.Length, insertContent);
            }
        }

        public static string ToXmlContent(string inputString)
        {
            var contentBuilder = new StringBuilder(inputString);
            contentBuilder.Replace("<![CDATA[", string.Empty);
            contentBuilder.Replace("]]>", string.Empty);
            contentBuilder.Insert(0, "<![CDATA[");
            contentBuilder.Append("]]>");
            return contentBuilder.ToString();
        }

        public static string StripTags(string inputString)
        {
            var retVal = RegexUtils.Replace("<script[^>]*>.*?<\\/script>", inputString, string.Empty);
            retVal = RegexUtils.Replace("<[\\/]?[^>]*>|<[\\S]+", retVal, string.Empty);
            return retVal;
        }

        public static string StripTags(string inputString, params string[] tagNames)
        {
            var retVal = inputString;
            foreach (var tagName in tagNames)
            {
                retVal = RegexUtils.Replace($"<[\\/]?{tagName}[^>]*>|<{tagName}", retVal, string.Empty);
            }
            return retVal;
        }

        public static string StripEntities(string inputString)
        {
            var retVal = RegexUtils.Replace("&[^;]*;", inputString, string.Empty);
            return retVal;
        }

        public static string ReplaceIgnoreCase(string value, string replace, string to)
        {
            if (value == null) return string.Empty;
            if (to == null) to = string.Empty;
            var count = 0;
            var position0 = 0;
            int position1;
            var upperString = value.ToUpper();
            var upperPattern = replace.ToUpper();
            var inc = (value.Length / replace.Length) * (to.Length - replace.Length);
            var chars = new char[value.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0, StringComparison.Ordinal)) != -1)
            {
                for (var i = position0; i < position1; ++i) chars[count++] = value[i];
                foreach (var t in to)
                {
                    chars[count++] = t;
                }
                position0 = position1 + replace.Length;
            }
            if (position0 == 0) return value;
            for (var i = position0; i < value.Length; ++i) chars[count++] = value[i];
            return new string(chars, 0, count);
        }

        public static string Replace(string value, string replace, string to)
        {
            if (value == null) return string.Empty;
            if (string.IsNullOrEmpty(replace)) return value;

            if (to == null) to = string.Empty;
            if (replace.StartsWith("/") && replace.EndsWith("/"))
            {
                return RegexUtils.Replace(replace.Trim('/'), value, to);
            }

            return value.Replace(replace, to);
        }

        public static void ReplaceHrefOrSrc(StringBuilder builder, string replace, string to)
        {
            builder.Replace("href=\"" + replace, "href=\"" + to);
            builder.Replace("href='" + replace, "href='" + to);
            builder.Replace("href=" + replace, "href=" + to);
            builder.Replace("href=&quot;" + replace, "href=&quot;" + to);
            builder.Replace("src=\"" + replace, "src=\"" + to);
            builder.Replace("src='" + replace, "src='" + to);
            builder.Replace("src=" + replace, "src=" + to);
            builder.Replace("src=&quot;" + replace, "src=&quot;" + to);
            builder.Replace("playurl=\"" + replace, "playurl=\"" + to);
            builder.Replace("playurl='" + replace, "playurl='" + to);
            builder.Replace("playurl=" + replace, "playurl=" + to);
            builder.Replace("playurl=&quot;" + replace, "playurl=&quot;" + to);
        }

        public static string ReplaceFirstByStartIndex(string input, string replace, string to, int startIndex)
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (input.Length <= startIndex) return input;

            var pre = input.Substring(0, startIndex);
            input = input.Substring(startIndex);
            return pre + ReplaceFirst(input, replace, to);
        }

        private static string ReplaceFirst(string input, string replace, string to)
        {
            var pos = input.IndexOf(replace, StringComparison.Ordinal);
            if (pos > 0)
            {
                //取位置前部分+替换字符串+位置（加上查找字符长度）后部分
                return input.Substring(0, pos) + to + input.Substring(pos + replace.Length);
            }
            if (pos == 0)
            {
                return to + input.Substring(replace.Length);
            }
            return input;
        }

        public static string ReplaceStartsWith(string input, string replace, string to)
        {
            var retVal = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.StartsWith(replace))
            {
                retVal = to + input.Substring(replace.Length);
            }
            return retVal;
        }

        public static string ReplaceStartsWithIgnoreCase(string input, string replace, string to)
        {
            var retVal = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.ToLower().StartsWith(replace.ToLower()))
            {
                retVal = to + input.Substring(replace.Length);
            }
            return retVal;
        }

        public static string ReplaceEndsWith(string input, string replace, string to)
        {
            var retVal = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.EndsWith(replace))
            {
                retVal = input.Substring(0, input.Length - replace.Length) + to;
            }
            return retVal;
        }

        public static string ReplaceEndsWithIgnoreCase(string input, string replace, string to)
        {
            var retVal = input;
            if (!string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(replace) && input.ToLower().EndsWith(replace.ToLower()))
            {
                retVal = input.Substring(0, input.Length - replace.Length) + to;
            }
            return retVal;
        }

        public static string ReplaceNewlineToBr(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\n':
                        retVal.Append("<br />");
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }

        public static string ReplaceNewline(string inputString, string replacement)
        {
            if (string.IsNullOrEmpty(inputString)) return string.Empty;
            var retVal = new StringBuilder();
            inputString = inputString.Trim();
            foreach (var t in inputString)
            {
                switch (t)
                {
                    case '\n':
                        retVal.Append(replacement);
                        break;
                    case '\r':
                        break;
                    default:
                        retVal.Append(t);
                        break;
                }
            }
            return retVal.ToString();
        }

        public static bool IsChinese(char c)
        {
            return 0x4e00 <= c && c <= 0x9fbb;
        }

        /// <summary>
        /// 得到innerText在content中的数目
        /// </summary>
        /// <param name="innerText"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static int GetCount(string innerText, string content)
        {
            if (innerText == null || content == null)
            {
                return 0;
            }
            var count = 0;
            for (var index = content.IndexOf(innerText, StringComparison.Ordinal); index != -1; index = content.IndexOf(innerText, index + innerText.Length, StringComparison.Ordinal))
            {
                count++;
            }
            return count;
        }

        public static int GetStartCount(char startChar, string content)
        {
            if (content == null)
            {
                return 0;
            }
            var count = 0;

            foreach (var theChar in content)
            {
                if (theChar == startChar)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }

        public static int GetStartCount(string startString, string content)
        {
            if (content == null)
            {
                return 0;
            }
            var count = 0;

            while (true)
            {
                if (content.StartsWith(startString))
                {
                    count++;
                    content = content.Remove(0, startString.Length);
                }
                else
                {
                    break;
                }
            }

            return count;
        }

        public static string GetFirstOfStringCollection(string collection, char separator)
        {
            if (!string.IsNullOrEmpty(collection))
            {
                var index = collection.IndexOf(separator);
                return index == -1 ? collection : collection.Substring(0, index);
            }
            return string.Empty;
        }


        private static int _randomSeq;
        public static int GetRandomInt(int minValue, int maxValue)
        {
            var ro = new Random(unchecked((int)DateTime.Now.Ticks));
            var retVal = ro.Next(minValue, maxValue);
            retVal += _randomSeq++;
            if (retVal >= maxValue)
            {
                _randomSeq = 0;
                retVal = minValue;
            }
            return retVal;
        }

        public static string ValueToUrl(string value)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retVal = value.Replace("=", "_equals_").Replace("&", "_and_").Replace("?", "_question_").Replace("'", "_quote_").Replace("+", "_add_").Replace("\r", "").Replace("\n", "");
            }
            return retVal;
        }

        public static string ValueFromUrl(string value)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retVal = value.Replace("_equals_", "=").Replace("_and_", "&").Replace("_question_", "?").Replace("_quote_", "'").Replace("_add_", "+");
            }
            return retVal;
        }

        public static string ToJsString(string value)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retVal = value.Replace("'", @"\'").Replace("\r", "\\r").Replace("\n", "\\n");
            }
            return retVal;
        }

        public static string ToJsonString(string value)
        {
            var retVal = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                retVal = value.Replace("\\", "\\\\");
            }
            return retVal;
        }

        public static string ParseReplace(string parsedContent, string replace, string to)
        {
            if (replace.IndexOf(',') != -1)
            {
                var replaceList = ListUtils.GetStringList(replace);
                var toList = ListUtils.GetStringList(to);

                if (replaceList.Count == toList.Count)
                {
                    for (var i = 0; i < replaceList.Count; i++)
                    {
                        parsedContent = parsedContent.Replace(replaceList[i], toList[i]);
                    }

                    return parsedContent;
                }

                if (toList.Count == 1)
                {
                    foreach (var replaceStr in replaceList)
                    {
                        parsedContent = parsedContent.Replace(replaceStr, to);
                    }

                    return parsedContent;
                }
            }

            string retVal;
            if (replace.StartsWith("/") && replace.EndsWith("/"))
            {
                retVal = RegexUtils.Replace(replace.Trim('/'), parsedContent, to);
            }
            else
            {
                retVal = parsedContent.Replace(replace, to);
            }

            return retVal;
        }

        public static string LowerFirst(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.First().ToString().ToLower() + input.Substring(1);
        }

        public static string UpperFirst(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        public static string HtmlEncode(string inputString)
        {
            return HttpUtility.HtmlEncode(inputString);
        }

        public static string HtmlDecode(string inputString)
        {
            return HttpUtility.HtmlDecode(inputString);
        }

        public static string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData)) return string.Empty;

            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string MaxLengthText(string inputString, int maxLength, string endString = Constants.Ellipsis)
        {
            var retVal = inputString;
            try
            {
                if (maxLength > 0)
                {
                    var decodedInputString = HttpUtility.HtmlDecode(retVal);
                    retVal = decodedInputString;

                    var totalLength = maxLength * 2;
                    var length = 0;
                    var builder = new StringBuilder();

                    var isOneBytesChar = false;
                    var lastChar = ' ';

                    if (!string.IsNullOrEmpty(retVal))
                    {
                        foreach (var singleChar in retVal.ToCharArray())
                        {
                            builder.Append(singleChar);

                            if (IsTwoBytesChar(singleChar))
                            {
                                length += 2;
                                if (length >= totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                            }
                            else
                            {
                                length += 1;
                                if (length == totalLength)
                                {
                                    isOneBytesChar = true;//已经截取到需要的字数，再多截取一位
                                }
                                else if (length > totalLength)
                                {
                                    lastChar = singleChar;
                                    break;
                                }
                                else
                                {
                                    isOneBytesChar = !isOneBytesChar;
                                }
                            }
                        }
                    }
                    if (isOneBytesChar && length > totalLength)
                    {
                        builder.Length--;
                        var theStr = builder.ToString();
                        retVal = builder.ToString();
                        if (char.IsLetter(lastChar))
                        {
                            for (var i = theStr.Length - 1; i > 0; i--)
                            {
                                var theChar = theStr[i];
                                if (!IsTwoBytesChar(theChar) && char.IsLetter(theChar))
                                {
                                    retVal = retVal.Substring(0, i - 1);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            //int index = retVal.LastIndexOfAny(new char[] { ' ', '\t', '\n', '\v', '\f', '\r', '\x0085' });
                            //if (index != -1)
                            //{
                            //    retVal = retVal.Substring(0, index);
                            //}
                        }
                    }
                    else
                    {
                        retVal = builder.ToString();
                    }

                    var isCut = decodedInputString != retVal;
                    retVal = HttpUtility.HtmlEncode(retVal);

                    if (isCut && endString != null)
                    {
                        retVal += endString;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retVal;
        }

        private static bool IsTwoBytesChar(char chr)
        {
            const string pattern = "[\u4e00-\u9fbb]";
            return Regex.IsMatch(chr.ToString(), pattern);
        }

        public const string StrictNameRegex = "^[a-z][a-z0-9\\-_]*$";

        public static bool IsStrictName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            var reg = new Regex(StrictNameRegex, RegexOptions.Singleline);
            return reg.IsMatch(name);
        }

        public static string ParseString(string content, string replace, string to, int startIndex, int length, int wordNum, string ellipsis, bool isClearTags, bool isReturnToBr, bool isLower, bool isUpper, string formatString)
        {
            var parsedContent = content;

            if (!string.IsNullOrEmpty(replace))
            {
                parsedContent = ParseReplace(parsedContent, replace, to);
            }

            if (isClearTags)
            {
                parsedContent = StripTags(parsedContent);
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
                    parsedContent = MaxLengthText(parsedContent, wordNum, ellipsis);
                }

                if (isReturnToBr)
                {
                    parsedContent = ReplaceNewlineToBr(parsedContent);
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

        public static string GetElementId()
        {
            return "e_" + GetShortGuid(false);
        }

        public static string ToString(object value)
        {
            return value?.ToString();
        }
    }
}
