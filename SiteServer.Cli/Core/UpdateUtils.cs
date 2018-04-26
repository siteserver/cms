using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SiteServer.Utils;

namespace SiteServer.Cli.Core
{
    public static class UpdateUtils
    {
        public static List<Dictionary<string, object>> UpdateRows(List<JObject> oldRows, Dictionary<string, string> convertDict)
        {
            var newRows = new List<Dictionary<string, object>>();

            foreach (var oldRow in oldRows)
            {
                var newRow = TranslateUtils.JsonGetDictionaryIgnorecase(oldRow);
                foreach (var convertKey in convertDict.Keys)
                {
                    newRow[convertKey] = newRow[convertDict[convertKey]];
                }

                newRows.Add(newRow);
            }

            return newRows;
        }
    }
}
