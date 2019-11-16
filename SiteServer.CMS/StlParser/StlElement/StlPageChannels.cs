using System;
using System.Data;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "翻页栏目列表", Description = "通过 stl:pageChannels 标签在模板中显示翻页栏目列表")]
    public class StlPageChannels : StlChannels
    {
        public new const string ElementName = "stl:pageChannels";

        [StlAttribute(Title = "每页显示的栏目数目")]
        private const string PageNum = nameof(PageNum);

        private string StlPageChannelsElement { get; set; }
        private PageInfo PageInfo { get; set; }
        private ContextInfo ContextInfo { get; set; }
        private DataSet DataSet { get; set; }

        public static async Task<StlPageChannels> GetAsync(string stlPageChannelsElement, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var stlPageChannels = new StlPageChannels
            {
                StlPageChannelsElement = stlPageChannelsElement, 
                PageInfo = pageInfo
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageChannelsElement);

            stlPageChannels.ContextInfo = contextInfo.Clone(stlPageChannelsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            stlPageChannels.DisplayInfo = await ListInfo.GetListInfoAsync(pageInfo, stlPageChannels.ContextInfo, EContextType.Channel);

            var channelId = await StlDataUtility.GetChannelIdByLevelAsync(pageInfo.SiteId, stlPageChannels.ContextInfo.ChannelId, stlPageChannels.DisplayInfo.UpLevel, stlPageChannels.DisplayInfo.TopLevel);

            channelId = await StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, stlPageChannels.DisplayInfo.ChannelIndex, stlPageChannels.DisplayInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(stlPageChannels.DisplayInfo.Others.Get(IsTotal));

            if (TranslateUtils.ToBool(stlPageChannels.DisplayInfo.Others.Get(IsAllChildren)))
            {
                stlPageChannels.DisplayInfo.Scope = EScopeType.Descendant;
            }

            stlPageChannels.DataSet = await StlDataUtility.GetPageChannelsDataSetAsync(pageInfo.SiteId, channelId, stlPageChannels.DisplayInfo.GroupChannel, stlPageChannels.DisplayInfo.GroupChannelNot, stlPageChannels.DisplayInfo.IsImageExists, stlPageChannels.DisplayInfo.IsImage, stlPageChannels.DisplayInfo.StartNum, stlPageChannels.DisplayInfo.TotalNum, stlPageChannels.DisplayInfo.OrderByString, stlPageChannels.DisplayInfo.Scope, isTotal, stlPageChannels.DisplayInfo.Where);

            return stlPageChannels;
        }


        public int GetPageCount(out int totalNum)
        {
            var pageCount = 1;
            totalNum = 0;//数据库中实际的内容数目
            if (DataSet == null) return pageCount;

            totalNum = DataSet.Tables[0].DefaultView.Count;
            if (DisplayInfo.PageNum != 0 && DisplayInfo.PageNum < totalNum)//需要翻页
            {
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(DisplayInfo.PageNum)));//需要生成的总页数
            }
            return pageCount;
        }

        public ListInfo DisplayInfo { get; private set; }

        public async Task<string> ParseAsync(int currentPageIndex, int pageCount)
        {
            var parsedContent = string.Empty;

            ContextInfo.PageItemIndex = currentPageIndex * DisplayInfo.PageNum;

            try
            {
                if (DataSet != null)
                {
                    var objPage = new PagedDataSource { DataSource = DataSet.Tables[0].DefaultView }; //分页类

                    if (pageCount > 1)
                    {
                        objPage.AllowPaging = true;
                        objPage.PageSize = DisplayInfo.PageNum;//每页显示的项数
                    }
                    else
                    {
                        objPage.AllowPaging = false;
                    }

                    objPage.CurrentPageIndex = currentPageIndex;//当前页的索引


                    if (DisplayInfo.Layout == ELayout.None)
                    {
                        var rptContents = new Repeater
                        {
                            ItemTemplate =
                                new RepeaterTemplate(DisplayInfo.ItemTemplate, DisplayInfo.SelectedItems,
                                    DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate,
                                    DisplayInfo.SeparatorRepeat, PageInfo, EContextType.Channel, ContextInfo)
                        };

                        if (!string.IsNullOrEmpty(DisplayInfo.HeaderTemplate))
                        {
                            rptContents.HeaderTemplate = new SeparatorTemplate(DisplayInfo.HeaderTemplate);
                        }
                        if (!string.IsNullOrEmpty(DisplayInfo.FooterTemplate))
                        {
                            rptContents.FooterTemplate = new SeparatorTemplate(DisplayInfo.FooterTemplate);
                        }
                        if (!string.IsNullOrEmpty(DisplayInfo.SeparatorTemplate))
                        {
                            rptContents.SeparatorTemplate = new SeparatorTemplate(DisplayInfo.SeparatorTemplate);
                        }
                        if (!string.IsNullOrEmpty(DisplayInfo.AlternatingItemTemplate))
                        {
                            rptContents.AlternatingItemTemplate = new RepeaterTemplate(DisplayInfo.AlternatingItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, PageInfo, EContextType.Channel, ContextInfo);
                        }

                        rptContents.DataSource = objPage;
                        rptContents.DataBind();

                        if (rptContents.Items.Count > 0)
                        {
                            parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                        }
                    }
                    else
                    {
                        var pdlContents = new ParsedDataList();

                        //设置显示属性
                        TemplateUtility.PutListInfoToMyDataList(pdlContents, DisplayInfo);

                        //设置列表模板
                        pdlContents.ItemTemplate = new DataListTemplate(DisplayInfo.ItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, PageInfo, EContextType.Channel, ContextInfo);
                        if (!string.IsNullOrEmpty(DisplayInfo.HeaderTemplate))
                        {
                            pdlContents.HeaderTemplate = new SeparatorTemplate(DisplayInfo.HeaderTemplate);
                        }
                        if (!string.IsNullOrEmpty(DisplayInfo.FooterTemplate))
                        {
                            pdlContents.FooterTemplate = new SeparatorTemplate(DisplayInfo.FooterTemplate);
                        }
                        if (!string.IsNullOrEmpty(DisplayInfo.SeparatorTemplate))
                        {
                            pdlContents.SeparatorTemplate = new SeparatorTemplate(DisplayInfo.SeparatorTemplate);
                        }
                        if (!string.IsNullOrEmpty(DisplayInfo.AlternatingItemTemplate))
                        {
                            pdlContents.AlternatingItemTemplate = new DataListTemplate(DisplayInfo.AlternatingItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, PageInfo, EContextType.Channel, ContextInfo);
                        }

                        pdlContents.DataSource = objPage;
                        pdlContents.DataKeyField = nameof(Channel.Id);
                        pdlContents.DataBind();

                        if (pdlContents.Items.Count > 0)
                        {
                            parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                parsedContent = await LogUtils.AddStlErrorLogAsync(PageInfo, ElementName, StlPageChannelsElement, ex);
            }

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            ContextInfo.PageItemIndex = 0;

            return parsedContent;
        }
    }

}
