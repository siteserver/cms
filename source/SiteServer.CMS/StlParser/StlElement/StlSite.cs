using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "获取站点值", Description = "通过 stl:site 标签在模板中显示站点值")]
    public class StlSite
	{
        private StlSite() { }
		public const string ElementName = "stl:site";

        public const string AttributeSiteName = "siteName";
        public const string AttributeSiteDir = "siteDir";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeSiteName, "站点名称"},
	        {AttributeSiteDir, "站点文件夹"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        //循环解析型标签
        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
		{
			var parsedContent = string.Empty;
            
			try
			{
                if (!string.IsNullOrEmpty(node.InnerXml))
                {
                    var contextInfo = contextInfoRef.Clone();
                    var siteName = string.Empty;
                    var siteDir = string.Empty;
                    var isDynamic = false;

                    var ie = node.Attributes?.GetEnumerator();
                    if (ie != null)
                    {
                        while (ie.MoveNext())
                        {
                            var attr = (XmlAttribute)ie.Current;

                            if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSiteName))
                            {
                                siteName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            }
                            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeSiteDir))
                            {
                                siteDir = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                            }
                            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                            {
                                isDynamic = TranslateUtils.ToBool(attr.Value);
                            }
                        }
                    }

                    parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, siteName, siteDir);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string siteName, string siteDir)
        {
            PublishmentSystemInfo publishmentSystemInfo = null;

            if (!string.IsNullOrEmpty(siteName))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(siteDir))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoByDirectory(siteDir);
            }
            else
            {
                var siteId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
                if (siteId > 0)
                {
                    publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteId);
                }
            }

            if (publishmentSystemInfo == null) return string.Empty;

            var prePublishmentSystemInfo = pageInfo.PublishmentSystemInfo;
            var prePageNodeId = pageInfo.PageNodeId;
            var prePageContentId = pageInfo.PageContentId;

            pageInfo.ChangeSite(publishmentSystemInfo, publishmentSystemInfo.PublishmentSystemId, 0, contextInfo);

            var innerBuilder = new StringBuilder(node.InnerXml);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            var parsedContent = innerBuilder.ToString();

            pageInfo.ChangeSite(prePublishmentSystemInfo, prePageNodeId, prePageContentId, contextInfo);

            return parsedContent;
        }
	}
}
