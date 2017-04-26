using System;
using System.Collections.Specialized;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlCount
	{
        private StlCount() { }
		public const string ElementName = "stl:count";              //显示数值

		public const string Attribute_Type = "type";		        //需要获取值的类型
        public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
        public const string Attribute_ChannelName = "channelname";				//栏目名称
        public const string Attribute_UpLevel = "uplevel";						//上级栏目的级别
        public const string Attribute_TopLevel = "toplevel";					//从首页向下的栏目级别
        public const string Attribute_Scope = "scope";							//内容范围
        public const string Attribute_Since = "since";				        //时间段
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

        public const string Type_Channels = "Channels";	            //栏目数
        public const string Type_Contents = "Contents";	            //内容数
        public const string Type_Comments = "Comments";	            //内容数
        public const string Type_Downloads = "Downloads";	        //下载次数

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_Type, "需要获取值的类型");
                attributes.Add(Attribute_ChannelIndex, "栏目索引");
                attributes.Add(Attribute_ChannelName, "栏目名称");
                attributes.Add(Attribute_UpLevel, "上级栏目的级别");
                attributes.Add(Attribute_TopLevel, "从首页向下的栏目级别");
                attributes.Add(Attribute_Scope, "内容范围");
                attributes.Add(Attribute_Since, "时间段");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
				var ie = node.Attributes.GetEnumerator();

				var type = string.Empty;
                var channelIndex = string.Empty;
                var channelName = string.Empty;
                var upLevel = 0;
                var topLevel = -1;
                var scope = EScopeType.Self;
                var since = string.Empty;
                var isDynamic = false;

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_Type))
					{
						type = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ChannelIndex))
                    {
                        channelIndex = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ChannelName))
                    {
                        channelName = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_UpLevel))
                    {
                        upLevel = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_TopLevel))
                    {
                        topLevel = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Scope))
                    {
                        scope = EScopeTypeUtils.GetEnumType(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_Since))
                    {
                        since = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value, false);
                    }
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, contextInfo, type, channelIndex, channelName, upLevel, topLevel, scope, since);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string type, string channelIndex, string channelName, int upLevel, int topLevel, EScopeType scope, string since)
        {
            var count = 0;

            var sinceDate = DateUtils.SqlMinValue;
            if (!string.IsNullOrEmpty(since))
            {
                sinceDate = DateTime.Now.AddHours(-DateUtils.GetSinceHours(since));
            }

            if (string.IsNullOrEmpty(type) || StringUtils.EqualsIgnoreCase(type, Type_Contents))
            {
                var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);
                channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);

                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo, scope, string.Empty, string.Empty);
                foreach (int nodeID in nodeIdList)
                {
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, nodeID);
                    count += DataProvider.ContentDao.GetCountOfContentAdd(tableName, pageInfo.PublishmentSystemId, nodeID, sinceDate, DateTime.Now.AddDays(1), string.Empty);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(type, Type_Channels))
            {
                var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);
                channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);

                var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);
                count = nodeInfo.ChildrenCount;
            }
            else if (StringUtils.EqualsIgnoreCase(type, Type_Comments))
            {
                count = DataProvider.CommentDao.GetCountChecked(pageInfo.PublishmentSystemId, contextInfo.ChannelID, contextInfo.ContentID);
            }
            else if (StringUtils.EqualsIgnoreCase(type, Type_Downloads))
            {
                if (contextInfo.ContentID > 0)
                {
                    count = CountManager.GetCount(pageInfo.PublishmentSystemInfo.AuxiliaryTableForContent, contextInfo.ContentID.ToString(), ECountType.Download);
                }
            }

            return count.ToString();
        }
	}
}
