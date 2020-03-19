using System.Collections.Generic;
using System.Text;
using Datory.Utils;

namespace SSCMS.Utils
{
    public class CsvUtils
	{
        public static void Export(string filePath, List<string> head, List<List<string>> rows)
        {
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryUtils.GetDirectoryPath(filePath));
            FileUtils.DeleteFileIfExists(filePath);

            var builder = new StringBuilder();
            foreach (var name in head)
            {
                builder.Append(name).Append(",");
            }
            builder.Length -= 1;
            builder.Append("\n");

            foreach (var row in rows)
            {
                foreach (var r in row)
                {
                    var value = r.Replace(@"""", @"""""");
                    builder.Append(@"""" + value + @"""").Append(",");
                }
                builder.Length -= 1;
                builder.Append("\n");
            }

            FileUtils.WriteText(filePath, builder.ToString());
        }

        public static void Import(string filePath, out List<string> head, out List<List<string>> rows)
        {
            head = new List<string>();
            rows = new List<List<string>>();

            var valueList = new List<string>();

            //string content = FileUtils.ReadText(filePath, ECharset.utf_8);
            var content = FileUtils.ReadText(filePath);
            if (!string.IsNullOrEmpty(content))
            {
                valueList = Utilities.GetStringList(content, '\n');
            }

            if (valueList.Count > 1)
            {
                head = Utilities.GetStringList(valueList[0]);
                valueList = valueList.GetRange(1, valueList.Count - 1);
            }

            foreach (var str in valueList)
            {
                var row = new List<string>();

                var value = str.Replace(@"""""", @"""");
                var list = Utilities.GetStringList(value);

                if (list.Count != head.Count) continue;
                foreach (var r in list)
                {
                    row.Add(r.Trim('"'));
                }
                rows.Add(row);
            }
        }
	}
}
