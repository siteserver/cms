using System.Web.UI.HtmlControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "显示图片", Description = "通过 stl:image 标签在模板中显示栏目或内容的图片")]
    public class StlImage
	{
		private StlImage(){}
		public const string ElementName = "stl:image";

		private static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
		private static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
        private static readonly Attr No = new Attr("no", "显示字段的顺序");
		private static readonly Attr Parent = new Attr("parent", "显示父栏目");
		private static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别");
        private static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别");
        private static readonly Attr Type = new Attr("type", "指定存储图片的字段");
        private static readonly Attr IsOriginal = new Attr("isOriginal", "如果是引用内容，是否获取所引用内容的值");
		private static readonly Attr Src = new Attr("src", "显示的图片地址");
        private static readonly Attr AltSrc = new Attr("altSrc", "当指定的图片不存在时显示的图片地址");
        private static readonly Attr Width = new Attr("width", "宽度");
        private static readonly Attr Height = new Attr("height", "高度");

        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
		{
		    var isGetPicUrlFromAttribute = false;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var no = 0;
            var upLevel = 0;
            var topLevel = -1;
            var type = BackgroundContentAttribute.ImageUrl;
            var isOriginal = false;
            var src = string.Empty;
            var altSrc = string.Empty;
            var stlImage = new HtmlImage();

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex.Name))
                {
                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName.Name))
                {
                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, No.Name))
                {
                    no = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent.Name))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel.Name))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel.Name))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type.Name))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOriginal.Name))
                {
                    isOriginal = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Src.Name))
                {
                    src = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AltSrc.Name))
                {
                    altSrc = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else
                {
                    stlImage.Attributes[name] = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, stlImage, isGetPicUrlFromAttribute, channelIndex, channelName, no, upLevel, topLevel, type, isOriginal, src, altSrc);
		}

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlImage stlImage, bool isGetPicUrlFromAttribute, string channelIndex, string channelName, int no, int upLevel, int topLevel, string type, bool isOriginal, string src, string altSrc)
        {
            var parsedContent = string.Empty;

            var contentId = 0;
            //判断是否图片地址由标签属性获得
            if (!isGetPicUrlFromAttribute)
            {
                contentId = contextInfo.ContentId;
            }
            var contextType = contextInfo.ContextType;

            var picUrl = string.Empty;
            if (!string.IsNullOrEmpty(src))
            {
                picUrl = src;
            }
            else
            {
                if (contextType == EContextType.Undefined)
                {
                    contextType = contentId != 0 ? EContextType.Content : EContextType.Channel;
                }

                if (contextType == EContextType.Content)//获取内容图片
                {
                    var contentInfo = contextInfo.ContentInfo;

                    if (isOriginal)
                    {
                        if (contentInfo != null && contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0)
                        {
                            var targetChannelId = contentInfo.SourceId;
                            //var targetSiteId = DataProvider.ChannelDao.GetSiteId(targetChannelId);
                            var targetSiteId = Node.GetSiteId(targetChannelId);
                            var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
                            var targetNodeInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);

                            var tableName = ChannelManager.GetTableName(targetSiteInfo, targetNodeInfo);
                            //var targetContentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentInfo.ReferenceId);
                            var targetContentInfo = Content.GetContentInfo(tableName, contentInfo.ReferenceId);
                            if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                            {
                                contentInfo = targetContentInfo;
                            }
                        }
                    }

                    if (contentInfo == null)
                    {
                        //contentInfo = DataProvider.ContentDao.GetContentInfo(ETableStyle.BackgroundContent, pageInfo.SiteInfo.AuxiliaryTableForContent, contentId);
                        contentInfo = Content.GetContentInfo(pageInfo.SiteInfo.TableName, contentId);
                    }

                    if (contentInfo != null)
                    {
                        if (no <= 1)
                        {
                            picUrl = contentInfo.GetString(type);
                        }
                        else
                        {
                            var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                            var extendValues = contentInfo.GetString(extendAttributeName);
                            if (!string.IsNullOrEmpty(extendValues))
                            {
                                var index = 2;
                                foreach (var extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                                {
                                    if (index == no)
                                    {
                                        picUrl = extendValue;
                                        break;
                                    }
                                    index++;
                                }
                            }
                        }
                    }
                }
                else if (contextType == EContextType.Channel)//获取栏目图片
                {
                    var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);

                    channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, channelIndex, channelName);

                    var channel = ChannelManager.GetChannelInfo(pageInfo.SiteId, channelId);

                    picUrl = channel.ImageUrl;
                }
                else if (contextType == EContextType.Each)
                {
                    picUrl = contextInfo.ItemContainer.EachItem.DataItem as string;
                }
            }

            if (string.IsNullOrEmpty(picUrl))
            {
                picUrl = altSrc;
            }

            if (!string.IsNullOrEmpty(picUrl))
            {
                var extension = PathUtils.GetExtension(picUrl);
                if (EFileSystemTypeUtils.IsFlash(extension))
                {
                    parsedContent = StlFlash.Parse(pageInfo, contextInfo);
                }
                else if (EFileSystemTypeUtils.IsPlayer(extension))
                {
                    parsedContent = StlPlayer.Parse(pageInfo, contextInfo);
                }
                else
                {
                    stlImage.Src = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, picUrl, pageInfo.IsLocal);
                    parsedContent = ControlUtils.GetControlRenderHtml(stlImage);
                }
            }

            return parsedContent;
        }
	}
}
