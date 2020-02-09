using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Datory.Utils;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "列表项循环", Description = "通过 stl:each 标签在模板中遍历指定的列表项")]
    public class StlEach
    {
        public const string ElementName = "stl:each";

        [StlAttribute(Title = "循环类型")]
        private const string Type = nameof(Type);

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {ContentAttribute.ImageUrl, "遍历内容的图片字段"},
            {ContentAttribute.VideoUrl, "遍历内容的视频字段"},
            {ContentAttribute.FileUrl, "遍历内容的附件字段"}
        };

        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var listInfo = await ListInfo.GetListInfoAsync(pageInfo, contextInfo, EContextType.Content);

            return await ParseImplAsync(pageInfo, contextInfo, listInfo);
        }

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, ListInfo listInfo)
        {
            var parsedContent = string.Empty;

            var type = listInfo.Others.Get(Type);
            if (string.IsNullOrEmpty(type))
            {
                type = ContentAttribute.ImageUrl;
            }

            var contextType = EContextType.Each;
            IEnumerable dataSource = null;
            var content = await contextInfo.GetContentAsync();
            if (content != null)
            {
                var eachList = new List<string>();

                if (!string.IsNullOrEmpty(content.Get<string>(type)))
                {
                    eachList.Add(content.Get<string>(type));
                }

                var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                var extendValues = content.Get<string>(extendAttributeName);
                if (!string.IsNullOrEmpty(extendValues))
                {
                    foreach (var extendValue in Utilities.GetStringList(extendValues))
                    {
                        eachList.Add(extendValue);
                    }
                }

                if (listInfo.StartNum > 1 || listInfo.TotalNum > 0)
                {
                    if (listInfo.StartNum > 1)
                    {
                        var count = listInfo.StartNum - 1;
                        if (count > eachList.Count)
                        {
                            count = eachList.Count;
                        }
                        eachList.RemoveRange(0, count);
                    }

                    if (listInfo.TotalNum > 0)
                    {
                        if (listInfo.TotalNum < eachList.Count)
                        {
                            eachList.RemoveRange(listInfo.TotalNum, eachList.Count - listInfo.TotalNum);
                        }
                    }
                }

                dataSource = eachList;
            }

            if (listInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater
                {
                    ItemTemplate =
                        new RepeaterTemplate(listInfo.ItemTemplate, listInfo.SelectedItems,
                            listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat,
                            pageInfo, contextType, contextInfo)
                };

                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    rptContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    rptContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    rptContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                }

                rptContents.DataSource = dataSource;

                rptContents.DataBind();

                if (rptContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                }
            }
            else
            {
                var pdlContents = new ParsedDataList();

                TemplateUtility.PutListInfoToMyDataList(pdlContents, listInfo);

                pdlContents.ItemTemplate = new DataListTemplate(listInfo.ItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                if (!string.IsNullOrEmpty(listInfo.HeaderTemplate))
                {
                    pdlContents.HeaderTemplate = new SeparatorTemplate(listInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.FooterTemplate))
                {
                    pdlContents.FooterTemplate = new SeparatorTemplate(listInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.SeparatorTemplate))
                {
                    pdlContents.SeparatorTemplate = new SeparatorTemplate(listInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(listInfo.AlternatingItemTemplate))
                {
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(listInfo.AlternatingItemTemplate, listInfo.SelectedItems, listInfo.SelectedValues, listInfo.SeparatorRepeatTemplate, listInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                }

                pdlContents.DataSource = dataSource;

                pdlContents.DataBind();

                if (pdlContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                }
            }

            return parsedContent;
        }
    }
}
