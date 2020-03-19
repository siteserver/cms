using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Model;
using SSCMS.Core.StlParser.Utility;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "翻页栏目列表", Description = "通过 stl:pageChannels 标签在模板中显示翻页栏目列表")]
    public class StlPageChannels : StlChannels
    {
        public new const string ElementName = "stl:pageChannels";

        [StlAttribute(Title = "每页显示的栏目数目")]
        private const string PageNum = nameof(PageNum);

        private IParseManager _parseManager;
        private ListInfo _listInfo;
        private List<KeyValuePair<int, Channel>> _channelList;

        public static async Task<StlPageChannels> GetAsync(string stlPageChannelsElement, IParseManager parseManager)
        {
            var stlPageChannels = new StlPageChannels
            {
                _parseManager = parseManager
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageChannelsElement);

            stlPageChannels._parseManager.ContextInfo = parseManager.ContextInfo.Clone(stlPageChannelsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes);

            stlPageChannels._listInfo = await ListInfo.GetListInfoAsync(parseManager, ParseType.Channel);

            stlPageChannels._channelList = await GetChannelsDataSourceAsync(parseManager, stlPageChannels._listInfo);

            //stlPageChannels.DataSet = await StlDataUtility.GetPageChannelsDataSetAsync(pageInfo.SiteId, channelId, stlPageChannels._listInfo.GroupChannel, stlPageChannels._listInfo.GroupChannelNot, stlPageChannels._listInfo.IsImageExists, stlPageChannels._listInfo.IsImage, stlPageChannels._listInfo.StartNum, stlPageChannels._listInfo.TotalNum, stlPageChannels._listInfo.Order, stlPageChannels._listInfo.Scope, isTotal);

            return stlPageChannels;
        }

        public int GetPageCount(out int totalNum)
        {
            var pageCount = 1;
            totalNum = 0;//数据库中实际的内容数目
            if (_channelList == null || _channelList.Count == 0) return pageCount;

            totalNum = _channelList.Count;
            if (_listInfo.PageNum != 0 && _listInfo.PageNum < totalNum)//需要翻页
            {
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(_listInfo.PageNum)));//需要生成的总页数
            }
            return pageCount;
        }

        public async Task<string> ParseAsync(int currentPageIndex, int pageCount)
        {
            var parsedContent = string.Empty;

            _parseManager.ContextInfo.PageItemIndex = currentPageIndex * _listInfo.PageNum;

            if (_channelList != null && _channelList.Count > 0)
            {
                var pageChannelList = pageCount > 1
                    ? _channelList.Skip(_parseManager.ContextInfo.PageItemIndex).Take(_listInfo.PageNum).ToList()
                    : _channelList;

                parsedContent = await ParseElementAsync(_parseManager, _listInfo, pageChannelList);
            }

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            _parseManager.ContextInfo.PageItemIndex = 0;

            return parsedContent;
        }

        //public int GetPageCount(out int totalNum)
        //{
        //    var pageCount = 1;
        //    totalNum = 0;//数据库中实际的内容数目
        //    if (DataSet == null) return pageCount;

        //    totalNum = DataSet.Tables[0].DefaultView.Count;
        //    if (_listInfo.PageNum != 0 && _listInfo.PageNum < totalNum)//需要翻页
        //    {
        //        pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(_listInfo.PageNum)));//需要生成的总页数
        //    }
        //    return pageCount;
        //}

        //public async Task<string> ParseAsync(int currentPageIndex, int pageCount)
        //{
        //    var parsedContent = string.Empty;

        //    _contextInfo.PageItemIndex = currentPageIndex * _listInfo.PageNum;

        //    try
        //    {
        //        if (DataSet != null)
        //        {
        //            var objPage = new PagedDataSource { DataSource = DataSet.Tables[0].DefaultView }; //分页类

        //            if (pageCount > 1)
        //            {
        //                objPage.AllowPaging = true;
        //                objPage.PageSize = _listInfo.PageNum;//每页显示的项数
        //            }
        //            else
        //            {
        //                objPage.AllowPaging = false;
        //            }

        //            objPage.CurrentPageIndex = currentPageIndex;//当前页的索引


        //            if (_listInfo.Layout == Model.Layout.None)
        //            {
        //                var rptContents = new Repeater
        //                {
        //                    ItemTemplate =
        //                        new RepeaterTemplate(_listInfo.ItemTemplate, _listInfo.SelectedItems,
        //                            _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate,
        //                            _listInfo.SeparatorRepeat, _pageInfo, ContextType.Channel, _contextInfo)
        //                };

        //                if (!string.IsNullOrEmpty(_listInfo.HeaderTemplate))
        //                {
        //                    rptContents.HeaderTemplate = new SeparatorTemplate(_listInfo.HeaderTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(_listInfo.FooterTemplate))
        //                {
        //                    rptContents.FooterTemplate = new SeparatorTemplate(_listInfo.FooterTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(_listInfo.SeparatorTemplate))
        //                {
        //                    rptContents.SeparatorTemplate = new SeparatorTemplate(_listInfo.SeparatorTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(_listInfo.AlternatingItemTemplate))
        //                {
        //                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(_listInfo.AlternatingItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, ContextType.Channel, _contextInfo);
        //                }

        //                rptContents.DataSource = objPage;
        //                rptContents.DataBind();

        //                if (rptContents.Items.Count > 0)
        //                {
        //                    parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
        //                }
        //            }
        //            else
        //            {
        //                var pdlContents = new ParsedDataList();

        //                //设置显示属性
        //                TemplateUtility.PutListInfoToMyDataList(pdlContents, _listInfo);

        //                //设置列表模板
        //                pdlContents.ItemTemplate = new DataListTemplate(_listInfo.ItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, ContextType.Channel, _contextInfo);
        //                if (!string.IsNullOrEmpty(_listInfo.HeaderTemplate))
        //                {
        //                    pdlContents.HeaderTemplate = new SeparatorTemplate(_listInfo.HeaderTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(_listInfo.FooterTemplate))
        //                {
        //                    pdlContents.FooterTemplate = new SeparatorTemplate(_listInfo.FooterTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(_listInfo.SeparatorTemplate))
        //                {
        //                    pdlContents.SeparatorTemplate = new SeparatorTemplate(_listInfo.SeparatorTemplate);
        //                }
        //                if (!string.IsNullOrEmpty(_listInfo.AlternatingItemTemplate))
        //                {
        //                    pdlContents.AlternatingItemTemplate = new DataListTemplate(_listInfo.AlternatingItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, ContextType.Channel, _contextInfo);
        //                }

        //                pdlContents.DataSource = objPage;
        //                pdlContents.DataKeyField = nameof(Channel.Id);
        //                pdlContents.DataBind();

        //                if (pdlContents.Items.Count > 0)
        //                {
        //                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        parsedContent = await LogUtils.AddStlErrorLogAsync(_pageInfo, ElementName, _stlPageChannelsElement, ex);
        //    }

        //    //还原翻页为0，使得其他列表能够正确解析ItemIndex
        //    _contextInfo.PageItemIndex = 0;

        //    return parsedContent;
        //}
    }

}
