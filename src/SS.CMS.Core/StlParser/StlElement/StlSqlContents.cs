using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Template;
using SS.CMS.Core.StlParser.Utility;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "数据库列表", Description = "通过 stl:sqlContents 标签在模板中显示数据库列表")]
    public class StlSqlContents
    {
        public const string ElementName = "stl:sqlContents";

        [StlAttribute(Title = "数据库名称")]
        public const string ConfigName = nameof(ConfigName);

        [StlAttribute(Title = "数据库类型")]
        public const string DatabaseType = nameof(DatabaseType);

        [StlAttribute(Title = "数据库链接字符串")]
        public const string ConnectionString = nameof(ConnectionString);

        [StlAttribute(Title = "数据库查询语句")]
        public const string QueryString = nameof(QueryString);

        public static object Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = ListInfo.GetListInfo(pageInfo, contextInfo, EContextType.SqlContent);
            // var dataSource = StlDataUtility.GetSqlContentsDataSource(listInfo.ConnectionString, listInfo.QueryString, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString);
            var sqlList = StlDataUtility.GetContainerSqlList(listInfo.ConnectionString, listInfo.QueryString, listInfo.StartNum, listInfo.TotalNum, listInfo.OrderByString);

            if (contextInfo.IsStlEntity)
            {
                return ParseEntity(sqlList);
            }

            return ParseElement(pageInfo, contextInfo, listInfo, sqlList);
        }

        public static string ParseElement(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo, List<Container.Sql> sqlList)
        {
            if (sqlList == null || sqlList.Count == 0) return string.Empty;

            var builder = new StringBuilder();

            if (listInfo.Layout == ELayout.None)
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

                for (var i = 0; i < sqlList.Count; i++)
                {
                    if (isSeparator && i % 2 != 0 && i != sqlList.Count - 1)
                    {
                        builder.Append(listInfo.SeparatorTemplate);
                    }

                    var sql = sqlList[i];

                    pageInfo.SqlItems.Push(sql);
                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                    builder.Append(TemplateUtility.GetSqlContentsTemplateString(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, EContextType.SqlContent, contextInfo));
                }

                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    builder.Append(listInfo.FooterTemplate);
                }
            }
            else
            {
                var isAlternative = false;
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    isAlternative = true;
                }

                var tableAttributes = listInfo.GetTableAttributes();
                var cellAttributes = listInfo.GetCellAttributes();

                using (Html.Table table = new Html.Table(builder, tableAttributes))
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
                                if (itemIndex < sqlList.Count)
                                {
                                    var sql = sqlList[itemIndex];

                                    pageInfo.SqlItems.Push(sql);
                                    var templateString = isAlternative ? listInfo.AlternatingItemTemplate : listInfo.ItemTemplate;
                                    cellHtml = TemplateUtility.GetSqlContentsTemplateString(templateString, listInfo.SelectedItems, listInfo.SelectedValues, string.Empty, pageInfo, EContextType.SqlContent, contextInfo);
                                }
                                tr.AddCell(cellHtml, cellAttributes);
                                itemIndex++;
                            }
                            if (itemIndex >= sqlList.Count) break;
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

            // var parsedContent = string.Empty;

            // if (listInfo.Layout == ELayout.None)
            // {
            //     var rptContents = new Repeater
            //     {
            //         ItemTemplate =
            //             new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues,
            //                 listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo,
            //                 EContextType.SqlContent, contextInfo)
            //     };

            //     if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
            //     {
            //         rptContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
            //     {
            //         rptContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
            //     {
            //         rptContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
            //     {
            //         rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.SqlContent, contextInfo);
            //     }

            //     rptContents.DataSource = dataSource;
            //     rptContents.DataBind();

            //     if (rptContents.Items.Count > 0)
            //     {
            //         parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
            //     }
            // }
            // else
            // {
            //     var pdlContents = new ParsedDataList();

            //     TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

            //     pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.SqlContent, contextInfo);
            //     if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
            //     {
            //         pdlContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
            //     {
            //         pdlContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
            //     {
            //         pdlContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
            //     }
            //     if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
            //     {
            //         pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, EContextType.SqlContent, contextInfo);
            //     }

            //     pdlContents.DataSource = dataSource;
            //     pdlContents.DataBind();

            //     if (pdlContents.Items.Count > 0)
            //     {
            //         parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
            //     }
            // }

            // return parsedContent;
        }

        private static object ParseEntity(List<Container.Sql> sqlList)
        {
            // var table = dataSource.Tables[0];
            // return TranslateUtils.DataTableToDictionaryList(table);

            var dictList = new List<Dictionary<string, object>>();
            foreach (var sql in sqlList)
            {
                dictList.Add(sql.Dictionary);
            }
            return dictList;
        }
    }
}
