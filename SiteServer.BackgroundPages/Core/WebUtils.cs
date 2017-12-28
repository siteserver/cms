using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using ModalExportMessage = SiteServer.BackgroundPages.Cms.ModalExportMessage;

namespace SiteServer.BackgroundPages.Core
{
    public class WebUtils
    {
        public static string GetContentTitle(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, string pageUrl)
        {
            string url;
            var title = ContentUtility.FormatTitle(contentInfo.GetString(BackgroundContentAttribute.TitleFormatString), contentInfo.Title);

            var displayString = contentInfo.IsTop ? $"<span style='color:#ff0000;text-decoration:none' title='醒目'>{title}</span>" : title;

            if (contentInfo.NodeId < 0)
            {
                url = displayString;
            }
            else if (contentInfo.IsChecked)
            {
                url =
                    $"<a href='{PageRedirect.GetRedirectUrlToContent(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id)}' target='blank'>{displayString}</a>";
            }
            else
            {
                url =
                    $"<a href='{PageContentView.GetContentViewUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl)}'>{displayString}</a>";
            }

            var image = string.Empty;
            if (contentInfo.IsRecommend)
            {
                image += "&nbsp;<img src='../pic/icon/recommend.gif' title='推荐' align='absmiddle' border=0 />";
            }
            if (contentInfo.IsHot)
            {
                image += "&nbsp;<img src='../pic/icon/hot.gif' title='热点' align='absmiddle' border=0 />";
            }
            if (contentInfo.IsTop)
            {
                image += "&nbsp;<img src='../pic/icon/top.gif' title='置顶' align='absmiddle' border=0 />";
            }
            if (contentInfo.ReferenceId > 0)
            {
                if (contentInfo.GetString(ContentAttribute.TranslateContentType) == ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用内容' align='absmiddle' border=0 />(引用内容)";
                }
                else if (contentInfo.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用地址' align='absmiddle' border=0 />(引用地址)";
                }
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(ContentAttribute.LinkUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/link.png' title='外部链接' align='absmiddle' border=0 />";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.ImageUrl)))
            {
                var imageUrl = PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentInfo.GetString(BackgroundContentAttribute.ImageUrl), true);
                var openWindowString = ModalMessage.GetOpenWindowString("预览图片", $"<img src='{imageUrl}' />", 500, 500);
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../assets/icons/img.gif' title='预览图片' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.VideoUrl)))
            {
                var openWindowString = ModalMessage.GetOpenWindowStringToPreviewVideoByUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.GetString(BackgroundContentAttribute.VideoUrl));
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../pic/icon/video.png' title='预览视频' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.FileUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/attachment.gif' title='附件' align='absmiddle' border=0 />";
                if (publishmentSystemInfo.Additional.IsCountDownload)
                {
                    var count = CountManager.GetCount(publishmentSystemInfo.AuxiliaryTableForContent, contentInfo.Id.ToString(), ECountType.Download);
                    image += $"下载次数:<strong>{count}</strong>";
                }
            }
            if (!string.IsNullOrEmpty(contentInfo.WritingUserName))
            {
                var openWindowString = ModalUserView.GetOpenWindowString(contentInfo.WritingUserName);
                image +=
                    $@"&nbsp;（<a href=""javascript:;"" onclick=""{openWindowString}"">投稿用户:{contentInfo.WritingUserName}</a>）";
            }
            return url + image;
        }

        public static string GetChannelListBoxTitle(int publishmentSystemId, int nodeId, string nodeName, int parentsCount, bool isLastNode, bool[] isLastNodeArray)
        {
            var str = string.Empty;
            if (nodeId == publishmentSystemId)
            {
                isLastNode = true;
            }
            if (isLastNode == false)
            {
                isLastNodeArray[parentsCount] = false;
            }
            else
            {
                isLastNodeArray[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, isLastNodeArray[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, StringUtils.MaxLengthText(nodeName, 8));

            return str;
        }

        public static string GetContentAddUploadWordUrl(int publishmentSystemId, NodeInfo nodeInfo, bool isFirstLineTitle, bool isFirstLineRemove, bool isClearFormat, bool isFirstLineIndent, bool isClearFontSize, bool isClearFontFamily, bool isClearImages, int contentLevel, string fileName, string returnUrl)
        {
            return
                $"{PageContentAdd.GetRedirectUrlOfAdd(publishmentSystemId, nodeInfo.NodeId, returnUrl)}&isUploadWord=True&isFirstLineTitle={isFirstLineTitle}&isFirstLineRemove={isFirstLineRemove}&isClearFormat={isClearFormat}&isFirstLineIndent={isFirstLineIndent}&isClearFontSize={isClearFontSize}&isClearFontFamily={isClearFontFamily}&isClearImages={isClearImages}&contentLevel={contentLevel}&fileName={fileName}";
        }

        public static string GetContentAddAddUrl(int publishmentSystemId, NodeInfo nodeInfo, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfAdd(publishmentSystemId, nodeInfo.NodeId, returnUrl);
        }

        public static string GetContentAddEditUrl(int publishmentSystemId, NodeInfo nodeInfo, int id, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfEdit(publishmentSystemId, nodeInfo.NodeId, id, returnUrl);
        }

        public static string GetContentCommands(string administratorName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string pageUrl)
        {
            var builder = new StringBuilder();

            if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentAdd) && nodeInfo.Additional.IsContentAddable)
            {
                builder.Append($@"
<a href=""{GetContentAddAddUrl(publishmentSystemInfo.PublishmentSystemId, nodeInfo, pageUrl)}"" class=""btn btn-primary"">
    <i class=""ion-plus""></i>
    添加
</a>");

                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{ModalUploadWord.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, StringUtils.ValueToUrl(pageUrl))}"">
    导入Word
</a>");
            }

            if (nodeInfo.ContentNum > 0 && AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentDelete))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{PageContentDelete.GetRedirectClickStringForSingleChannel(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, false, pageUrl)}"">
    <i class=""fa fa-trash-o""></i>
    删 除
</a>");
            }

            if (nodeInfo.ContentNum > 0)
            {
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentEdit))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{ModalContentAttributes.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">
    <i class=""fa fa-tag""></i>
    属性
</a>");
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{ModalAddToGroup.GetOpenWindowStringToContent(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">
    内容组
</a>");
                }
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentTranslate))
                {
                    var redirectUrl = PageContentTranslate.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl);
                    var clickString = PageUtils.GetRedirectStringWithCheckBoxValue(redirectUrl, "ContentIDCollection", "ContentIDCollection", "请选择需要转移的内容！");
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{clickString}"">
    转 移
</a>");
                }
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentEdit))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{ModalContentTaxis.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">
    排 序
</a>");
                }
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentCheck))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{ModalContentCheck.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">
    审 核
</a>");
                }
                if (AdminUtility.HasSitePermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, AppManager.Permissions.WebSite.Create) || AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.CreatePage))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateContentsOneByOne(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">
    生 成
</a>");
                }
            }

            if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelEdit))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-primary"" onclick=""{ModalSelectColumns.GetOpenWindowStringToContent(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, true)}"">
    <i class=""fa fa-exclamation-circle""></i>
    显示项
</a>");
            }

            if (nodeInfo.ContentNum > 0)
            {
                builder.Append(@"
<a href=""javascript:;;"" class=""btn btn-primary"" onClick=""$('#contentSearch').toggle(); return false"">
    <i class=""ion-search""></i>
    查找
</a>");
            }

            return builder.ToString();
        }

        public static string GetContentMoreCommands(string administratorName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string pageUrl)
        {
            var builder = new StringBuilder();

            if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentAdd) && nodeInfo.Additional.IsContentAddable)
            {
                builder.Append($@"
<li>
    <a href=""javascript:;"" onclick=""{ModalContentImport.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">
        导 入
    </a>
</li>");
            }

            if (nodeInfo.ContentNum > 0)
            {
                builder.Append($@"
<li>
    <a href=""javascript:;"" onclick=""{ModalContentExport.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">
        导 出
    </a>
</li>");
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentOrder))
                {
                    builder.Append($@"
<li>
    <a href=""javascript:;"" onclick=""{ModalContentTidyUp.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">
        整 理
    </a>
</li>");
                }
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentArchive))
                {
                    builder.Append($@"
<li>
    <a href=""javascript:;"" onclick=""{ModalContentArchive.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">
        归 档
    </a>
</li>");
                }
                if (CrossSiteTransUtility.IsCrossSiteTrans(publishmentSystemInfo, nodeInfo) && !CrossSiteTransUtility.IsAutomatic(nodeInfo))
                {
                    builder.Append($@"
<li>
    <a href=""javascript:;"" onclick=""{ModalContentCrossSiteTrans.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">
        跨站转发
    </a>
</li>");
                }
            }

            return builder.ToString();
        }

        public static string GetChannelCommands(string administratorName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string pageUrl, string currentFileName)
        {
            var iconUrl = SiteServerAssets.GetIconUrl(string.Empty);
            var builder = new StringBuilder();
            //添加栏目
            if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelAdd) && nodeInfo.Additional.IsChannelAddable)
            {
                builder.Append(
                    $@"<a href=""{PageChannelAdd.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId,
                        nodeInfo.NodeId, pageUrl)}""><img style=""MARGIN-RIGHT: 3px"" src=""{iconUrl}/add.gif"" align=""absMiddle"" />添加栏目</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                builder.Append(
                    $@"<a href=""javascript:;"" onclick=""{ModalChannelsAdd.GetOpenWindowString(
                        publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">快速添加</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
            }
            if (nodeInfo.ChildrenCount > 0)
            {
                //删除栏目
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelDelete))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelDelete.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, pageUrl),
                            "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的栏目！")}"">删除栏目</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //清空内容
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ContentDelete))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelDelete.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, pageUrl),
                            "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除内容的栏目！")}"">清空内容</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelAdd))
                {
                    //导 入
                    if (nodeInfo.Additional.IsChannelAddable)
                    {
                        builder.Append(
                           $@"<a href=""javascript:;"" onclick=""{ModalChannelImport.GetOpenWindowString(
                               publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">导 入</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                    }
                    //导 出
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalExportMessage.GetOpenWindowStringToChannel(
                            publishmentSystemInfo.PublishmentSystemId, "ChannelIDCollection", "请选择需要导出的栏目！")}"">导 出</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                //设置栏目组
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelEdit))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalAddToGroup.GetOpenWindowStringToChannel(
                            publishmentSystemInfo.PublishmentSystemId)}"">设置栏目组</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //转 移
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelTranslate))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelTranslate.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId,
                                nodeInfo.NodeId, pageUrl), "ChannelIDCollection", "ChannelIDCollection", "请选择需要转移的栏目！")}"">转 移</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                //生 成
                if (AdminUtility.HasSitePermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, AppManager.Permissions.WebSite.Create) || AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.CreatePage))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalCreateChannels.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId)}"">生 成</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
            }
            else
            {
                //导 入
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelAdd) && nodeInfo.Additional.IsChannelAddable)
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalChannelImport.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">导 入</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
            }
            if (publishmentSystemInfo.PublishmentSystemId != nodeInfo.NodeId)
            {
                builder.Append(
                    $@"<a href=""{$"{currentFileName}?PublishmentSystemID={publishmentSystemInfo.PublishmentSystemId}&NodeID={nodeInfo.ParentId}"}""><img style=""MARGIN-RIGHT: 3px"" src=""{iconUrl}/upfolder.gif"" align=""absMiddle"" />向 上</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
            }
            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 15;
            }
            return builder.ToString();
        }

        public static string GetTextEditorCommands(PublishmentSystemInfo publishmentSystemInfo, string attributeName)
        {
            return $@"
<script type=""text/javascript"">
function getWordSpliter(){{
    var pureText = {ETextEditorTypeUtils.GetPureTextScript(attributeName)}
	$.post('{AjaxCmsService.GetWordSpliterUrl(publishmentSystemInfo.PublishmentSystemId)}&r=' + Math.random(), {{content:pureText}}, function(data) {{
		if(data !=''){{
			$('#Tags').val(data).focus();
		}}else{{
            {PageUtils.GetOpenTipsString("对不起，内容不足，无法提取关键字", PageUtils.TipsError)}
        }}
	}});	
}}
function detection_{attributeName}(){{
    var pureText = {ETextEditorTypeUtils.GetPureTextScript(attributeName)}
    var htmlContent = {ETextEditorTypeUtils.GetContentScript(attributeName)}
    var keyword = '';
	$.post('{AjaxCmsService.GetDetectionUrl(publishmentSystemInfo.PublishmentSystemId)}&r=' + Math.random(), {{content:pureText}}, function(data) {{
        debugger;
		if(data){{
			var arr = data.split(',');
            var i=0;
			for(;i<arr.length;i++)
			{{
                var reg = new RegExp(arr[i], 'gi');
				htmlContent = htmlContent.replace(reg,'<span style=""background-color:#ffff00;"">' + arr[i] + '</span>');
			}}
            keyword=data;
			{ETextEditorTypeUtils.GetSetContentScript(attributeName, "htmlContent")}
            {PageUtils.GetOpenTipsString("共检测到' + i + '个敏感词，内容已用黄色背景标明", PageUtils.TipsWarn)}
		}} else {{
            {PageUtils.GetOpenTipsString("检测成功，没有检测到任何敏感词", PageUtils.TipsSuccess)}
        }}
	}});	
}}
</script>
<div class=""btn-group"">
    <button class=""btn"" onclick=""{ModalTextEditorImportWord.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">导入Word</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertVideo.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">插入视频</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertAudio.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">插入音频</button>
    <button class=""btn"" onclick=""getWordSpliter();"">提取关键字</button>
    <button class=""btn"" onclick=""detection_{attributeName}();"">敏感词检测</button>
</div>
";
        }

        public static string GetAutoCheckKeywordsScript(PublishmentSystemInfo publishmentSystemInfo)
        {
            var isAutoCheckKeywords = publishmentSystemInfo.Additional.IsAutoCheckKeywords.ToString().ToLower();
            var url = AjaxCmsService.GetDetectionReplaceUrl(publishmentSystemInfo.PublishmentSystemId);
            var getPureText = ETextEditorTypeUtils.GetPureTextScript(BackgroundContentAttribute.Content);
            var getContent = ETextEditorTypeUtils.GetContentScript(BackgroundContentAttribute.Content);
            var setContent = ETextEditorTypeUtils.GetSetContentScript(BackgroundContentAttribute.Content, "htmlContent");
            var tipsWarn = PageUtils.GetOpenTipsString("内容中共检测到' + i + '个敏感词，已用黄色背景标明", PageUtils.TipsWarn, false, "自动替换并保存", "autoReplaceKeywords");

            var command = $@"
<script type=""text/javascript"">
var bairongKeywordArray;
function autoCheckKeywords() {{
    if({isAutoCheckKeywords}) {{
        var pureText = {getPureText}
        var htmlContent = {getContent}
	    $.post('{url}&r=' + Math.random(), {{content:pureText}}, function(data) {{
		    if(data) {{
                bairongKeywordArray = data;
			    var arr = data.split(',');
                var i=0;
			    for(;i<arr.length;i++)
			    {{
                    var tmpArr = arr[i].split('|');
                    var keyword = tmpArr[0];
                    var replace = tmpArr.length==2?tmpArr[1]:'';
                    var reg = new RegExp(keyword, 'gi');
				    htmlContent = htmlContent.replace(reg,'<span style=""background-color:#ffff00;"">' + keyword + '</span>');
			    }}
			    {setContent}
                {tipsWarn}
		    }} else {{
                $('#BtnSubmit').attr('onclick', '').click();
            }}
	    }});
        return false;	
    }}
}}
function autoReplaceKeywords() {{
    var arr = bairongKeywordArray.split(',');
    var i=0;
    var htmlContent = {getContent}
	for(;i<arr.length;i++)
	{{
        var tmpArr = arr[i].split('|');
        var keyword = tmpArr[0];
        var replace = tmpArr.length==2?tmpArr[1]:'';
        var reg = new RegExp('<span style=""background-color:#ffff00;"">' + keyword + '</span>', 'gi');
		htmlContent = htmlContent.replace(reg, replace);
        //IE8
        reg = new RegExp('<span style=""background-color:#ffff00"">' + keyword + '</span>', 'gi');
		htmlContent = htmlContent.replace(reg, replace);
	}}
    {setContent}
    $('#BtnSubmit').attr('onclick', '').click();
}}
</script>
";
            


            return command;
        }

        public static string GetImageUrlButtonGroupHtml(PublishmentSystemInfo publishmentSystemInfo, string attributeName)
        {
            return $@"
<div class=""btn-group"">
    <button class=""btn"" onclick=""{ModalUploadImage.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectImage.GetOpenWindowString(publishmentSystemInfo, attributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalCuttingImage.GetOpenWindowStringWithTextBox(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        裁切
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewImage(publishmentSystemInfo.PublishmentSystemId, attributeName)}"">
        预览
    </button>
</div>
";
        }
    }
}
