using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "滚动焦点图")]
    public class StlFocusViewer
    {
        private StlFocusViewer() { }
        public const string ElementName = "stl:focusViewer";

        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeScope = "scope";
        public const string AttributeGroup = "group";
        public const string AttributeGroupNot = "groupNot";
        public const string AttributeGroupChannel = "groupChannel";
        public const string AttributeGroupChannelNot = "groupChannelNot";
        public const string AttributeGroupContent = "groupContent";
        public const string AttributeGroupContentNot = "groupContentNot";
        public const string AttributeTags = "tags";
        public const string AttributeOrder = "order";
        public const string AttributeStartNum = "startNum";
        public const string AttributeTotalNum = "totalNum";
        public const string AttributeTitleWordNum = "titleWordNum";
        public const string AttributeWhere = "where";
        public const string AttributeIsTop = "isTop";
        public const string AttributeIsRecommend = "isRecommend";
        public const string AttributeIsHot = "isHot";
        public const string AttributeIsColor = "isColor";
        public const string AttributeTheme = "theme";
        public const string AttributeWidth = "width";
        public const string AttributeHeight = "height";
        public const string AttributeBgColor = "bgColor";
        public const string AttributeIsShowText = "isShowText";
        public const string AttributeIsTopText = "isTopText";

        public const string ThemeStyle1 = "Style1";
        public const string ThemeStyle2 = "Style2";
        public const string ThemeStyle3 = "Style3";
        public const string ThemeStyle4 = "Style4";

        //对“flash滚动焦点图”（stl:focusViewer）元素进行解析
        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            // 如果是实体标签则返回空
            if (contextInfo.IsStlEntity)
            {
                return string.Empty;
            }

            var channelIndex = string.Empty;
            var channelName = string.Empty;
            var scopeType = EScopeType.Self;
            var groupChannel = string.Empty;
            var groupChannelNot = string.Empty;
            var groupContent = string.Empty;
            var groupContentNot = string.Empty;
            var tags = string.Empty;
            var orderByString = ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);
            var startNum = 1;
            var totalNum = 0;
            var isShowText = true;
            var isTopText = string.Empty;
            var titleWordNum = 0;
            var where = string.Empty;

            var isTop = false;
            var isTopExists = false;
            var isRecommend = false;
            var isRecommendExists = false;
            var isHot = false;
            var isHotExists = false;
            var isColor = false;
            var isColorExists = false;

            var theme = string.Empty;
            var imageWidth = 260;
            var imageHeight = 182;
            var textHeight = 25;
            var bgColor = string.Empty;
            var genericControl = new HtmlGenericControl("div");

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeChannelIndex))
                {
                    channelIndex = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeChannelName))
                {
                    channelName = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeScope))
                {
                    scopeType = EScopeTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupChannel))
                {
                    groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupChannelNot))
                {
                    groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupContent))
                {
                    groupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeGroupContentNot))
                {
                    groupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTags))
                {
                    tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeOrder))
                {
                    orderByString = StlDataUtility.GetContentOrderByString(pageInfo.SiteId, value, ETaxisType.OrderByTaxisDesc);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeStartNum))
                {
                    startNum = TranslateUtils.ToInt(value, 1);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTotalNum))
                {
                    totalNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTitleWordNum))
                {
                    titleWordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWhere))
                {
                    where = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsTop))
                {
                    isTopExists = true;
                    isTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsRecommend))
                {
                    isRecommendExists = true;
                    isRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsHot))
                {
                    isHotExists = true;
                    isHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsColor))
                {
                    isColorExists = true;
                    isColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTheme))
                {
                    theme = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWidth))
                {
                    if (StringUtils.EndsWithIgnoreCase(value, "px"))
                    {
                        value = value.Substring(0, value.Length - 2);
                    }
                    imageWidth = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeHeight))
                {
                    if (StringUtils.EndsWithIgnoreCase(value, "px"))
                    {
                        value = value.Substring(0, value.Length - 2);
                    }
                    imageHeight = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeBgColor))
                {
                    bgColor = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsShowText))
                {
                    isShowText = TranslateUtils.ToBool(value, true);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsTopText))
                {
                    isTopText = value;
                }
                else
                {
                    genericControl.Attributes[name] = value;
                }
            }

            return ParseImpl(pageInfo, contextInfo, genericControl, channelIndex, channelName, scopeType, groupChannel, groupChannelNot, groupContent, groupContentNot, tags, orderByString, startNum, totalNum, isShowText, isTopText, titleWordNum, where, isTop, isTopExists, isRecommend, isRecommendExists, isHot, isHotExists, isColor, isColorExists, theme, imageWidth, imageHeight, textHeight, bgColor);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, HtmlGenericControl genericControl, string channelIndex, string channelName, EScopeType scopeType, string groupChannel, string groupChannelNot, string groupContent, string groupContentNot, string tags, string orderByString, int startNum, int totalNum, bool isShowText, string isTopText, int titleWordNum, string where, bool isTop, bool isTopExists, bool isRecommend, bool isRecommendExists, bool isHot, bool isHotExists, bool isColor, bool isColorExists, string theme, int imageWidth, int imageHeight, int textHeight, string bgColor)
        {
            var parsedContent = string.Empty;

            var channelId = StlDataUtility.GetChannelIdByChannelIdOrChannelIndexOrChannelName(pageInfo.SiteId, contextInfo.ChannelId, channelIndex, channelName);

            var minContentInfoList = StlDataUtility.GetMinContentInfoList(pageInfo.SiteInfo, channelId, 0, groupContent, groupContentNot, tags, true, true, false, false, false, false, false, startNum, totalNum, orderByString, isTopExists, isTop, isRecommendExists, isRecommend, isHotExists, isHot, isColorExists, isColor, where, scopeType, groupChannel, groupChannelNot, null);

            if (minContentInfoList != null)
            {
                if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle2))
                {
                    pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcSwfObject);

                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.ImageUrl;

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            if (imageUrl.ToLower().EndsWith(".jpg") || imageUrl.ToLower().EndsWith(".jpeg") || imageUrl.ToLower().EndsWith(".png") || imageUrl.ToLower().EndsWith(".pneg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, pageInfo.IsLocal)));
                                imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, imageUrl, pageInfo.IsLocal)));
                            }
                        }
                    }

                    //foreach (var dataItem in dataSource)
                    //{
                    //    var contentInfo = new BackgroundContentInfo(dataItem);
                    //    if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
                    //    {
                    //        if (contentInfo.ImageUrl.ToLower().EndsWith(".jpg") || contentInfo.ImageUrl.ToLower().EndsWith(".jpeg") || contentInfo.ImageUrl.ToLower().EndsWith(".png") || contentInfo.ImageUrl.ToLower().EndsWith(".pneg"))
                    //        {
                    //            titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                    //            navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo)));
                    //            imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, contentInfo.ImageUrl)));
                    //        }
                    //    }
                    //}

                    if (string.IsNullOrEmpty(bgColor))
                    {
                        bgColor = "0xDADADA";
                    }
                    else
                    {
                        bgColor = bgColor.TrimStart('#');
                        if (!bgColor.StartsWith("0x"))
                        {
                            bgColor = "0x" + bgColor;
                        }
                    }

                    if (string.IsNullOrEmpty(isTopText))
                    {
                        isTopText = "0";
                    }
                    else
                    {
                        isTopText = (TranslateUtils.ToBool(isTopText)) ? "0" : "1";
                    }

                    var uniqueId = "FocusViewer_" + pageInfo.UniqueId;
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""quality"", ""high"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""wmode"", ""transparent"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""menu"", ""false"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""FlashVars"", ""bcastr_file=""+files_uniqueID+""&bcastr_link=""+links_uniqueID+""&bcastr_title=""+texts_uniqueID+""&AutoPlayTime=5&TitleBgPosition={isTopText}&TitleBgColor={bgColor}&BtnDefaultColor={bgColor}"");").Append(StringUtils.Constants.ReturnAndNewline);

                    string scriptHtml = $@"
<div id=""flashcontent_{uniqueId}""></div>
<script type=""text/javascript"">
var files_uniqueID='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}';
var links_uniqueID='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}';
var texts_uniqueID='{TranslateUtils.ObjectCollectionToString(titleCollection, "|")}';

var so_{uniqueId} = new SWFObject(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.Bcastr)}"", ""flash_{uniqueId}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{uniqueId}.write(""flashcontent_{uniqueId}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueId);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle3))
                {
                    pageInfo.AddPageBodyCodeIfNotExists(PageInfo.Const.JsAcSwfObject);

                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.GetString(BackgroundContentAttribute.ImageUrl);

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            if (imageUrl.ToLower().EndsWith(".jpg") || imageUrl.ToLower().EndsWith(".jpeg") || imageUrl.ToLower().EndsWith(".png") || imageUrl.ToLower().EndsWith(".pneg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, pageInfo.IsLocal)));
                                imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, imageUrl, pageInfo.IsLocal)));
                            }
                        }
                    }

                    //foreach (var dataItem in dataSource)
                    //{
                    //    var contentInfo = new BackgroundContentInfo(dataItem);
                    //    if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
                    //    {
                    //        if (contentInfo.ImageUrl.ToLower().EndsWith(".jpg") || contentInfo.ImageUrl.ToLower().EndsWith(".jpeg") || contentInfo.ImageUrl.ToLower().EndsWith(".png") || contentInfo.ImageUrl.ToLower().EndsWith(".pneg"))
                    //        {
                    //            titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                    //            navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo)));
                    //            imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, contentInfo.ImageUrl)));
                    //        }
                    //    }
                    //}

                    var uniqueId = "FocusViewer_" + pageInfo.UniqueId;
                    var paramBuilder = new StringBuilder();
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""quality"", ""high"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""wmode"", ""transparent"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""allowFullScreen"", ""true"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""allowScriptAccess"", ""always"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append($@"so_{uniqueId}.addParam(""menu"", ""false"");").Append(StringUtils.Constants.ReturnAndNewline);
                    paramBuilder.Append(
                        $@"so_{uniqueId}.addParam(""flashvars"", ""pw={imageWidth}&ph={imageHeight}&Times=4000&sizes=14&umcolor=16777215&btnbg=12189697&txtcolor=16777215&urls=""+urls_uniqueID+""&imgs=""+imgs_uniqueID+""&titles=""+titles_uniqueID);").Append(StringUtils.Constants.ReturnAndNewline);

                    string scriptHtml = $@"
<div id=""flashcontent_{uniqueId}""></div>
<script type=""text/javascript"">
var urls_uniqueID='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}';
var imgs_uniqueID='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}';
var titles_uniqueID='{TranslateUtils.ObjectCollectionToString(titleCollection, "|")}';

var so_{uniqueId} = new SWFObject(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.Ali)}"", ""flash_{uniqueId}"", ""{imageWidth}"", ""{imageHeight}"", ""7"", """");
{paramBuilder}
so_{uniqueId}.write(""flashcontent_{uniqueId}"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueId);

                    parsedContent = scriptHtml;
                }
                else if (StringUtils.EqualsIgnoreCase(theme, ThemeStyle4))
                {
                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.GetString(BackgroundContentAttribute.ImageUrl);

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            navigationUrls.Add(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, pageInfo.IsLocal));
                            imageUrls.Add(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, imageUrl, pageInfo.IsLocal));
                        }
                    }

                    //foreach (var dataItem in dataSource)
                    //{
                    //    var contentInfo = new BackgroundContentInfo(dataItem);
                    //    if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
                    //    {
                    //        navigationUrls.Add(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo));
                    //        imageUrls.Add(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, contentInfo.ImageUrl));
                    //    }
                    //}

                    var imageBuilder = new StringBuilder();
                    var numBuilder = new StringBuilder();

                    imageBuilder.Append($@"
<div style=""display:block""><a href=""{navigationUrls[0]}"" target=""_blank""><img src=""{imageUrls[0]}"" width=""{imageWidth}"" height=""{imageHeight}"" border=""0"" onMouseOver=""fv_clearAuto();"" onMouseOut=""fv_setAuto()"" /></a></div>
");

                    if (navigationUrls.Count > 1)
                    {
                        for (var i = 1; i < navigationUrls.Count; i++)
                        {
                            imageBuilder.Append($@"
<div style=""display:none""><a href=""{navigationUrls[i]}"" target=""_blank""><img src=""{imageUrls[i]}"" width=""{imageWidth}"" height=""{imageHeight}"" border=""0"" onMouseOver=""fv_clearAuto();"" onMouseOut=""fv_setAuto()"" /></a></div>
");
                        }
                    }

                    numBuilder.Append(@"
<td width=""18"" height=""16"" class=""fv_bigon"" onmouseover=""fv_clearAuto();fv_Mea(0);"" onmouseout=""fv_setAuto()"">1</td>
");

                    if (navigationUrls.Count > 1)
                    {
                        for (var i = 1; i < navigationUrls.Count; i++)
                        {
                            numBuilder.Append($@"
<td width=""18"" class=""fv_bigoff"" onmouseover=""fv_clearAuto();fv_Mea({i});"" onmouseout=""fv_setAuto()"">{i + 1}</td>
");
                        }
                    }

                    var bgUrl = PageUtility.ParseNavigationUrl(pageInfo.SiteInfo,
                        "@/images/focusviewerbg.png", pageInfo.IsLocal);
                    string scriptHtml = $@"
<style type=""text/css"">
.fv_adnum{{ position:absolute; z-index:1005; width:{imageWidth - 24}px; height:24px; padding:5px 3px 0 0; left:21px; top:{imageHeight -
                                                                                                                                                                                                  29}px; }}
.fv_bigon{{ background:#E59948; font-family:Arial; color:#fff; font-size:12px; text-align:center; cursor:pointer}}
.fv_bigoff{{ background:#7DCABD; font-family:Arial; color:#fff; font-size:12px; text-align:center; cursor:pointer}}
</style>
<div style=""position:relative; left:0; top:0; width:{imageWidth}px; height:{imageHeight}px; background:#000"">
	<div id=""fv_filbox"" style=""position:absolute; z-index:999; left:0; top:0; width:{imageWidth}px; height:{imageHeight}px; filter:progid:DXImageTransform.Microsoft.Fade( duration=0.5,overlap=1.0 );"">
		{imageBuilder}
    </div>
    <div class=""fv_adnum"" style=""background:url({bgUrl}) no-repeat !important; background:none ;filter: progid:DXImageTransform.Microsoft.AlphaImageLoader(enabled=true, src='{bgUrl}')""></div>
    <div class=""fv_adnum"">
        <table border=""0"" cellspacing=""1"" cellpadding=""0"" align=""right"" id=""fv_num"">
          <tr>
            {numBuilder}
          </tr>
        </table>
    </div>
</div>
<script language=""javascript""> 
    var fv_n=0;
    var fv_nums={navigationUrls.Count};
    var fv_showNum = document.getElementById(""fv_num"");
    var is_IE=(navigator.appName==""Microsoft Internet Explorer"");
    function fv_Mea(value){{
        fv_n=value;
        for(var i=0;i<fv_nums;i++)
            if(value==i){{
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('className', 'fv_bigon');
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('class', 'fv_bigon');
            }}
            else{{	
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('className', 'fv_bigoff');
                fv_showNum.getElementsByTagName(""td"")[i].setAttribute('class', 'fv_bigoff');
            }}
        var divs = document.getElementById(""fv_filbox"").getElementsByTagName(""div""); 
		if (is_IE){{
        	document.getElementById(""fv_filbox"").filters[0].Apply();
		}}
		for(i=0;i<fv_nums;i++)i==value?divs[i].style.display=""block"":divs[i].style.display=""none"";
		if (is_IE){{
			document.getElementById(""fv_filbox"").filters[0].play();
		}}
    }}
    function fv_clearAuto(){{clearInterval(autoStart)}}
    function fv_setAuto(){{autoStart=setInterval(""auto(fv_n)"", 5000)}}
    function auto(){{
        fv_n++;
        if(fv_n>(fv_nums-1))fv_n=0;
        fv_Mea(fv_n);
    }}
    fv_setAuto(); 
</script>
";

                    parsedContent = scriptHtml.Replace("fv_", $"fv{pageInfo.UniqueId}_");
                }
                else
                {
                    var imageUrls = new StringCollection();
                    var navigationUrls = new StringCollection();
                    var titleCollection = new StringCollection();

                    foreach (var minContentInfo in minContentInfoList)
                    {
                        var contentInfo = ContentManager.GetContentInfo(pageInfo.SiteInfo, minContentInfo.ChannelId, minContentInfo.Id);
                        var imageUrl = contentInfo.ImageUrl;

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            //这里使用png图片不管用
                            //||contentInfo.ImageUrl.ToLower().EndsWith(".png")||contentInfo.ImageUrl.ToLower().EndsWith(".pneg")
                            if (imageUrl.ToLower().EndsWith(".jpg") || imageUrl.ToLower().EndsWith(".jpeg"))
                            {
                                titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                                navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo, pageInfo.IsLocal)));
                                imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, imageUrl, pageInfo.IsLocal)));
                            }
                        }
                    }

                    //foreach (var dataItem in dataSource)
                    //{
                    //    var contentInfo = new BackgroundContentInfo(dataItem);
                    //    if (!string.IsNullOrEmpty(contentInfo?.ImageUrl))
                    //    {
                    //        //这里使用png图片不管用
                    //        //||contentInfo.ImageUrl.ToLower().EndsWith(".png")||contentInfo.ImageUrl.ToLower().EndsWith(".pneg")
                    //        if (contentInfo.ImageUrl.ToLower().EndsWith(".jpg") || contentInfo.ImageUrl.ToLower().EndsWith(".jpeg"))
                    //        {
                    //            titleCollection.Add(StringUtils.ToJsString(PageUtils.UrlEncode(StringUtils.MaxLengthText(StringUtils.StripTags(contentInfo.Title), titleWordNum))));
                    //            navigationUrls.Add(PageUtils.UrlEncode(PageUtility.GetContentUrl(pageInfo.SiteInfo, contentInfo)));
                    //            imageUrls.Add(PageUtils.UrlEncode(PageUtility.ParseNavigationUrl(pageInfo.SiteInfo, contentInfo.ImageUrl)));
                    //        }
                    //    }
                    //}

                    if (string.IsNullOrEmpty(bgColor))
                    {
                        bgColor = "#DADADA";
                    }
                    var titles = string.Empty;
                    if (isShowText == false)
                    {
                        textHeight = 0;
                    }
                    else
                    {
                        titles = TranslateUtils.ObjectCollectionToString(titleCollection, "|");
                    }
                    var uniqueId = "FocusViewer_" + pageInfo.UniqueId;
                    genericControl.ID = uniqueId;
                    genericControl.InnerHtml = "&nbsp;";
                    var divHtml = ControlUtils.GetControlRenderHtml(genericControl);
                    string scriptHtml = $@"
<script type=""text/javascript"" src=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.BaiRongFlash.Js)}""></script>
<script type=""text/javascript"">
	var uniqueID_focus_width={imageWidth}
	var uniqueID_focus_height={imageHeight}
	var uniqueID_text_height={textHeight}
	var uniqueID_swf_height = uniqueID_focus_height + uniqueID_text_height
	
	var uniqueID_pics='{TranslateUtils.ObjectCollectionToString(imageUrls, "|")}'
	var uniqueID_links='{TranslateUtils.ObjectCollectionToString(navigationUrls, "|")}'
	var uniqueID_texts='{titles}'
	
	var uniqueID_FocusFlash = new bairongFlash(""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.Flashes.FocusViewer)}"", ""focusflash"", uniqueID_focus_width, uniqueID_swf_height, ""7"", ""{bgColor}"", false, ""High"");
	uniqueID_FocusFlash.addParam(""allowScriptAccess"", ""sameDomain"");
	uniqueID_FocusFlash.addParam(""menu"", ""false"");
	uniqueID_FocusFlash.addParam(""wmode"", ""transparent"");

	uniqueID_FocusFlash.addVariable(""pics"", uniqueID_pics);
	uniqueID_FocusFlash.addVariable(""links"", uniqueID_links);
	uniqueID_FocusFlash.addVariable(""texts"", uniqueID_texts);
	uniqueID_FocusFlash.addVariable(""borderwidth"", uniqueID_focus_width);
	uniqueID_FocusFlash.addVariable(""borderheight"", uniqueID_focus_height);
	uniqueID_FocusFlash.addVariable(""textheight"", uniqueID_text_height);
	uniqueID_FocusFlash.write(""uniqueID"");
</script>
";
                    scriptHtml = scriptHtml.Replace("uniqueID", uniqueId);

                    parsedContent = divHtml + scriptHtml;
                }
            }

            return parsedContent;
        }
    }
}