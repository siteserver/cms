using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Mocks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Services;
using SSCMS.Core.StlParser.Enums;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "数据库列表", Description = "通过 stl:sqlContents 标签在模板中显示数据库列表")]
    public class StlSqlContents
    {
        public const string ElementName = "stl:sqlContents";

        [StlAttribute(Title = "数据库类型名称")]
        public const string DatabaseTypeName = nameof(DatabaseTypeName);

        [StlAttribute(Title = "数据库类型")]
        public const string DatabaseType = nameof(DatabaseType);

        [StlAttribute(Title = "数据库链接字符串名称")]
        public const string ConnectionStringName = nameof(ConnectionStringName);
        
        [StlAttribute(Title = "数据库链接字符串")]
        public const string ConnectionString = nameof(ConnectionString);
        
        [StlAttribute(Title = "数据库查询语句")]
        public const string QueryString = nameof(QueryString);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var listInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.SqlContent);

            List<KeyValuePair<int, IDictionary<string, object>>> dataSource = null;
            if (!string.IsNullOrEmpty(listInfo.QueryString))
            {
                dataSource = await parseManager.DatabaseManager.ParserGetSqlDataSourceAsync(listInfo.DatabaseType,
                    listInfo.ConnectionString, listInfo.QueryString);
            }
            else if (listInfo.Query != null)
            {
                dataSource = await parseManager.DatabaseManager.ParserGetSqlDataSourceAsync(listInfo.DatabaseType,
                    listInfo.ConnectionString, listInfo.Query);
            }

            if (dataSource == null) return string.Empty;

            if (parseManager.ContextInfo.IsStlEntity)
            {
                return ParseEntity(dataSource);
            }

            return await ParseElementAsync(parseManager, listInfo, dataSource);
        }

        internal static async Task<string> ParseElementAsync(IParseManager parseManager, ListInfo listInfo, List<KeyValuePair<int, IDictionary<string, object>>> dataSource)
        {
            var pageInfo = parseManager.PageInfo;

            if (dataSource == null || dataSource.Count == 0) return string.Empty;

            var builder = new StringBuilder();
            if (listInfo.Layout == ListLayout.None)
            {
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    builder.Append(listInfo.HeaderTemplate);
                }

                var isAlternative = false;
                var isSeparator = false;
                var isSeparatorRepeat = false;
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    isAlternative = true;
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    isSeparator = true;
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorRepeatTemplate))
                {
                    isSeparatorRepeat = true;
                }

                for (var i = 0; i < dataSource.Count; i++)
                {
                    var dict = dataSource[i];

                    pageInfo.SqlItems.Push(dict);
                    var templateString = isAlternative && i % 2 == 1 ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(await TemplateUtility.GetSqlContentsTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.SqlContent));

                    if (isSeparator && i != dataSource.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    if (isSeparatorRepeat && (i + 1) % listInfo.SeparatorRepeat == 0 && i != dataSource.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorRepeatTemplate);
                    }
                }

                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    builder.Append(listInfo.FooterTemplate);
                }
            }
            else
            {
                var isAlternative = !string.IsNullOrEmpty(listInfo.AlternatingItemTemplate);

                var tableAttributes = listInfo.GetTableAttributes();
                var cellAttributes = listInfo.GetCellAttributes();

                using var table = new HtmlTable(builder, tableAttributes);

                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    table.StartHead();
                    using (var tHead = table.AddRow())
                    {
                        tHead.AddCell(listInfo.HeaderTemplate, cellAttributes);
                    }
                    table.EndHead();
                }

                table.StartBody();

                var columns = listInfo.Columns <= 1 ? 1 : listInfo.Columns;
                var itemIndex = 0;

                while (true)
                {
                    using var tr = table.AddRow();
                    for (var cell = 1; cell <= columns; cell++)
                    {
                        var cellHtml = string.Empty;
                        if (itemIndex < dataSource.Count)
                        {
                            var dict = dataSource[itemIndex];

                            pageInfo.SqlItems.Push(dict);
                            var templateString = isAlternative && itemIndex % 2 == 1
                                ? listInfo.AlternatingItemTemplate
                                : listInfo.ItemTemplate;
                            cellHtml = await TemplateUtility.GetSqlContentsTemplateStringAsync(templateString,
                                listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager,
                                ParseType.SqlContent);
                        }

                        tr.AddCell(cellHtml, cellAttributes);
                        itemIndex++;
                    }

                    if (itemIndex >= dataSource.Count) break;
                }

                table.EndBody();

                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    table.StartFoot();
                    using (var tFoot = table.AddRow())
                    {
                        tFoot.AddCell(listInfo.FooterTemplate, cellAttributes);
                    }
                    table.EndFoot();
                }
            }

            return builder.ToString();
        }

        private static object ParseEntity(IEnumerable<KeyValuePair<int, IDictionary<string, object>>> dataSource)
        {
            var list = dataSource.Select(x => x.Value).ToList();
            return list;
        }
    }
}
