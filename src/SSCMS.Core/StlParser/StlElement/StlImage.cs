using System.Collections.Specialized;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "显示图片", Description = "通过 stl:image 标签在模板中显示栏目或内容的图片")]
    public static class StlImage
	{
		public const string ElementName = "stl:image";

		[StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目索引")]
        private const string Index = nameof(Index);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);
        
		[StlAttribute(Title = "显示父栏目")]
        private const string Parent = nameof(Parent);
        
		[StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);
        
        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        [StlAttribute(Title = "指定存储图片的字段")]
        private const string Type = nameof(Type);

	    [StlAttribute(Title = "显示字段存储的第几幅图片，默认为 1")]
	    private const string No = nameof(No);

        [StlAttribute(Title = "是否清除 HTML 标签")]
        private const string IsClearTags = nameof(IsClearTags);

        [StlAttribute(Title = "如果是引用内容，是否获取所引用内容的值")]
        private const string IsOriginal = nameof(IsOriginal);
        
		[StlAttribute(Title = "显示的图片地址")]
        private const string Src = nameof(Src);
        
        [StlAttribute(Title = "当指定的图片不存在时显示的图片地址")]
        private const string AltSrc = nameof(AltSrc);

        public static async Task<object> ParseAsync(IParseManager parseManager)
		{
		    var isGetPicUrlFromAttribute = false;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var type = nameof(Content.ImageUrl);
		    var no = 0;
            var isClearTags = false;
            var isOriginal = false;
            var src = string.Empty;
            var altSrc = string.Empty;
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex) || StringUtils.EqualsIgnoreCase(name, Index))
                {
                    channelIndex = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (!string.IsNullOrEmpty(channelIndex))
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
                {
                    channelName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (!string.IsNullOrEmpty(channelName))
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Parent))
                {
                    if (TranslateUtils.ToBool(value))
                    {
                        upLevel = 1;
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
                {
                    upLevel = TranslateUtils.ToInt(value);
                    if (upLevel > 0)
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
                {
                    topLevel = TranslateUtils.ToInt(value);
                    if (topLevel >= 0)
                    {
                        isGetPicUrlFromAttribute = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseManager.ContextInfo.ContextType = TranslateUtils.ToEnum(value, ParseType.Undefined);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, No))
                {
                    no = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsClearTags))
                {
                    isClearTags = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsOriginal))
                {
                    isOriginal = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Src))
                {
                    src = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AltSrc))
                {
                    altSrc = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, attributes, isGetPicUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, no, isOriginal, isClearTags, src, altSrc);
		}

        private static async Task<object> ParseAsync(IParseManager parseManager, NameValueCollection attributes,
            bool isGetPicUrlFromAttribute, string channelIndex, string channelName, int upLevel, int topLevel,
            string type, int no, bool isOriginal, bool isClearTags, string src, string altSrc)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var contentId = 0;
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
                if (contextType == ParseType.Undefined)
                {
                    contextType = contentId != 0 ? ParseType.Content : ParseType.Channel;
                }

                if (contextType == ParseType.Content)
                {
                    var contentInfo = await parseManager.GetContentAsync();

                    if (isOriginal)
                    {
                        if (contentInfo != null && contentInfo.ReferenceId > 0 && contentInfo.SourceId > 0)
                        {
                            var targetChannelId = contentInfo.SourceId;
                            var targetSiteId = await databaseManager.ChannelRepository.GetSiteIdAsync(targetChannelId);
                            var targetSite = await databaseManager.SiteRepository.GetAsync(targetSiteId);
                            var targetNodeInfo = await databaseManager.ChannelRepository.GetAsync(targetChannelId);

                            var targetContentInfo =
                                await databaseManager.ContentRepository.GetAsync(targetSite, targetNodeInfo,
                                    contentInfo.ReferenceId);
                            if (targetContentInfo != null && targetContentInfo.ChannelId > 0)
                            {
                                contentInfo = targetContentInfo;
                            }
                        }
                    }

                    if (contentInfo == null)
                    {
                        contentInfo =
                            await databaseManager.ContentRepository.GetAsync(pageInfo.Site, contextInfo.ChannelId,
                                contentId);
                    }

                    if (contentInfo != null)
                    {
                        if (no <= 1)
                        {
                            picUrl = contentInfo.Get<string>(type);
                        }
                        else
                        {
                            var extendName = ColumnsManager.GetExtendName(type, no - 1);
                            picUrl = contentInfo.Get<string>(extendName);
                        }
                    }
                }
                else if (contextType == ParseType.Channel) //获取栏目图片
                {
                    var dataManager = new StlDataManager(parseManager.DatabaseManager);
                    var channelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId,
                        upLevel, topLevel);

                    channelId = await dataManager.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(
                        pageInfo.SiteId, channelId, channelIndex, channelName);

                    var channel = await databaseManager.ChannelRepository.GetAsync(channelId);

                    if (no <= 1)
                    {
                        picUrl = channel.Get<string>(type);
                    }
                    else
                    {
                        var extendName = ColumnsManager.GetExtendName(type, no - 1);
                        picUrl = channel.Get<string>(extendName);
                    }
                }
                else if (contextType == ParseType.Each)
                {
                    picUrl = contextInfo.ItemContainer.EachItem.Value as string;
                }
            }

            if (string.IsNullOrEmpty(picUrl))
            {
                picUrl = altSrc;
            }

            if (string.IsNullOrEmpty(picUrl)) return string.Empty;

            var extension = PathUtils.GetExtension(picUrl);
            if (FileUtils.IsFlash(extension))
            {
                return await StlPdf.ParseAsync(parseManager);
            }

            if (FileUtils.IsPlayer(extension))
            {
                return await StlPlayer.ParseAsync(parseManager);
            }

            picUrl = await parseManager.PathManager.ParseSiteUrlAsync(pageInfo.Site, picUrl, pageInfo.IsLocal);
            if (isClearTags || contextInfo.IsStlEntity)
            {
                return picUrl;
            }

            attributes["src"] = picUrl;
            if (pageInfo.EditMode == EditMode.Visual)
            {
                var editable = VisualUtility.GetEditable(pageInfo, contextInfo);
                var editableAttributes = VisualUtility.GetEditableAttributes(editable);
                foreach (var key in editableAttributes.AllKeys)
                {
                    attributes[key] = editableAttributes[key];
                }
            }
            return $@"<img {TranslateUtils.ToAttributesString(attributes)}>";
        }
    }
}
