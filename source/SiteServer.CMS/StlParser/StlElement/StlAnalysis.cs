using System;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "显示浏览量", Description = "通过 stl:analysis 标签在模板中显示页面的浏览量及点击次数、点击率")]
    public class StlAnalysis
	{
        private StlAnalysis() { }
        public const string ElementName = "stl:analysis";

        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
		public const string AttributeType = "type";
        public const string AttributeScope = "scope";
        public const string AttributeIsAverage = "isAverage";
        public const string AttributeStyle = "style";
        public const string AttributeAddNum = "addNum";
        public const string AttributeSince = "since";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeType, StringUtils.SortedListToAttributeValueString("统计类型", TypeList)},
            {AttributeScope, StringUtils.SortedListToAttributeValueString("统计范围", ScopeList)},
            {AttributeIsAverage, "是否显示日均量"},
            {AttributeStyle, "显示样式"},
            {AttributeAddNum, "添加数目"},
            {AttributeSince, "时间段"}
        };

        public static readonly string TypePageView = "PageView";
        public static readonly string TypeUniqueVisitor = "UniqueVisitor";
        public static readonly string TypeCurrentVisitor = "CurrentVisitor";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypePageView, "显示访问量（PV）"},
            {TypeUniqueVisitor, "显示独立访客量（UV）"},
            {TypeCurrentVisitor, "显示当前人数"}
        };

        public static readonly string ScopeSite = "Site";
        public static readonly string ScopePage = "Page";

        public static SortedList<string, string> ScopeList => new SortedList<string, string>
        {
            {ScopeSite, "统计全站"},
            {ScopePage, "统计页面"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var isGetUrlFromAttribute = false;
            var channelIndex = string.Empty;
            var channelName = string.Empty;

            var type = TypePageView;
            var scope = ScopePage;
            var isAverage = false;
            var style = string.Empty;
            var addNum = 0;
            var since = string.Empty;

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        isGetUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        isGetUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeScope))
                {
                    scope = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsAverage))
                {
                    isAverage = TranslateUtils.ToBool(value, false);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeStyle))
                {
                    style = ETrackerStyleUtils.GetValue(ETrackerStyleUtils.GetEnumType(value));
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeAddNum))
                {
                    addNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSince))
                {
                    since = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, isGetUrlFromAttribute, channelIndex, channelName, type, scope, isAverage, style, addNum, since);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, bool isGetUrlFromAttribute, string channelIndex, string channelName , string type, string scope, bool isAverage, string style, int addNum, string since)
        {
            var channelId = StlDataUtility.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelId, channelIndex, channelName);

            var contentId = 0;
            //判断是否链接地址由标签属性获得
            if (isGetUrlFromAttribute == false)
            {
                contentId = contextInfo.ContentId;
            }

            var templateType = pageInfo.TemplateInfo.TemplateType;
            if (contextInfo.ChannelId != 0)
            {
                templateType = ETemplateType.ChannelTemplate;
                if (contentId != 0)
                {
                    templateType = ETemplateType.ContentTemplate;
                }
            }

            return GetAnalysisValue(pageInfo, contextInfo, channelId, contentId, ETemplateTypeUtils.GetValue(templateType), type, scope, isAverage, style, addNum, since, string.Empty);
        }

        private static string GetAnalysisValue(PageInfo pageInfo, ContextInfo contextInfo, int channelId, int contentId, string templateType, string type, string scope, bool isAverage, string style, int addNum, string since, string referrer)
        {
            var publishmentSystemInfo = pageInfo.PublishmentSystemInfo;
            if (publishmentSystemInfo == null) return string.Empty;

            string html;

            var eStyle = ETrackerStyle.Number;
            if (!string.IsNullOrEmpty(style))
            {
                eStyle = ETrackerStyleUtils.GetEnumType(style);
            }
            if (string.IsNullOrEmpty(scope) || !StringUtils.EqualsIgnoreCase(scope, ScopeSite))
            {
                scope = ScopePage;
            }
            var eTemplateType = ETemplateTypeUtils.GetEnumType(templateType);

            var accessNum = 0;
            var sinceDate = DateUtils.SqlMinValue;
            if (!string.IsNullOrEmpty(since))
            {
                sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
            }

            if (StringUtils.EqualsIgnoreCase(type, TypePageView))
            {
                if (StringUtils.EqualsIgnoreCase(scope, ScopePage))
                {
                    //accessNum = eTemplateType != ETemplateType.FileTemplate ? DataProvider.TrackingDao.GetTotalAccessNumByPageInfo(pageInfo.PublishmentSystemId, channelId, contentId, sinceDate) : DataProvider.TrackingDao.GetTotalAccessNumByPageUrl(pageInfo.PublishmentSystemId, referrer, sinceDate);
                    accessNum = eTemplateType != ETemplateType.FileTemplate ? Tracking.GetTotalAccessNumByPageInfo(pageInfo.PublishmentSystemId, channelId, contentId, sinceDate) : Tracking.GetTotalAccessNumByPageUrl(pageInfo.PublishmentSystemId, referrer, sinceDate);
                }
                else
                {
                    //accessNum = DataProvider.TrackingDao.GetTotalAccessNum(pageInfo.PublishmentSystemId, sinceDate);
                    accessNum = Tracking.GetTotalAccessNum(pageInfo.PublishmentSystemId, sinceDate);
                    accessNum = accessNum + publishmentSystemInfo.Additional.TrackerPageView;
                }
                if (isAverage)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemId);
                    var timeSpan = new TimeSpan(DateTime.Now.Ticks - nodeInfo.AddDate.Ticks);
                    if (!string.IsNullOrEmpty(since))
                    {
                        timeSpan = new TimeSpan(DateTime.Now.Ticks - sinceDate.Ticks);
                    }
                    var trackerDays = timeSpan.Days == 0 ? 1 : timeSpan.Days;//总统计天数
                    trackerDays = trackerDays + publishmentSystemInfo.Additional.TrackerDays;
                    accessNum = Convert.ToInt32(Math.Round(Convert.ToDouble(accessNum / trackerDays)));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeUniqueVisitor))
            {
                if (StringUtils.EqualsIgnoreCase(scope, ScopePage))
                {
                    //accessNum = eTemplateType != ETemplateType.FileTemplate ? DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageInfo(pageInfo.PublishmentSystemId, channelId, contentId, sinceDate) : DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageUrl(pageInfo.PublishmentSystemId, referrer, sinceDate);
                    accessNum = eTemplateType != ETemplateType.FileTemplate ? Tracking.GetTotalUniqueAccessNumByPageInfo(pageInfo.PublishmentSystemId, channelId, contentId, sinceDate) : Tracking.GetTotalUniqueAccessNumByPageUrl(pageInfo.PublishmentSystemId, referrer, sinceDate);
                }
                else
                {
                    //accessNum = DataProvider.TrackingDao.GetTotalUniqueAccessNum(pageInfo.PublishmentSystemId, sinceDate);
                    accessNum = Tracking.GetTotalUniqueAccessNum(pageInfo.PublishmentSystemId, sinceDate);
                    accessNum = accessNum + publishmentSystemInfo.Additional.TrackerUniqueVisitor;
                }
                if (isAverage)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemId);
                    var timeSpan = new TimeSpan(DateTime.Now.Ticks - nodeInfo.AddDate.Ticks);
                    var trackerDays = timeSpan.Days == 0 ? 1 : timeSpan.Days;//总统计天数
                    trackerDays = trackerDays + publishmentSystemInfo.Additional.TrackerDays;
                    accessNum = Convert.ToInt32(Math.Round(Convert.ToDouble(accessNum / trackerDays)));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeCurrentVisitor))
            {
                //accessNum = DataProvider.TrackingDao.GetCurrentVisitorNum(pageInfo.PublishmentSystemId, publishmentSystemInfo.Additional.TrackerCurrentMinute);
                accessNum = Tracking.GetCurrentVisitorNum(pageInfo.PublishmentSystemId, publishmentSystemInfo.Additional.TrackerCurrentMinute);
            }

            accessNum = accessNum + addNum;
            if (accessNum == 0) accessNum = 1;

            // 如果是实体标签，则只返回数字
            if (contextInfo.IsCurlyBrace)
            {
                return accessNum.ToString();
            }
            else
            {
                if (eStyle == ETrackerStyle.Number)
                {
                    html = accessNum.ToString();
                }
                else
                {
                    var numString = accessNum.ToString();
                    var htmlBuilder = new StringBuilder();
                    string imgFolder = $"{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Tracker.DirectoryName)}/{ETrackerStyleUtils.GetValue(eStyle)}";
                    foreach (var t in numString)
                    {
                        string imgHtml = $"<img src='{imgFolder}/{t}.gif' align=absmiddle border=0>";
                        htmlBuilder.Append(imgHtml);
                    }
                    html = htmlBuilder.ToString();
                }

                return html;
            }
        }
	}
}
