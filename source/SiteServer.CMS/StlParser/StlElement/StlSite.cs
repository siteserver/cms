using System;
using System.Collections.Specialized;
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
	public class StlSite
	{
        private StlSite() { }
		public const string ElementName = "stl:site";       //显示指定站点的数据

        public const string Attribute_SiteName = "sitename";				//站点名称
        public const string Attribute_Directory = "directory";				//站点文件夹
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
                attributes.Add(Attribute_SiteName, "站点名称");
                attributes.Add(Attribute_Directory, "站点文件夹");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}


        //循环解析型标签
        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
		{
			var parsedContent = string.Empty;
            
			try
			{
                if (!string.IsNullOrEmpty(node.InnerXml))
                {
                    var contextInfo = contextInfoRef.Clone();
                    var ie = node.Attributes.GetEnumerator();
                    var siteName = string.Empty;
                    var directory = string.Empty;
                    var isDynamic = false;

                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();
                        if (attributeName.Equals(Attribute_SiteName))
                        {
                            siteName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (attributeName.Equals(Attribute_Directory))
                        {
                            directory = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (attributeName.Equals(Attribute_IsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                    }

                    if (isDynamic)
                    {
                        parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                    }
                    else
                    {
                        parsedContent = ParseImpl(node, pageInfo, contextInfo, siteName, directory);
                    }
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string siteName, string directory)
        {
            var parsedContent = string.Empty;

            PublishmentSystemInfo publishmentSystemInfo = null;

            if (!string.IsNullOrEmpty(siteName))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoBySiteName(siteName);
            }
            else if (!string.IsNullOrEmpty(directory))
            {
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfoByDirectory(directory);
            }
            else
            {
                var siteID = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
                if (siteID > 0)
                {
                    publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(siteID);
                }
            }

            if (publishmentSystemInfo != null)
            {
                var prePublishmentSystemInfo = pageInfo.PublishmentSystemInfo;
                var prePageNodeID = pageInfo.PageNodeId;
                var prePageContentID = pageInfo.PageContentId;

                pageInfo.ChangeSite(publishmentSystemInfo, publishmentSystemInfo.PublishmentSystemId, 0, contextInfo);

                var innerBuilder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                parsedContent = innerBuilder.ToString();

                pageInfo.ChangeSite(prePublishmentSystemInfo, prePageNodeID, prePageContentID, contextInfo);
            }

            return parsedContent;
        }
	}
}
