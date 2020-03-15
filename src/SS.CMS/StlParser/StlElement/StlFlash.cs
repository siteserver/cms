using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.Utility;

namespace SS.CMS.StlParser.StlElement
{
    [StlElement(Title = "显示Flash", Description = "通过 stl:flash 标签在模板中获取并显示栏目或内容的Flash")]
    public class StlFlash
    {
        private StlFlash() { }
        public const string ElementName = "stl:flash";

        [StlAttribute(Title = "栏目索引")]
        private const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目名称")]
        private const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "显示父栏目")]
        private const string Parent = nameof(Parent);

        [StlAttribute(Title = "上级栏目的级别")]
        private const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        private const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "指定存储flash的字段")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "显示的flash地址")]
        private const string Src = nameof(Src);

        [StlAttribute(Title = "当指定的flash不存在时显示的flash地址")]
        private const string AltSrc = nameof(AltSrc);

        [StlAttribute(Title = "宽度")]
        private const string Width = nameof(Width);

        [StlAttribute(Title = "高度")]
        private const string Height = nameof(Height);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var isGetPicUrlFromAttribute = false;
            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var upLevel = 0;
            var topLevel = -1;
            var type = nameof(Content.ImageUrl);
            var src = string.Empty;
            var altSrc = string.Empty;
            var width = "100%";
            var height = "180";

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
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
                else if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Src))
                {
                    src = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AltSrc))
                {
                    altSrc = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Width))
                {
                    width = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Height))
                {
                    height = value;
                }
            }

            return await ParseImplAsync(parseManager, isGetPicUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, src, altSrc, width, height);
        }

        private static async Task<object> ParseImplAsync(IParseManager parseManager, bool isGetPicUrlFromAttribute, string channelIndex, string channelName, int upLevel, int topLevel, string type, string src, string altSrc, string width, string height)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            object parsedContent = null;

            var contentId = 0;
            //判断是否图片地址由标签属性获得
            if (!isGetPicUrlFromAttribute)
            {
                contentId = contextInfo.ContentId;
            }
            var contentInfo = await parseManager.GetContentAsync();

            var flashUrl = string.Empty;
            if (!string.IsNullOrEmpty(src))
            {
                flashUrl = src;
            }
            else
            {
                if (contentId != 0)//获取内容Flash
                {
                    if (contentInfo != null)
                    {
                        flashUrl = contentInfo.Get<string>(type);
                    }
                }
                else//获取栏目Flash
                {
                    var dataManager = new StlDataManager(parseManager.DatabaseManager);
                    var channelId = await dataManager.GetChannelIdByLevelAsync(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);

                    channelId = await dataManager.GetChannelIdByChannelIdOrChannelIndexOrChannelNameAsync(pageInfo.SiteId, channelId, channelIndex, channelName);
                    var channel = await parseManager.DatabaseManager.ChannelRepository.GetAsync(channelId);

                    flashUrl = channel.ImageUrl;
                }
            }

            if (string.IsNullOrEmpty(flashUrl))
            {
                flashUrl = altSrc;
            }

            // 如果是实体标签则返回空
            if (contextInfo.IsStlEntity)
            {
                return flashUrl;
            }

            if (!string.IsNullOrEmpty(flashUrl))
            {
                var extension = PathUtils.GetExtension(flashUrl);
                if (FileUtils.IsImage(extension))
                {
                    parsedContent = await StlImage.ParseAsync(parseManager);
                }
                else if (FileUtils.IsPlayer(extension))
                {
                    parsedContent = await StlPlayer.ParseAsync(parseManager);
                }
                else
                {                    
                    flashUrl = await parseManager.PathManager.ParseNavigationUrlAsync(pageInfo.Site, flashUrl, pageInfo.IsLocal);

                    parsedContent = $@"
<embed src=""{flashUrl}"" allowfullscreen=""true"" width=""{width}"" height=""{height}"" align=""middle"" allowscriptaccess=""always"" type=""application/x-shockwave-flash"" />
";
                }
            }

            return parsedContent;
        }
    }
}


//using System.Text;
//using SS.CMS.Abstractions;
//using SS.CMS;
//using SS.CMS.Abstractions.Attributes;
//using SS.CMS.StlParser.Cache;
//using SS.CMS.StlParser.Model;
//using SS.CMS.StlParser.Parsers;
//using SS.CMS.StlParser.Utility;
//using SiteServer.Utils.Enumerations;

//namespace SS.CMS.StlParser.StlElement
//{
//    [StlClass(Usage = "显示Flash", Description = "通过 stl:flash 标签在模板中获取并显示栏目或内容的Flash")]
//    public class StlFlash
//    {
//        private StlFlash() { }
//        public const string ElementName = "stl:flash";

//        private static readonly Attr ChannelIndex = new Attr("channelIndex", "栏目索引");
//        private static readonly Attr ChannelName = new Attr("channelName", "栏目名称");
//        private static readonly Attr Parent = new Attr("parent", "显示父栏目");
//        private static readonly Attr UpLevel = new Attr("upLevel", "上级栏目的级别");
//        private static readonly Attr TopLevel = new Attr("topLevel", "从首页向下的栏目级别");
//        private static readonly Attr InputType = new Attr("type", "指定存储flash的字段");
//        private static readonly Attr Src = new Attr("src", "显示的flash地址");
//        private static readonly Attr AltSrc = new Attr("altSrc", "当指定的flash不存在时显示的flash地址");
//        private static readonly Attr Width = new Attr("width", "宽度");
//        private static readonly Attr Height = new Attr("height", "高度");

//        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
//        {
//            var isGetPicUrlFromAttribute = false;
//            var channelIndex = string.Empty;
//            var channelName = string.Empty;
//            var upLevel = 0;
//            var topLevel = -1;
//            var type = ContentAttribute.ImageUrl;
//            var src = string.Empty;
//            var altSrc = string.Empty;
//            var width = "100%";
//            var height = "180";

//            foreach (var name in contextInfo.Attributes.AllKeys)
//            {
//                var value = contextInfo.Attributes[name];

//                if (StringUtils.EqualsIgnoreCase(name, ChannelIndex))
//                {
//                    channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
//                    if (!string.IsNullOrEmpty(channelIndex))
//                    {
//                        isGetPicUrlFromAttribute = true;
//                    }
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, ChannelName))
//                {
//                    channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
//                    if (!string.IsNullOrEmpty(channelName))
//                    {
//                        isGetPicUrlFromAttribute = true;
//                    }
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, Parent))
//                {
//                    if (TranslateUtils.ToBool(value))
//                    {
//                        upLevel = 1;
//                        isGetPicUrlFromAttribute = true;
//                    }
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, UpLevel))
//                {
//                    upLevel = TranslateUtils.ToInt(value);
//                    if (upLevel > 0)
//                    {
//                        isGetPicUrlFromAttribute = true;
//                    }
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, TopLevel))
//                {
//                    topLevel = TranslateUtils.ToInt(value);
//                    if (topLevel >= 0)
//                    {
//                        isGetPicUrlFromAttribute = true;
//                    }
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, InputType))
//                {
//                    type = value;
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, Src))
//                {
//                    src = value;
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, AltSrc))
//                {
//                    altSrc = value;
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, Width))
//                {
//                    width = value;
//                }
//                else if (StringUtils.EqualsIgnoreCase(name, Height))
//                {
//                    height = value;
//                }
//            }

//            return ParseImpl(pageInfo, contextInfo, isGetPicUrlFromAttribute, channelIndex, channelName, upLevel, topLevel, type, src, altSrc, width, height);
//        }

//        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, bool isGetPicUrlFromAttribute, string channelIndex, string channelName, int upLevel, int topLevel, string type, string src, string altSrc, string width, string height)
//        {
//            var parsedContent = string.Empty;

//            var contentId = 0;
//            //判断是否图片地址由标签属性获得
//            if (!isGetPicUrlFromAttribute)
//            {
//                contentId = contextInfo.ContentId;
//            }
//            var contentInfo = contextInfo.Body;

//            string picUrl;
//            if (!string.IsNullOrEmpty(src))
//            {
//                picUrl = src;
//            }
//            else
//            {
//                if (contentId != 0)//获取内容Flash
//                {
//                    if (contentInfo == null)
//                    {
//                        var node = await GlobalSettings.ChannelRepository.GetAsync(contextInfo.Site.Id, contextInfo.ChannelId);
//                        var tableName = GlobalSettings.ChannelRepository.GetTableName(contextInfo.Site, node);

//                        //picUrl = GlobalSettings.ContentRepository.GetValue(tableName, contentId, type);
//                        picUrl = Body.GetValue(tableName, contentId, type);
//                    }
//                    else
//                    {
//                        picUrl = contextInfo.Body.GetString(type);
//                    }
//                }
//                else//获取栏目Flash
//                {
//                    var channelId = StlDataUtility.GetChannelIdByLevel(pageInfo.SiteId, contextInfo.ChannelId, upLevel, topLevel);

//                    channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, channelId, channelIndex, channelName);
//                    var channel = await GlobalSettings.ChannelRepository.GetAsync(pageInfo.SiteId, channelId);

//                    picUrl = channel.ImageUrl;
//                }
//            }

//            if (string.IsNullOrEmpty(picUrl))
//            {
//                picUrl = altSrc;
//            }

//            // 如果是实体标签则返回空
//            if (contextInfo.IsStlEntity)
//            {
//                return picUrl;
//            }

//            if (!string.IsNullOrEmpty(picUrl))
//            {
//                var extension = PathUtils.GetExtension(picUrl);
//                if (EFileSystemTypeUtils.IsImage(extension))
//                {
//                    parsedContent = StlImage.Parse(pageInfo, contextInfo);
//                }
//                else if (EFileSystemTypeUtils.IsPlayer(extension))
//                {
//                    parsedContent = StlPlayer.Parse(pageInfo, contextInfo);
//                }
//                else
//                {
//                    pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcSwfObject);

//                    picUrl = PageUtility.ParseNavigationUrl(pageInfo.Site, picUrl, pageInfo.IsLocal);

//                    if (string.IsNullOrEmpty(contextInfo.Attributes["quality"]))
//                    {
//                        contextInfo.Attributes["quality"] = "high";
//                    }
//                    if (string.IsNullOrEmpty(contextInfo.Attributes["wmode"]))
//                    {
//                        contextInfo.Attributes["wmode"] = "transparent";
//                    }
//                    var paramBuilder = new StringBuilder();
//                    var uniqueId = pageInfo.UniqueId;
//                    foreach (var key in contextInfo.Attributes.AllKeys)
//                    {
//                        paramBuilder.Append($@"    so_{uniqueId}.addParam(""{key}"", ""{contextInfo.Attributes[key]}"");").Append(Constants.ReturnAndNewline);
//                    }

//                    parsedContent = $@"
//<div id=""flashcontent_{uniqueId}""></div>
//<script type=""text/javascript"">
//    // <![CDATA[
//    var so_{uniqueId} = new SWFObject(""{picUrl}"", ""flash_{uniqueId}"", ""{width}"", ""{height}"", ""7"", """");
//{paramBuilder}
//    so_{uniqueId}.write(""flashcontent_{uniqueId}"");
//    // ]]>
//</script>
//";
//                }
//            }

//            return parsedContent;
//        }
//    }
//}
