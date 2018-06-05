using System.Text;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public static class WebUtils
    {
        public static string GetContentTitle(SiteInfo siteInfo, ContentInfo contentInfo, string pageUrl)
        {
            string url;
            var title = ContentUtility.FormatTitle(contentInfo.GetString(BackgroundContentAttribute.TitleFormatString), contentInfo.Title);

            var displayString = contentInfo.IsColor ? $"<span style='color:#ff0000;text-decoration:none' title='醒目'>{title}</span>" : title;

            if (contentInfo.ChannelId < 0)
            {
                url = displayString;
            }
            else if (contentInfo.IsChecked)
            {
                url =
                    $"<a href='{PageRedirect.GetRedirectUrlToContent(siteInfo.Id, contentInfo.ChannelId, contentInfo.Id)}' target='blank'>{displayString}</a>";
            }
            else
            {
                url =
                    $"<a href='{PageContentView.GetContentViewUrl(siteInfo.Id, contentInfo.ChannelId, contentInfo.Id, pageUrl)}'>{displayString}</a>";
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
                var imageUrl = PageUtility.ParseNavigationUrl(siteInfo, contentInfo.GetString(BackgroundContentAttribute.ImageUrl), true);
                var openWindowString = ModalMessage.GetOpenWindowString(siteInfo.Id, "预览图片", $"<img src='{imageUrl}' />", 500, 500);
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../assets/icons/img.gif' title='预览图片' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.VideoUrl)))
            {
                var openWindowString = ModalMessage.GetOpenWindowStringToPreviewVideoByUrl(siteInfo.Id, contentInfo.GetString(BackgroundContentAttribute.VideoUrl));
                image +=
                    $@"&nbsp;<a href=""javascript:;"" onclick=""{openWindowString}""><img src='../pic/icon/video.png' title='预览视频' align='absmiddle' border=0 /></a>";
            }
            if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.FileUrl)))
            {
                image += "&nbsp;<img src='../pic/icon/attachment.gif' title='附件' align='absmiddle' border=0 />";
                if (siteInfo.Additional.IsCountDownload)
                {
                    var count = CountManager.GetCount(siteInfo.TableName, contentInfo.Id.ToString(), ECountType.Download);
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

        public static string GetChannelListBoxTitle(int siteId, int channelId, string nodeName, int parentsCount, bool isLastNode, bool[] isLastNodeArray)
        {
            var str = string.Empty;
            if (channelId == siteId)
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

        public static string GetContentAddUploadWordUrl(int siteId, ChannelInfo nodeInfo, bool isFirstLineTitle, bool isFirstLineRemove, bool isClearFormat, bool isFirstLineIndent, bool isClearFontSize, bool isClearFontFamily, bool isClearImages, int contentLevel, string fileName, string returnUrl)
        {
            return
                $"{PageContentAdd.GetRedirectUrlOfAdd(siteId, nodeInfo.Id, returnUrl)}&isUploadWord=True&isFirstLineTitle={isFirstLineTitle}&isFirstLineRemove={isFirstLineRemove}&isClearFormat={isClearFormat}&isFirstLineIndent={isFirstLineIndent}&isClearFontSize={isClearFontSize}&isClearFontFamily={isClearFontFamily}&isClearImages={isClearImages}&contentLevel={contentLevel}&fileName={fileName}";
        }

        public static string GetContentAddAddUrl(int siteId, ChannelInfo nodeInfo, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfAdd(siteId, nodeInfo.Id, returnUrl);
        }

        public static string GetContentAddEditUrl(int siteId, ChannelInfo nodeInfo, int id, string returnUrl)
        {
            return PageContentAdd.GetRedirectUrlOfEdit(siteId, nodeInfo.Id, id, returnUrl);
        }

        public static string GetContentCommands(PermissionManager permissionManager, SiteInfo siteInfo, ChannelInfo nodeInfo, string pageUrl)
        {
            var builder = new StringBuilder();

            if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && nodeInfo.Additional.IsContentAddable)
            {
                builder.Append($@"
<a href=""{GetContentAddAddUrl(siteInfo.Id, nodeInfo, pageUrl)}"" class=""btn btn-light text-secondary"">
    <i class=""ion-plus""></i>
    添加
</a>");

                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalUploadWord.GetOpenWindowString(siteInfo.Id, nodeInfo.Id, StringUtils.ValueToUrl(pageUrl))}"">
    导入Word
</a>");
            }

            if (nodeInfo.ContentNum > 0 && permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentDelete))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{PageContentDelete.GetRedirectClickStringForSingleChannel(siteInfo.Id, nodeInfo.Id, false, pageUrl)}"">
    <i class=""ion-trash-a""></i>
    删 除
</a>");
            }

            if (nodeInfo.ContentNum > 0)
            {
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentEdit))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalContentAttributes.GetOpenWindowString(siteInfo.Id, nodeInfo.Id)}"">
    <i class=""ion-flag""></i>
    属性
</a>");
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalAddToGroup.GetOpenWindowStringToContent(siteInfo.Id, nodeInfo.Id)}"">
    内容组
</a>");
                }
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentTranslate))
                {
                    var redirectUrl = PageContentTranslate.GetRedirectUrl(siteInfo.Id, nodeInfo.Id, pageUrl);
                    var clickString = PageUtils.GetRedirectStringWithCheckBoxValue(redirectUrl, "contentIdCollection", "contentIdCollection", "请选择需要转移的内容！");
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{clickString}"">
    转 移
</a>");
                }
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentEdit))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalContentTaxis.GetOpenWindowString(siteInfo.Id, nodeInfo.Id, pageUrl)}"">
    排 序
</a>");
                }
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentCheck))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalContentCheck.GetOpenWindowString(siteInfo.Id, nodeInfo.Id, pageUrl)}"">
    审 核
</a>");
                }
                if (permissionManager.HasSitePermissions(siteInfo.Id, ConfigManager.WebSitePermissions.Create) || permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.CreatePage))
                {
                    builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalProgressBar.GetOpenWindowStringWithCreateContentsOneByOne(siteInfo.Id, nodeInfo.Id)}"">
    <i class=""ion-wand""></i>
    生 成
</a>");
                }
            }

            if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit))
            {
                builder.Append($@"
<a href=""javascript:;"" class=""btn btn-light text-secondary"" onclick=""{ModalSelectColumns.GetOpenWindowString(siteInfo.Id, nodeInfo.Id, true)}"">
    <i class=""ion-ios-list-outline""></i>
    显示项
</a>");
            }

            if (nodeInfo.ContentNum > 0)
            {
                builder.Append(@"
<a href=""javascript:;;"" class=""btn btn-light text-secondary text-secondary"" onClick=""$('#contentSearch').toggle(); return false"">
    <i class=""ion-search""></i>
    查找
</a>");
            }

            return builder.ToString();
        }

        public static string GetContentMoreCommands(PermissionManager permissionManager, SiteInfo siteInfo, ChannelInfo nodeInfo, string pageUrl)
        {
            var builder = new StringBuilder();

            if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentAdd) && nodeInfo.Additional.IsContentAddable)
            {
                builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentImport.GetOpenWindowString(siteInfo.Id, nodeInfo.Id)}"">
    导 入
</a>");
            }

            if (nodeInfo.ContentNum > 0)
            {
                builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentExport.GetOpenWindowString(siteInfo.Id, nodeInfo.Id)}"">
    导 出
</a>");
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentOrder))
                {
                    builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentTidyUp.GetOpenWindowString(siteInfo.Id, nodeInfo.Id, pageUrl)}"">
    整 理
</a>");
                }
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentArchive))
                {
                    builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentArchive.GetOpenWindowString(siteInfo.Id, nodeInfo.Id, pageUrl)}"">
    归 档
</a>");
                }
                if (CrossSiteTransUtility.IsCrossSiteTrans(siteInfo, nodeInfo) && !CrossSiteTransUtility.IsAutomatic(nodeInfo))
                {
                    builder.Append($@"
<a class=""dropdown-item"" href=""javascript:;"" onclick=""{ModalContentCrossSiteTrans.GetOpenWindowString(siteInfo.Id, nodeInfo.Id)}"">
    跨站转发
</a>");
                }
            }

            return builder.ToString();
        }

        public static string GetChannelCommands(PermissionManager permissionManager, SiteInfo siteInfo, ChannelInfo nodeInfo, string pageUrl, string currentFileName)
        {
            var iconUrl = SiteServerAssets.GetIconUrl(string.Empty);
            var builder = new StringBuilder();
            //添加栏目
            if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ChannelAdd) && nodeInfo.Additional.IsChannelAddable)
            {
                builder.Append(
                    $@"<a href=""{PageChannelAdd.GetRedirectUrl(siteInfo.Id,
                        nodeInfo.Id, pageUrl)}""><img style=""MARGIN-RIGHT: 3px"" src=""{iconUrl}/add.gif"" align=""absMiddle"" />添加栏目</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                builder.Append(
                    $@"<a href=""javascript:;"" onclick=""{ModalChannelsAdd.GetOpenWindowString(
                        siteInfo.Id, nodeInfo.Id, pageUrl)}"">快速添加</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
            }
            if (nodeInfo.ChildrenCount > 0)
            {
                //删除栏目
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ChannelDelete))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelDelete.GetRedirectUrl(siteInfo.Id, pageUrl),
                            "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除的栏目！")}"">删除栏目</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //清空内容
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ContentDelete))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelDelete.GetRedirectUrl(siteInfo.Id, pageUrl),
                            "ChannelIDCollection", "ChannelIDCollection", "请选择需要删除内容的栏目！")}"">清空内容</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ChannelAdd))
                {
                    //导 入
                    if (nodeInfo.Additional.IsChannelAddable)
                    {
                        builder.Append(
                           $@"<a href=""javascript:;"" onclick=""{ModalChannelImport.GetOpenWindowString(
                               siteInfo.Id, nodeInfo.Id)}"">导 入</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                    }
                    //导 出
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{Cms.ModalExportMessage.GetOpenWindowStringToChannel(
                            siteInfo.Id, "ChannelIDCollection", "请选择需要导出的栏目！")}"">导 出</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                //设置栏目组
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ChannelEdit))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalAddToGroup.GetOpenWindowStringToChannel(
                            siteInfo.Id)}"">设置栏目组</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
                //转 移
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ChannelTranslate))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{PageUtils.GetRedirectStringWithCheckBoxValue(
                            PageChannelTranslate.GetRedirectUrl(siteInfo.Id,
                                nodeInfo.Id, pageUrl), "ChannelIDCollection", "ChannelIDCollection", "请选择需要转移的栏目！")}"">转 移</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }

                //生 成
                if (permissionManager.HasSitePermissions(siteInfo.Id, ConfigManager.WebSitePermissions.Create) || permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.CreatePage))
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalCreateChannels.GetOpenWindowString(
                            siteInfo.Id)}"">生 成</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
            }
            else
            {
                //导 入
                if (permissionManager.HasChannelPermissions(siteInfo.Id, nodeInfo.Id, ConfigManager.ChannelPermissions.ChannelAdd) && nodeInfo.Additional.IsChannelAddable)
                {
                    builder.Append(
                        $@"<a href=""javascript:;"" onclick=""{ModalChannelImport.GetOpenWindowString(
                            siteInfo.Id, nodeInfo.Id)}"">导 入</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
                }
            }
            if (siteInfo.Id != nodeInfo.Id)
            {
                builder.Append(
                    $@"<a href=""{$"{currentFileName}?siteId={siteInfo.Id}&channelId={nodeInfo.ParentId}"}""><img style=""MARGIN-RIGHT: 3px"" src=""{iconUrl}/upfolder.gif"" align=""absMiddle"" />向 上</a> <span class=""gray"">&nbsp;|&nbsp;</span> ");
            }
            if (builder.Length > 0)
            {
                builder.Length = builder.Length - 15;
            }
            return builder.ToString();
        }

        public static string GetTextEditorCommands(SiteInfo siteInfo, string attributeName)
        {
            return $@"
<script type=""text/javascript"">
function getWordSpliter(){{
    var pureText = {ETextEditorTypeUtils.GetPureTextScript(attributeName)}
	$.post('{AjaxCmsService.GetWordSpliterUrl(siteInfo.Id)}&r=' + Math.random(), {{content:pureText}}, function(data) {{
		if(data !=''){{
            $('.nav-pills').children('li').eq(1).find('a').click();
			$('#TbTags').val(data).focus();
		}}else{{
            {AlertUtils.Error("提取关键字", "对不起，内容不足，无法提取关键字")}
        }}
	}});	
}}
function detection_{attributeName}(){{
    var pureText = {ETextEditorTypeUtils.GetPureTextScript(attributeName)}
    var htmlContent = {ETextEditorTypeUtils.GetContentScript(attributeName)}
    var keyword = '';
	$.post('{AjaxCmsService.GetDetectionUrl(siteInfo.Id)}&r=' + Math.random(), {{content:pureText}}, function(data) {{
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
            {AlertUtils.Warning("敏感词检测", "共检测到' + i + '个敏感词，内容已用黄色背景标明", "取 消", string.Empty, string.Empty)}
		}} else {{
            {AlertUtils.Success("敏感词检测", "检测成功，没有检测到任何敏感词")}
        }}
	}});	
}}
</script>
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalTextEditorImportWord.GetOpenWindowString(siteInfo.Id, attributeName)}"">导入Word</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertImage.GetOpenWindowString(siteInfo.Id, attributeName)}"">插入图片</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertVideo.GetOpenWindowString(siteInfo.Id, attributeName)}"">插入视频</button>
    <button class=""btn"" onclick=""{ModalTextEditorInsertAudio.GetOpenWindowString(siteInfo.Id, attributeName)}"">插入音频</button>
    <button class=""btn"" onclick=""getWordSpliter();return false;"">提取关键字</button>
    <button class=""btn"" onclick=""detection_{attributeName}();return false;"">敏感词检测</button>
</div>
";
        }

        public static string GetAutoCheckKeywordsScript(SiteInfo siteInfo)
        {
            var isAutoCheckKeywords = siteInfo.Additional.IsAutoCheckKeywords.ToString().ToLower();
            var url = AjaxCmsService.GetDetectionReplaceUrl(siteInfo.Id);
            var getPureText = ETextEditorTypeUtils.GetPureTextScript(BackgroundContentAttribute.Content);
            var getContent = ETextEditorTypeUtils.GetContentScript(BackgroundContentAttribute.Content);
            var setContent = ETextEditorTypeUtils.GetSetContentScript(BackgroundContentAttribute.Content, "htmlContent");
            var tipsWarn = AlertUtils.Warning("敏感词检测", "内容中共检测到' + i + '个敏感词，已用黄色背景标明", "取 消", "自动替换并保存",
                "autoReplaceKeywords");

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

        public static string GetImageUrlButtonGroupHtml(SiteInfo siteInfo, string attributeName)
        {
            return $@"
<div class=""btn-group btn-group-sm"">
    <button class=""btn"" onclick=""{ModalUploadImage.GetOpenWindowString(siteInfo.Id, attributeName)}"">
        上传
    </button>
    <button class=""btn"" onclick=""{ModalSelectImage.GetOpenWindowString(siteInfo, attributeName)}"">
        选择
    </button>
    <button class=""btn"" onclick=""{ModalCuttingImage.GetOpenWindowStringWithTextBox(siteInfo.Id, attributeName)}"">
        裁切
    </button>
    <button class=""btn"" onclick=""{ModalMessage.GetOpenWindowStringToPreviewImage(siteInfo.Id, attributeName)}"">
        预览
    </button>
</div>
";
        }
    }
}
