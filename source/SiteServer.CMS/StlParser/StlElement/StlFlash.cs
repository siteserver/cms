using System;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlFlash
	{
		private StlFlash(){}
		public const string ElementName = "stl:flash";//显示Flash

		public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
		public const string Attribute_ChannelName = "channelname";				//栏目名称
		public const string Attribute_Parent = "parent";						//显示父栏目
		public const string Attribute_UpLevel = "uplevel";						//上级栏目的级别
        public const string Attribute_TopLevel = "toplevel";					//从首页向下的栏目级别
        public const string Attribute_Type = "type";							//指定存储flash的字段
		public const string Attribute_Src = "src";								//显示的flash地址
        public const string Attribute_AltSrc = "altsrc";						//当指定的flash不存在时显示的flash地址

		public const string Attribute_Width = "width";							//宽度
		public const string Attribute_Height = "height";						//高度
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();
				attributes.Add(Attribute_ChannelIndex, "栏目索引");
				attributes.Add(Attribute_ChannelName, "栏目名称");
				attributes.Add(Attribute_Parent, "显示父栏目");
				attributes.Add(Attribute_UpLevel, "上级栏目的级别");
                attributes.Add(Attribute_TopLevel, "从首页向下的栏目级别");
                attributes.Add(Attribute_Type, "指定存储flash的字段");
                attributes.Add(Attribute_Src, "显示的flash地址");
                attributes.Add(Attribute_AltSrc, "当指定的flash不存在时显示的flash地址");
				attributes.Add(Attribute_Width, "宽度");
				attributes.Add(Attribute_Height, "高度");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}


		//对“栏目链接”（stl:flash）元素进行解析
        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
				var ie = node.Attributes.GetEnumerator();
				var isGetPicUrlFromAttribute = false;
				var channelIndex = string.Empty;
				var channelName = string.Empty;
				var upLevel = 0;
                var topLevel = -1;
                var type = BackgroundContentAttribute.ImageUrl;
				var src = string.Empty;
                var altSrc = string.Empty;
				var width = "100%";
                var height = "180";
                var isDynamic = false;
                var parameters = new NameValueCollection();

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
					if (attributeName.Equals(Attribute_ChannelIndex))
					{
                        channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
						if (!string.IsNullOrEmpty(channelIndex))
						{
							isGetPicUrlFromAttribute = true;
						}
					}
					else if (attributeName.Equals(Attribute_ChannelName))
					{
                        channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
						if (!string.IsNullOrEmpty(channelName))
						{
							isGetPicUrlFromAttribute = true;
						}
					}
					else if (attributeName.Equals(Attribute_Parent))
					{					
						if (TranslateUtils.ToBool(attr.Value))
						{
							upLevel = 1;
							isGetPicUrlFromAttribute = true;
						}
					}
					else if (attributeName.Equals(Attribute_UpLevel))
					{
						upLevel = TranslateUtils.ToInt(attr.Value);
						if (upLevel > 0)
						{
							isGetPicUrlFromAttribute = true;
						}
                    }
                    else if (attributeName.Equals(Attribute_TopLevel))
                    {
                        topLevel = TranslateUtils.ToInt(attr.Value);
                        if (topLevel >= 0)
                        {
                            isGetPicUrlFromAttribute = true;
                        }
                    }
                    else if (attributeName.Equals(Attribute_Type))
                    {
                        type = attr.Value;
                    }
					else if (attributeName.Equals(Attribute_Src))
					{
						src = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_AltSrc))
                    {
                        altSrc = attr.Value;
                    }
					else if (attributeName.Equals(Attribute_Width))
					{
                        width = attr.Value;
					}
					else if (attributeName.Equals(Attribute_Height))
					{
                        height = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value, false);
                    }
                    else
                    {
                        parameters.Add(attr.Name, attr.Value);
                    }
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(stlElement, node, pageInfo, contextInfo, isGetPicUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, src, altSrc, width, height, parameters);
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImpl(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, bool isGetPicUrlFromAttribute, string channelIndex, string channelName, int upLevel, int topLevel, string type, string src, string altSrc, string width, string height, NameValueCollection parameters)
        {
            var parsedContent = string.Empty;

            var contentID = 0;
            //判断是否图片地址由标签属性获得
            if (!isGetPicUrlFromAttribute)
            {
                contentID = contextInfo.ContentID;
            }
            var contentInfo = contextInfo.ContentInfo;

            string picUrl;
            if (!string.IsNullOrEmpty(src))
            {
                picUrl = src;
            }
            else
            {
                if (contentID != 0)//获取内容Flash
                {
                    if (contentInfo == null)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(contextInfo.PublishmentSystemInfo.PublishmentSystemId, contextInfo.ChannelID);
                        var tableName = NodeManager.GetTableName(contextInfo.PublishmentSystemInfo, nodeInfo);

                        picUrl = BaiRongDataProvider.ContentDao.GetValue(tableName, contentID, type);
                    }
                    else
                    {
                        picUrl = contextInfo.ContentInfo.GetExtendedAttribute(type);
                    }
                }
                else//获取栏目Flash
                {
                    var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);

                    channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);
                    var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);

                    picUrl = channel.ImageUrl;
                }
            }

            if (string.IsNullOrEmpty(picUrl))
            {
                picUrl = altSrc;
            }

            if (!string.IsNullOrEmpty(picUrl))
            {
                var extension = PathUtils.GetExtension(picUrl);
                if (EFileSystemTypeUtils.IsImage(extension))
                {
                    parsedContent = StlImage.Parse(stlElement, node, pageInfo, contextInfo);
                }
                else if (EFileSystemTypeUtils.IsPlayer(extension))
                {
                    parsedContent = StlPlayer.Parse(stlElement, node, pageInfo, contextInfo);
                }
                else
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

                    picUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, picUrl);

                    if (string.IsNullOrEmpty(parameters["quality"]))
                    {
                        parameters["quality"] = "high";
                    }
                    if (string.IsNullOrEmpty(parameters["wmode"]))
                    {
                        parameters["wmode"] = "transparent";
                    }
                    var paramBuilder = new StringBuilder();
                    var uniqueID = pageInfo.UniqueId;
                    foreach (string key in parameters.Keys)
                    {
                        paramBuilder.Append($@"    so_{uniqueID}.addParam(""{key}"", ""{parameters[key]}"");").Append(StringUtils.Constants.ReturnAndNewline);
                    }

                    parsedContent = $@"
<div id=""flashcontent_{uniqueID}""></div>
<script type=""text/javascript"">
    // <![CDATA[
    var so_{uniqueID} = new SWFObject(""{picUrl}"", ""flash_{uniqueID}"", ""{width}"", ""{height}"", ""7"", """");
{paramBuilder}
    so_{uniqueID}.write(""flashcontent_{uniqueID}"");
    // ]]>
</script>
";
                }
            }

            return parsedContent;
        }
	}
}
