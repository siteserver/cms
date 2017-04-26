using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.User;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public class WebUtils
    {
        public static string GetContentTitle(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, string pageUrl)
        {
            string url;
            string displayString;
            var title = TextUtility.FormatTitle(contentInfo.Attributes[BackgroundContentAttribute.TitleFormatString], contentInfo.Title);

            if (TranslateUtils.ToBool(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.IsColor)))
            {
                displayString = $"<span style='color:#ff0000;text-decoration:none' title='醒目'>{title}</span>";
            }
            else
            {
                displayString = title;
            }

            if (contentInfo.NodeId < 0)
            {
                url = displayString;
            }
            else if (contentInfo.IsChecked)
            {
                url =
                    $"<a href='{PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo, true)}' target='blank'>{displayString}</a>";
            }
            else
            {
                url =
                    $"<a href='{PageContentView.GetContentViewUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, contentInfo.Id, pageUrl)}'>{displayString}</a>";
            }

            var image = string.Empty;
            if (TranslateUtils.ToBool(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.IsRecommend)))
            {
                image += "&nbsp;<img src='../pic/icon/recommend.gif' title='推荐' align='absmiddle' border=0 />";
            }
            if (TranslateUtils.ToBool(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.IsHot)))
            {
                image += "&nbsp;<img src='../pic/icon/hot.gif' title='热点' align='absmiddle' border=0 />";
            }
            if (TranslateUtils.ToBool(contentInfo.GetExtendedAttribute(ContentAttribute.IsTop)))
            {
                image += "&nbsp;<img src='../pic/icon/top.gif' title='置顶' align='absmiddle' border=0 />";
            }
            if (contentInfo.ReferenceId > 0)
            {
                if (contentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) == ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用内容' align='absmiddle' border=0 />(引用内容)";
                }
                else if (contentInfo.GetExtendedAttribute(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
                {
                    image += "&nbsp;<img src='../pic/icon/reference.png' title='引用地址' align='absmiddle' border=0 />(引用地址)";
                }
            }
            if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/link.png' title='外部链接' align='absmiddle' border=0 />";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl)))
            {
                var imageUrl = PageUtility.ParseNavigationUrl(publishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl));
                var openWindowString = ModalMessage.GetOpenWindowString("预览图片", $"<img src='{imageUrl}' />", 500, 500);
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../assets/icons/img.gif' title='预览图片' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl)))
            {
                var openWindowString = ModalMessage.GetOpenWindowStringToPreviewVideoByUrl(publishmentSystemInfo.PublishmentSystemId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.VideoUrl));
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../pic/icon/video.png' title='预览视频' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl)))
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

        public static string GetChannelListBoxTitle(int publishmentSystemID, int nodeID, string nodeName, ENodeType nodeType, int parentsCount, bool isLastNode, bool[] isLastNodeArray)
        {
            var str = string.Empty;
            if (nodeID == publishmentSystemID)
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
                if (isLastNodeArray[i])
                {
                    str = string.Concat(str, "　");
                }
                else
                {
                    str = string.Concat(str, "│");
                }
            }
            if (isLastNode)
            {
                str = string.Concat(str, "└");
            }
            else
            {
                str = string.Concat(str, "├");
            }
            str = string.Concat(str, StringUtils.MaxLengthText(nodeName, 8));

            return str;
        }

        private static string GetContentAddUrl(EContentModelType modelType, int publishmentSystemId, int nodeId, string returnUrl)
        {
            if (modelType == EContentModelType.GovPublic)
            {
                return PageGovPublicContentAdd.GetRedirectUrlOfAdd(publishmentSystemId, nodeId, returnUrl);
            }
            if (modelType == EContentModelType.Vote)
            {
                return PageVoteContentAdd.GetRedirectUrlOfAdd(publishmentSystemId, nodeId, returnUrl);
            }
            return PageContentAdd.GetRedirectUrlOfAdd(publishmentSystemId, nodeId, returnUrl);
        }

        private static string GetContentEditUrl(EContentModelType modelType, int publishmentSystemId, int nodeId, int id, string returnUrl)
        {
            if (modelType == EContentModelType.GovPublic)
            {
                return PageGovPublicContentAdd.GetRedirectUrlOfEdit(publishmentSystemId, nodeId, id, returnUrl);
            }
            if (modelType == EContentModelType.Vote)
            {
                return PageVoteContentAdd.GetRedirectUrlOfEdit(publishmentSystemId, nodeId, id, returnUrl);
            }
            return PageContentAdd.GetRedirectUrlOfEdit(publishmentSystemId, nodeId, id, returnUrl);
        }

        public static string GetContentAddUploadWordUrl(int publishmentSystemId, NodeInfo nodeInfo, bool isFirstLineTitle, bool isFirstLineRemove, bool isClearFormat, bool isFirstLineIndent, bool isClearFontSize, bool isClearFontFamily, bool isClearImages, int contentLevel, string fileName, string returnUrl)
        {
            var modelType = EContentModelTypeUtils.GetEnumType(nodeInfo.ContentModelId);
            return
                $"{GetContentAddUrl(modelType, publishmentSystemId, nodeInfo.NodeId, returnUrl)}&isUploadWord=True&isFirstLineTitle={isFirstLineTitle}&isFirstLineRemove={isFirstLineRemove}&isClearFormat={isClearFormat}&isFirstLineIndent={isFirstLineIndent}&isClearFontSize={isClearFontSize}&isClearFontFamily={isClearFontFamily}&isClearImages={isClearImages}&contentLevel={contentLevel}&fileName={fileName}";
        }

        public static string GetContentAddAddUrl(int publishmentSystemId, NodeInfo nodeInfo, string returnUrl)
        {
            var modelType = EContentModelTypeUtils.GetEnumType(nodeInfo.ContentModelId);
            return GetContentAddUrl(modelType, publishmentSystemId, nodeInfo.NodeId, returnUrl);
        }

        public static string GetContentAddEditUrl(int publishmentSystemId, NodeInfo nodeInfo, int id, string returnUrl)
        {
            var modelType = EContentModelTypeUtils.GetEnumType(nodeInfo.ContentModelId);
            return GetContentEditUrl(modelType, publishmentSystemId, nodeInfo.NodeId, id, returnUrl);
        }

        public static string GetContentCommands(string administratorName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string pageUrl, string currentFileName, bool isCheckPage)
        {
            var iconUrl = SiteServerAssets.GetIconUrl(string.Empty);
            var modelType = EContentModelTypeUtils.GetEnumType(nodeInfo.ContentModelId);

            var builder = new StringBuilder();
            //添加内容
            if (!isCheckPage && AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentAdd) && nodeInfo.Additional.IsContentAddable)
            {
                var redirectUrl = GetContentAddAddUrl(publishmentSystemInfo.PublishmentSystemId, nodeInfo, pageUrl);
                var title = "添加内容";
                if (modelType == EContentModelType.GovPublic)
                {
                    title = "采集信息";
                }
                else if (modelType == EContentModelType.GovInteract)
                {
                    title = "新增办件";
                }
                else if (modelType == EContentModelType.Photo)
                {
                    title = "添加图片";
                }
                else if (modelType == EContentModelType.Vote)
                {
                    title = "发起投票";
                }

                builder.Append(
                    $@"<a href=""{redirectUrl}""><img style=""margin-right: 3px"" src=""{iconUrl}/add.gif"" align=""absMiddle"" />{title}</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");

                builder.Append($@"<a href=""javascript:;"" onclick=""{ModalContentImport.GetOpenWindowString(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">导入内容</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");

                if (modelType != EContentModelType.UserDefined && modelType != EContentModelType.Vote && modelType != EContentModelType.Job && modelType != EContentModelType.GovInteract)
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalUploadWord.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, StringUtils.ValueToUrl(pageUrl))}"">导入Word</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
            }
            //删 除
            if (nodeInfo.ContentNum > 0 && AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentDelete))
            {
                builder.Append(
                    $@"<a href=""javascript:;"" onclick=""{PageContentDelete.GetRedirectClickStringForSingleChannel(
                        publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, false, pageUrl)}"">删 除</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
            }

            if (nodeInfo.ContentNum > 0)
            {
                builder.Append(
                    $@"<a href=""javascript:;"" onclick=""{ModalContentExport.GetOpenWindowString(
                        publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">导 出</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                //设置
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentEdit))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalContentAttributes.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">设置属性</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalAddToGroup.GetOpenWindowStringToContent(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">设置内容组</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //转 移
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentTranslate))
                {
                    var redirectUrl = PageContentTranslate.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl);

                    var clickString = PageUtils.GetRedirectStringWithCheckBoxValue(redirectUrl, "ContentIDCollection", "ContentIDCollection", "请选择需要转移的内容！");

                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{clickString}"">转 移</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //排 序
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentEdit))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalContentTaxis.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">排 序</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //整理
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentOrder))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalContentTidyUp.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">整 理</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //审 核
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentCheck))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalContentCheck.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">审 核</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //归 档
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentArchive))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalContentArchive.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">归 档</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //跨站转发
                if (CrossSiteTransUtility.IsTranslatable(publishmentSystemInfo, nodeInfo))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalContentCrossSiteTrans.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId)}"">跨站转发</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //生 成
                if (!isCheckPage && (AdminUtility.HasWebsitePermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, AppManager.Cms.Permission.WebSite.Create) || AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.CreatePage)))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{Cms.ModalProgressBar
                            .GetOpenWindowStringWithCreateContentsOneByOne(publishmentSystemInfo.PublishmentSystemId,
                                nodeInfo.NodeId)}"">生 成</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
            }

            //选择显示项
            //if (nodeInfo.NodeType != ENodeType.BackgroundImageNode)
            //{
            if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelEdit))
            {
                builder.Append(
                    $@"<a href=""javascript:;"" onclick=""{ModalSelectColumns.GetOpenWindowStringToContent(
                        publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, true)}"">显示项</a> &nbsp; &nbsp; ");
            }
            //}

            if (!isCheckPage && nodeInfo.ContentNum > 0)
            {
                if (builder.Length > 0)
                {
                    builder.Length = builder.Length - 15;
                }

                //builder.Append(GetContentLinks(publishmentSystemInfo, nodeInfo, contentType, currentFileName));

                builder.Append(
                    $@"&nbsp; <a href=""javascript:;;"" onClick=""$('#contentSearch').toggle(); return false""><img src=""{iconUrl}/search.gif"" align=""absMiddle"" alt=""快速查找"" /></a>");
            }


            //if (builder.Length > 0)
            //{
            //    builder.Length = builder.Length - 16;
            //}
            return builder.ToString();
        }

        public static string GetChannelCommands(string administratorName, PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string pageUrl, string currentFileName)
        {
            var iconUrl = SiteServerAssets.GetIconUrl(string.Empty);
            var builder = new StringBuilder();
            //添加栏目
            if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelAdd) && nodeInfo.Additional.IsChannelAddable)
            {
                builder.Append(
                    $@"<a href=""{PageChannelAdd.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId,
                        nodeInfo.NodeId, pageUrl)}""><img style=""MARGIN-RIGHT: 3px"" src=""{iconUrl}/add.gif"" align=""absMiddle"" />添加栏目</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                builder.Append(
                    $@"<a href=""javascript:;"" onclick=""{ModalChannelAdd.GetOpenWindowString(
                        publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, pageUrl)}"">快速添加</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
            }
            if (nodeInfo.ChildrenCount > 0)
            {
                //删除栏目
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelDelete))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelDelete.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, pageUrl),
                            "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的栏目！")}"">删除栏目</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //清空内容
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ContentDelete))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelDelete.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId, pageUrl),
                            "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除内容的栏目！")}"">清空内容</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelAdd))
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
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelEdit))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalAddToGroup.GetOpenWindowStringToChannel(
                            publishmentSystemInfo.PublishmentSystemId)}"">设置栏目组</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //转 移
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelTranslate))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelTranslate.GetRedirectUrl(publishmentSystemInfo.PublishmentSystemId,
                                nodeInfo.NodeId, pageUrl), "ChannelIDCollection", "ChannelIDCollection", "请选择需要转移的栏目！")}"">转 移</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                //生 成
                if (AdminUtility.HasWebsitePermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, AppManager.Cms.Permission.WebSite.Create) || AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.CreatePage))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalCreateChannels.GetOpenWindowString(
                            publishmentSystemInfo.PublishmentSystemId)}"">生 成</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
            }
            else
            {
                //导 入
                if (AdminUtility.HasChannelPermissions(administratorName, publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelAdd) && nodeInfo.Additional.IsChannelAddable)
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
            var builder = new StringBuilder();

            builder.Append(
                $@"<div class=""btn_word"" onclick=""{ModalTextEditorImportWord.GetOpenWindowString(
                    publishmentSystemInfo.PublishmentSystemId, attributeName)}"">导入Word</div>");

            builder.Append(
                $@"<div class=""btn_video"" onclick=""{ModalTextEditorInsertVideo.GetOpenWindowString(
                    publishmentSystemInfo.PublishmentSystemId, attributeName)}"">插入视频</div>");

            builder.Append(
                $@"<div class=""btn_audio"" onclick=""{ModalTextEditorInsertAudio.GetOpenWindowString(
                    publishmentSystemInfo.PublishmentSystemId, attributeName)}"">插入音频</div>");

            var command = @"<div class=""btn_keywords"" onclick=""getWordSpliter();"">提取关键字</div>
<script type=""text/javascript"">
function getWordSpliter(){
    var pureText = [getPureText]
	$.post('[url]&r=' + Math.random(), {content:pureText}, function(data) {
		if(data !=''){
			$('#Tags').val(data).focus();
		}else{
            [tips]
        }
	});	
}
</script>
";
            command = command.Replace("[url]", AjaxCmsService.GetWordSpliterUrl(publishmentSystemInfo.PublishmentSystemId));
            command = command.Replace("[getPureText]", ETextEditorTypeUtils.GetPureTextScript(attributeName));
            command = command.Replace("[tips]", PageUtils.GetOpenTipsString("对不起，内容不足，无法提取关键字", PageUtils.TipsError));

            builder.Append(command);

            command = @"<div class=""btn_detection"" onclick=""detection_[attributeName]();"">敏感词检测</div>
<script type=""text/javascript"">
function detection_[attributeName](){
    var pureText = [getPureText]
    var htmlContent = [getContent]
    var keyword = '';
	$.post('[url]&r=' + Math.random(), {content:pureText}, function(data) {
        debugger;
		if(data){
			var arr = data.split(',');
            var i=0;
			for(;i<arr.length;i++)
			{
                var reg = new RegExp(arr[i], 'gi');
				htmlContent = htmlContent.replace(reg,'<span style=""background-color:#ffff00;"">' + arr[i] + '</span>');
			}
            keyword=data;
			[setContent]
            [tips_warn]
		}else{
            [tips_success]
        }
	});	
}
</script>
";
            command = command.Replace("[attributeName]", attributeName);
            command = command.Replace("[url]", AjaxCmsService.GetDetectionUrl(publishmentSystemInfo.PublishmentSystemId));
            command = command.Replace("[getPureText]", ETextEditorTypeUtils.GetPureTextScript(attributeName));
            command = command.Replace("[getContent]", ETextEditorTypeUtils.GetContentScript(attributeName));
            command = command.Replace("[setContent]", ETextEditorTypeUtils.GetSetContentScript(attributeName, "htmlContent"));
            command = command.Replace("[tips_warn]", PageUtils.GetOpenTipsString("共检测到' + i + '个敏感词，内容已用黄色背景标明", PageUtils.TipsWarn));
            command = command.Replace("[tips_success]", PageUtils.GetOpenTipsString("检测成功，没有检测到任何敏感词", PageUtils.TipsSuccess));
            builder.Append(command);

            return builder.ToString();
        }

        public static string GetAutoCheckKeywordsScript(PublishmentSystemInfo publishmentSystemInfo)
        {
            var builder = new StringBuilder();

            var command = @"
<script type=""text/javascript"">
var bairongKeywordArray;
function autoCheckKeywords(){
    if([isAutoCheckKeywords]){
        var pureText = [getPureText]
        var htmlContent = [getContent]
	    $.post('[url]&r=' + Math.random(), {content:pureText}, function(data) {
		    if(data){
                bairongKeywordArray = data;
			    var arr = data.split(',');
                var i=0;
			    for(;i<arr.length;i++)
			    {
                    var tmpArr = arr[i].split('|');
                    var keyword = tmpArr[0];
                    var replace = tmpArr.length==2?tmpArr[1]:'';
                    var reg = new RegExp(keyword, 'gi');
				    htmlContent = htmlContent.replace(reg,'<span style=""background-color:#ffff00;"">' + keyword + '</span>');
			    }
			    [setContent]
                [tips_warn]
		    }else{
                $('#BtnSubmit').attr('onclick', '').click();
            }
	    });
        return false;	
    }
}
function autoReplaceKeywords(){
    var arr = bairongKeywordArray.split(',');
    var i=0;
    var htmlContent = [getContent]
	for(;i<arr.length;i++)
	{
        var tmpArr = arr[i].split('|');
        var keyword = tmpArr[0];
        var replace = tmpArr.length==2?tmpArr[1]:'';
        var reg = new RegExp('<span style=""background-color:#ffff00;"">' + keyword + '</span>', 'gi');
		htmlContent = htmlContent.replace(reg, replace);
        //IE8
        reg = new RegExp('<span style=""background-color:#ffff00"">' + keyword + '</span>', 'gi');
		htmlContent = htmlContent.replace(reg, replace);
	}
    [setContent]
    $('#BtnSubmit').attr('onclick', '').click();
}
</script>
";
            command = command.Replace("[isAutoCheckKeywords]",
                $"{publishmentSystemInfo.Additional.IsAutoCheckKeywords.ToString().ToLower()}");
            command = command.Replace("[url]", AjaxCmsService.GetDetectionReplaceUrl(publishmentSystemInfo.PublishmentSystemId));
            command = command.Replace("[getPureText]", ETextEditorTypeUtils.GetPureTextScript(BackgroundContentAttribute.Content));
            command = command.Replace("[getContent]", ETextEditorTypeUtils.GetContentScript(BackgroundContentAttribute.Content));
            command = command.Replace("[setContent]", ETextEditorTypeUtils.GetSetContentScript(BackgroundContentAttribute.Content, "htmlContent"));
            
            command = command.Replace("[tips_warn]", PageUtils.GetOpenTipsString("内容中共检测到' + i + '个敏感词，已用黄色背景标明", PageUtils.TipsWarn, false, "自动替换并保存", "autoReplaceKeywords"));
            builder.Append(command);

            return builder.ToString();
        }
    }
}
