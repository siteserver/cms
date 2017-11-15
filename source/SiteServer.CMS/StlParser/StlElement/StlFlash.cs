using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "显示Flash", Description = "通过 stl:flash 标签在模板中获取并显示栏目或内容的Flash")]
    public class StlFlash
    {
        private StlFlash() { }
        public const string ElementName = "stl:flash";

        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeParent = "parent";
        public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeType = "type";
        public const string AttributeSrc = "src";
        public const string AttributeAltSrc = "altSrc";
        public const string AttributeWidth = "width";
        public const string AttributeHeight = "height";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeParent, "显示父栏目"},
            {AttributeUpLevel, "上级栏目的级别"},
            {AttributeTopLevel, "从首页向下的栏目级别"},
            {AttributeType, "指定存储flash的字段"},
            {AttributeSrc, "显示的flash地址"},
            {AttributeAltSrc, "当指定的flash不存在时显示的flash地址"},
            {AttributeWidth, "宽度"},
            {AttributeHeight, "高度"}
        };

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
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

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeParent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeUpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeType))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeSrc))
                {
                    src = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeAltSrc))
                {
                    altSrc = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWidth))
                {
                    width = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeHeight))
                {
                    height = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, isGetPicUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, src, altSrc, width, height);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, bool isGetPicUrlFromAttribute, string channelIndex, string channelName, int upLevel, int topLevel, string type, string src, string altSrc, string width, string height)
        {
            var parsedContent = string.Empty;

            var contentId = 0;
            //判断是否图片地址由标签属性获得
            if (!isGetPicUrlFromAttribute)
            {
                contentId = contextInfo.ContentId;
            }
            var contentInfo = contextInfo.ContentInfo;

            string picUrl;
            if (!string.IsNullOrEmpty(src))
            {
                picUrl = src;
            }
            else
            {
                if (contentId != 0)//获取内容Flash
                {
                    if (contentInfo == null)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(contextInfo.PublishmentSystemInfo.PublishmentSystemId, contextInfo.ChannelId);
                        var tableName = NodeManager.GetTableName(contextInfo.PublishmentSystemInfo, nodeInfo);

                        //picUrl = BaiRongDataProvider.ContentDao.GetValue(tableName, contentId, type);
                        picUrl = Content.GetValue(tableName, contentId, type);
                    }
                    else
                    {
                        picUrl = contextInfo.ContentInfo.GetExtendedAttribute(type);
                    }
                }
                else//获取栏目Flash
                {
                    var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);

                    channelId = StlDataUtility.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);
                    var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

                    picUrl = channel.ImageUrl;
                }
            }

            if (string.IsNullOrEmpty(picUrl))
            {
                picUrl = altSrc;
            }

            // 如果是实体标签则返回空
            if (contextInfo.IsCurlyBrace)
            {
                return picUrl;
            }

            if (!string.IsNullOrEmpty(picUrl))
            {
                var extension = PathUtils.GetExtension(picUrl);
                if (EFileSystemTypeUtils.IsImage(extension))
                {
                    parsedContent = StlImage.Parse(pageInfo, contextInfo);
                }
                else if (EFileSystemTypeUtils.IsPlayer(extension))
                {
                    parsedContent = StlPlayer.Parse(pageInfo, contextInfo);
                }
                else
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAcSwfObject);

                    picUrl = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, picUrl);

                    if (!contextInfo.Attributes.ContainsKey("quality"))
                    {
                        contextInfo.Attributes["quality"] = "high";
                    }
                    if (!contextInfo.Attributes.ContainsKey("wmode"))
                    {
                        contextInfo.Attributes["wmode"] = "transparent";
                    }
                    var paramBuilder = new StringBuilder();
                    var uniqueId = pageInfo.UniqueId;
                    foreach (var key in contextInfo.Attributes.Keys)
                    {
                        paramBuilder.Append($@"    so_{uniqueId}.addParam(""{key}"", ""{contextInfo.Attributes[key]}"");").Append(StringUtils.Constants.ReturnAndNewline);
                    }

                    parsedContent = $@"
<div id=""flashcontent_{uniqueId}""></div>
<script type=""text/javascript"">
    // <![CDATA[
    var so_{uniqueId} = new SWFObject(""{picUrl}"", ""flash_{uniqueId}"", ""{width}"", ""{height}"", ""7"", """");
{paramBuilder}
    so_{uniqueId}.write(""flashcontent_{uniqueId}"");
    // ]]>
</script>
";
                }
            }

            return parsedContent;
        }
    }
}
