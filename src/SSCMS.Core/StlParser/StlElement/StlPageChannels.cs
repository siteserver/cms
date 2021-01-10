using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Models;
using SSCMS.Services;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "翻页栏目列表", Description = "通过 stl:pageChannels 标签在模板中显示翻页栏目列表")]
    public class StlPageChannels : StlChannels
    {
        public new const string ElementName = "stl:pageChannels";

        private IParseManager _parseManager;
        private ListInfo _listInfo;
        private List<KeyValuePair<int, Channel>> _channelList;

        public static async Task<StlPageChannels> GetAsync(string stlPageChannelsElement, IParseManager parseManager)
        {
            var stlPageChannels = new StlPageChannels
            {
                _parseManager = parseManager
            };

            var stlElementInfo = StlParserUtility.ParseStlElement(stlPageChannelsElement, -1);

            stlPageChannels._parseManager.ContextInfo = parseManager.ContextInfo.Clone(ElementName, stlPageChannelsElement, stlElementInfo.InnerHtml, stlElementInfo.Attributes, stlElementInfo.StartIndex);

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

                parsedContent = await ParseAsync(_parseManager, _listInfo, pageChannelList);
            }

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            _parseManager.ContextInfo.PageItemIndex = 0;

            return parsedContent;
        }
    }

}
