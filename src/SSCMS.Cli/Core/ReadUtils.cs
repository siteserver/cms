using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using SSCMS.Utils;

namespace SSCMS.Cli.Core
{
    public static class ReadUtils
    {
        public static string GetSelect(string text, List<string> options)
        {
            var option = Prompt.GetString($"{text}({ListUtils.ToString(options)}):");
            return ListUtils.ContainsIgnoreCase(options, option) ? option : GetSelect(text, options);
        }

        public static string GetString(string text)
        {
            var value = Prompt.GetString(text);
            return !string.IsNullOrEmpty(value) ? value: GetString(text);
        }

        public static List<string> GetStringList(string text)
        {
            var value = Prompt.GetString(text);
            return ListUtils.GetStringList(value);
        }

        public static string GetPassword(string text)
        {
            var value = Prompt.GetPassword(text);
            return !string.IsNullOrEmpty(value) ? value : GetPassword(text);
        }

        public static int GetInt(string text)
        {
            var value = Prompt.GetInt(text);
            return value > 0 ? value : GetInt(text);
        }

        public static decimal GetDecimal(string text)
        {
            var value = Prompt.GetString(text);
            return TranslateUtils.ToDecimal(value);
        }

        public static bool GetYesNo(string text)
        {
            return Prompt.GetYesNo(text, true);
        }
    }
}
