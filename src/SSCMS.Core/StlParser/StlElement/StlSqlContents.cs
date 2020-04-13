using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Mock;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Services;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "数据库列表", Description = "通过 stl:sqlContents 标签在模板中显示数据库列表")]
    public class StlSqlContents
    {
        public const string ElementName = "stl:sqlContents";

        [StlAttribute(Title = "数据库链接字符串名称")]
        public const string ConnectionStringName = nameof(ConnectionStringName);
        
        [StlAttribute(Title = "数据库链接字符串")]
        public const string ConnectionString = nameof(ConnectionString);
        
        [StlAttribute(Title = "数据库查询语句")]
        public const string QueryString = nameof(QueryString);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var listInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.SqlContent);
            //var dataSource = StlDataUtility.GetSqlContentsDataSource(listInfo.ConnectionString, listInfo.QueryString, listInfo.StartNum, listInfo.TotalNum, listInfo.Order);
            var dataSource = GetDataSource(parseManager, listInfo.ConnectionString, listInfo.QueryString, listInfo.StartNum,
                listInfo.TotalNum, listInfo.Order);

            if (parseManager.ContextInfo.IsStlEntity)
            {
                return ParseEntity(dataSource);
            }

            return await ParseElementAsync(parseManager, listInfo, dataSource);
        }

        public static List<KeyValuePair<int, Dictionary<string, object>>> GetDataSource(IParseManager parseManager, string connectionString, string queryString, int startNum, int totalNum, string order)
        {
            //var sqlString = CacheManager.GetSelectSqlStringByQueryString(connectionString, queryString, startNum, totalNum, order);
            return parseManager.DatabaseManager.ParserGetSqlDataSource(connectionString, queryString);
        }

        protected static async Task<string> ParseElementAsync(IParseManager parseManager, ListInfo listInfo, List<KeyValuePair<int, Dictionary<string, object>>> dataSource)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (dataSource == null || dataSource.Count == 0) return string.Empty;

            var builder = new StringBuilder();
            if (listInfo.Layout == Layout.None)
            {
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    builder.Append(listInfo.HeaderTemplate);
                }

                var isAlternative = false;
                var isSeparator = false;
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    isAlternative = true;
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    isSeparator = true;
                }

                for (var i = 0; i < dataSource.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != dataSource.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var dict = dataSource[i];

                    pageInfo.SqlItems.Push(dict);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(await TemplateUtility.GetSqlContentsTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.SqlContent));
                }

                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    builder.Append(listInfo.FooterTemplate);
                }
            }
            else
            {
                bool isAlternative = !string.IsNullOrEmpty(listInfo.AlternatingItemTemplate);

                var tableAttributes = listInfo.GetTableAttributes();
                var cellAttributes = listInfo.GetCellAttributes();

                using (var table = new HtmlTable(builder, tableAttributes))
                {
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
                        using (var tr = table.AddRow(null))
                        {
                            for (var cell = 1; cell <= columns; cell++)
                            {
                                var cellHtml = string.Empty;
                                if (itemIndex < dataSource.Count)
                                {
                                    var dict = dataSource[itemIndex];

                                    pageInfo.SqlItems.Push(dict);
                                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                                    cellHtml = await TemplateUtility.GetSqlContentsTemplateStringAsync(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, parseManager, ParseType.SqlContent);
                                }
                                tr.AddCell(cellHtml, cellAttributes);
                                itemIndex++;
                            }
                            if (itemIndex >= dataSource.Count) break;
                        }
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
            }

            return builder.ToString();
        }

        private static object ParseEntity(List<KeyValuePair<int, Dictionary<string, object>>> dataSource)
        {
            var list = dataSource.Select(x => x.Value).ToList();
            return list;
        }
    }
}
