using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlAnalysis
	{
        private StlAnalysis() { }
        public const string ElementName = "stl:analysis";//显示浏览量

        public const string AttributeChannelIndex = "channelindex";			    //栏目索引
        public const string AttributeChannelName = "channelname";				//栏目名称
		public const string AttributeType = "type";		                        //显示类型
        public const string AttributeScope = "scope";		                    //统计范围
        public const string AttributeIsAverage = "isaverage";		            //是否显示日均量
        public const string AttributeStyle = "style";		                    //显示样式
        public const string AttributeAddNum = "addnum";		                    //添加数目
        public const string AttributeSince = "since";				            //时间段
        public const string AttributeIsDynamic = "isdynamic";                   //是否动态显示

        public static readonly string TypePageView = "PageView";               //显示访问量（PV）
        public static readonly string TypeUniqueVisitor = "UniqueVisitor";     //显示独立访客量（UV）
        public static readonly string TypeCurrentVisitor = "CurrentVisitor";   //显示当前人数

        public static readonly string ScopeSite = "Site";                      //统计全站
        public static readonly string ScopePage = "Page";                      //统计页面

	    public static ListDictionary AttributeList => new ListDictionary
	    {
	        {AttributeChannelIndex, "栏目索引"},
	        {AttributeChannelName, "栏目名称"},
	        {AttributeType, "统计类型"},
	        {AttributeScope, "统计范围"},
	        {AttributeIsAverage, "是否显示日均量"},
	        {AttributeStyle, "显示样式"},
	        {AttributeAddNum, "添加数目"},
	        {AttributeSince, "时间段"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var ie = node.Attributes?.GetEnumerator();

                var isGetUrlFromAttribute = false;
                var channelIndex = string.Empty;
                var channelName = string.Empty;

                var type = TypePageView;
                var scope = ScopePage;
                var isAverage = false;
                var style = string.Empty;
                var addNum = 0;
                var since = string.Empty;
                var isDynamic = false;

                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(AttributeChannelIndex))
                        {
                            channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            if (!string.IsNullOrEmpty(channelIndex))
                            {
                                isGetUrlFromAttribute = true;
                            }
                        }
                        else if (attributeName.Equals(AttributeChannelName))
                        {
                            channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            if (!string.IsNullOrEmpty(channelName))
                            {
                                isGetUrlFromAttribute = true;
                            }
                        }
                        else if (attributeName.Equals(AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeScope))
                        {
                            scope = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeIsAverage))
                        {
                            isAverage = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else if (attributeName.Equals(AttributeStyle))
                        {
                            style = ETrackerStyleUtils.GetValue(ETrackerStyleUtils.GetEnumType(attr.Value));
                        }
                        else if (attributeName.Equals(AttributeAddNum))
                        {
                            addNum = TranslateUtils.ToInt(attr.Value);
                        }
                        else if (attributeName.Equals(AttributeSince))
                        {
                            since = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, isGetUrlFromAttribute, channelIndex, channelName, type, scope, isAverage, style, addNum, since);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, bool isGetUrlFromAttribute, string channelIndex, string channelName , string type, string scope, bool isAverage, string style, int addNum, string since)
        {
            var channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelID, channelIndex, channelName);

            var contentId = 0;
            //判断是否链接地址由标签属性获得
            if (isGetUrlFromAttribute == false)
            {
                contentId = contextInfo.ContentID;
            }

            var templateType = pageInfo.TemplateInfo.TemplateType;
            if (contextInfo.ChannelID != 0)
            {
                templateType = ETemplateType.ChannelTemplate;
                if (contentId != 0)
                {
                    templateType = ETemplateType.ContentTemplate;
                }
            }

            return GetAnalysisValue(pageInfo, channelId, contentId, ETemplateTypeUtils.GetValue(templateType), type, scope, isAverage, style, addNum, since, string.Empty);
        }

        private static string GetAnalysisValue(PageInfo pageInfo, int channelId, int contentId, string templateType, string type, string scope, bool isAverage, string style, int addNum, string since, string referrer)
        {
            var publishmentSystemInfo = pageInfo.PublishmentSystemInfo;
            if (publishmentSystemInfo == null) return string.Empty;

            var html = string.Empty;

            var eStyle = publishmentSystemInfo.Additional.TrackerStyle;
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
                    if (eTemplateType != ETemplateType.FileTemplate)
                    {
                        accessNum = DataProvider.TrackingDao.GetTotalAccessNumByPageInfo(pageInfo.PublishmentSystemId, channelId, contentId, sinceDate);
                    }
                    else
                    {
                        accessNum = DataProvider.TrackingDao.GetTotalAccessNumByPageUrl(pageInfo.PublishmentSystemId, referrer, sinceDate);
                    }
                }
                else
                {
                    accessNum = DataProvider.TrackingDao.GetTotalAccessNum(pageInfo.PublishmentSystemId, sinceDate);
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
                    var trackerDays = (timeSpan.Days == 0) ? 1 : timeSpan.Days;//总统计天数
                    trackerDays = trackerDays + publishmentSystemInfo.Additional.TrackerDays;
                    accessNum = Convert.ToInt32(Math.Round(Convert.ToDouble(accessNum / trackerDays)));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeUniqueVisitor))
            {
                if (StringUtils.EqualsIgnoreCase(scope, ScopePage))
                {
                    if (eTemplateType != ETemplateType.FileTemplate)
                    {
                        accessNum = DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageInfo(pageInfo.PublishmentSystemId, channelId, contentId, sinceDate);
                    }
                    else
                    {
                        accessNum = DataProvider.TrackingDao.GetTotalUniqueAccessNumByPageUrl(pageInfo.PublishmentSystemId, referrer, sinceDate);
                    }
                }
                else
                {
                    accessNum = DataProvider.TrackingDao.GetTotalUniqueAccessNum(pageInfo.PublishmentSystemId, sinceDate);
                    accessNum = accessNum + publishmentSystemInfo.Additional.TrackerUniqueVisitor;
                }
                if (isAverage)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PublishmentSystemId);
                    var timeSpan = new TimeSpan(DateTime.Now.Ticks - nodeInfo.AddDate.Ticks);
                    var trackerDays = (timeSpan.Days == 0) ? 1 : timeSpan.Days;//总统计天数
                    trackerDays = trackerDays + publishmentSystemInfo.Additional.TrackerDays;
                    accessNum = Convert.ToInt32(Math.Round(Convert.ToDouble(accessNum / trackerDays)));
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, TypeCurrentVisitor))
            {
                accessNum = DataProvider.TrackingDao.GetCurrentVisitorNum(pageInfo.PublishmentSystemId, publishmentSystemInfo.Additional.TrackerCurrentMinute);
            }

            accessNum = accessNum + addNum;
            if (accessNum == 0) accessNum = 1;

            if (eStyle != ETrackerStyle.None)
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
            }

            return html;
        }
	}
}
