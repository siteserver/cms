using System;
using SiteServer.CMS.Model;
using System.Text;
using SiteServer.Utils;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using System.Linq;
using SiteServer.CMS.DataCache.Content;

namespace SiteServer.CMS.Core
{
    public static class ContentUtility
    {
        public static string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页占位符

        public static string TextEditorContentEncode(SiteInfo siteInfo, string content)
        {
            if (siteInfo == null) return content;
            
            if (siteInfo.Additional.IsSaveImageInTextEditor && !string.IsNullOrEmpty(content))
            {
                content = PathUtility.SaveImage(siteInfo, content);
            }

            var builder = new StringBuilder(content);

            var url = siteInfo.Additional.WebUrl;
            if (!string.IsNullOrEmpty(url) && url != "/")
            {
                StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            }
            //if (!string.IsNullOrEmpty(url))
            //{
            //    StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            //}

            var relatedSiteUrl = PageUtils.ParseNavigationUrl($"~/{siteInfo.SiteDir}");
            StringUtils.ReplaceHrefOrSrc(builder, relatedSiteUrl, "@");

            builder.Replace("@'@", "'@");
            builder.Replace("@\"@", "\"@");

            return builder.ToString();
        }

        public static string TextEditorContentDecode(SiteInfo siteInfo, string content, bool isLocal)
        {
            if (siteInfo == null) return content;
            
            var builder = new StringBuilder(content);

            var virtualAssetsUrl = $"@/{siteInfo.Additional.AssetsDir}";
            string assetsUrl;
            if (isLocal)
            {
                assetsUrl = PageUtility.GetSiteUrl(siteInfo,
                    siteInfo.Additional.AssetsDir, true);
            }
            else
            {
                assetsUrl = siteInfo.Additional.AssetsUrl;
            }
            StringUtils.ReplaceHrefOrSrc(builder, virtualAssetsUrl, assetsUrl);
            StringUtils.ReplaceHrefOrSrc(builder, "@/", siteInfo.Additional.WebUrl);
            StringUtils.ReplaceHrefOrSrc(builder, "@", siteInfo.Additional.WebUrl);

            builder.Replace("&#xa0;", "&nbsp;");

            return builder.ToString();
        }

        public static string GetTitleFormatString(bool isStrong, bool isEm, bool isU, string color)
        {
            return $"{isStrong}_{isEm}_{isU}_{color}";
        }

        public static bool SetTitleFormatControls(string titleFormatString, CheckBox formatStrong, CheckBox formatEm, CheckBox formatU, TextBox formatColor)
        {
            var isTitleFormatted = false;
            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    formatStrong.Checked = TranslateUtils.ToBool(formats[0]);
                    formatEm.Checked = TranslateUtils.ToBool(formats[1]);
                    formatU.Checked = TranslateUtils.ToBool(formats[2]);
                    formatColor.Text = formats[3];
                    if (formatStrong.Checked || formatEm.Checked || formatU.Checked || !string.IsNullOrEmpty(formatColor.Text))
                    {
                        isTitleFormatted = true;
                    }
                }
            }
            return isTitleFormatted;
        }

        public static void SetTitleFormatControls(string titleFormatString, out bool formatStrong, out bool formatEm, out bool formatU, out string formatColor)
        {
            formatStrong = formatEm = formatU = false;
            formatColor = string.Empty;

            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    formatStrong = TranslateUtils.ToBool(formats[0]);
                    formatEm = TranslateUtils.ToBool(formats[1]);
                    formatU = TranslateUtils.ToBool(formats[2]);
                    formatColor = formats[3];
                }
            }
        }

        public static string FormatTitle(string titleFormatString, string title)
        {
            var formattedTitle = title;
            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    var isStrong = TranslateUtils.ToBool(formats[0]);
                    var isEm = TranslateUtils.ToBool(formats[1]);
                    var isU = TranslateUtils.ToBool(formats[2]);
                    var color = formats[3];

                    if (!string.IsNullOrEmpty(color))
                    {
                        if (!color.StartsWith("#"))
                        {
                            color = "#" + color;
                        }
                        formattedTitle = $@"<span style=""color:{color}"">{formattedTitle}</span>";
                    }
                    if (isStrong)
                    {
                        formattedTitle = $"<strong>{formattedTitle}</strong>";
                    }
                    if (isEm)
                    {
                        formattedTitle = $"<em>{formattedTitle}</em>";
                    }
                    if (isU)
                    {
                        formattedTitle = $"<u>{formattedTitle}</u>";
                    }
                }
            }
            return formattedTitle;
        }

        public static void PutImagePaths(SiteInfo siteInfo, ContentInfo contentInfo, NameValueCollection collection)
        {
            if (contentInfo == null) return;

            var imageUrl = contentInfo.GetString(BackgroundContentAttribute.ImageUrl);
            var videoUrl = contentInfo.GetString(BackgroundContentAttribute.VideoUrl);
            var fileUrl = contentInfo.GetString(BackgroundContentAttribute.FileUrl);
            var content = contentInfo.GetString(BackgroundContentAttribute.Content);

            if (!string.IsNullOrEmpty(imageUrl) && PageUtility.IsVirtualUrl(imageUrl))
            {
                collection[imageUrl] = PathUtility.MapPath(siteInfo, imageUrl);
            }
            if (!string.IsNullOrEmpty(videoUrl) && PageUtility.IsVirtualUrl(videoUrl))
            {
                collection[videoUrl] = PathUtility.MapPath(siteInfo, videoUrl);
            }
            if (!string.IsNullOrEmpty(fileUrl) && PageUtility.IsVirtualUrl(fileUrl))
            {
                collection[fileUrl] = PathUtility.MapPath(siteInfo, fileUrl);
            }

            var srcList = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var src in srcList)
            {
                if (PageUtility.IsVirtualUrl(src))
                {
                    collection[src] = PathUtility.MapPath(siteInfo, src);
                }
            }
        }

        public static string GetAutoPageContent(string content, int pageWordNum)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Replace(PagePlaceHolder, string.Empty);
                AutoPage(builder, content, pageWordNum);
            }
            return builder.ToString();
        }

        private static void AutoPage(StringBuilder builder, string content, int pageWordNum)
        {
            if (content.Length > pageWordNum)
            {
                var i = content.IndexOf("</P>", pageWordNum, StringComparison.Ordinal);
                if (i == -1)
                {
                    i = content.IndexOf("</p>", pageWordNum, StringComparison.Ordinal);
                }

                if (i != -1)
                {
                    var start = i + 4;
                    builder.Append(content.Substring(0, start));
                    content = content.Substring(start);
                    if (!string.IsNullOrEmpty(content))
                    {
                        builder.Append(PagePlaceHolder);
                        AutoPage(builder, content, pageWordNum);
                    }
                }
                else
                {
                    builder.Append(content);
                }
            }
            else
            {
                builder.Append(content);
            }
        }

        public static List<TableStyleInfo> GetAllTableStyleInfoList(List<TableStyleInfo> tableStyleInfoList)
        {
            var taxis = 1;
            var list = new List<TableStyleInfo>
            {
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Id,
                    DisplayName = "内容Id",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Title,
                    DisplayName = "标题",
                    Taxis = taxis++
                }
            };

            if (tableStyleInfoList != null)
            {
                foreach (var tableStyleInfo in tableStyleInfoList)
                {
                    if (!list.Exists(t => t.AttributeName == tableStyleInfo.AttributeName))
                    {
                        list.Add(new TableStyleInfo
                        {
                            AttributeName = tableStyleInfo.AttributeName,
                            DisplayName = tableStyleInfo.DisplayName,
                            InputType = tableStyleInfo.InputType,
                            Taxis = taxis++
                        });
                    }
                }
            }

            list.AddRange(new List<TableStyleInfo>
            {
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.LinkUrl,
                    DisplayName = "外部链接",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.AddDate,
                    DisplayName = "添加时间",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.LastEditDate,
                    DisplayName = "最后修改时间",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.AddUserName,
                    DisplayName = "添加人",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.LastEditUserName,
                    DisplayName = "最后修改人",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.GroupNameCollection,
                    DisplayName = "内容组",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Tags,
                    DisplayName = "标签",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.AdminId,
                    DisplayName = "管理员",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.UserId,
                    DisplayName = "投稿用户",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.SourceId,
                    DisplayName = "来源标识",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Hits,
                    DisplayName = "点击量",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.HitsByDay,
                    DisplayName = "日点击",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.HitsByWeek,
                    DisplayName = "周点击",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.HitsByMonth,
                    DisplayName = "月点击",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.LastHitsDate,
                    DisplayName = "最后点击时间",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.CheckUserName,
                    DisplayName = "审核人",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.CheckDate,
                    DisplayName = "审核时间",
                    Taxis = taxis++
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.CheckReasons,
                    DisplayName = "审核原因",
                    Taxis = taxis
                },
            });

            return list.OrderBy(styleInfo => styleInfo.Taxis == 0 ? int.MaxValue : styleInfo.Taxis).ToList();
        }

        public static List<TableStyleInfo> GetEditableTableStyleInfoList(List<TableStyleInfo> tableStyleInfoList)
        {
            var list = new List<TableStyleInfo>
            {
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Title,
                    InputType = InputType.Text,
                    DisplayName = "标题"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.LinkUrl,
                    InputType = InputType.Text,
                    DisplayName = "外部链接"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.AddDate,
                    InputType = InputType.DateTime,
                    DisplayName = "添加时间"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.GroupNameCollection,
                    InputType = InputType.CheckBox,
                    DisplayName = "内容组"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Tags,
                    InputType = InputType.CheckBox,
                    DisplayName = "标签"
                }
            };

            if (tableStyleInfoList != null)
            {
                list.InsertRange(2, tableStyleInfoList);
            }

            return list;
        }

        public static bool AfterContentAdded(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, bool isCrossSiteTrans, bool isAutomatic)
        {
            var isTranslated = false;
            if (isCrossSiteTrans && isAutomatic)
            {
                var targetSiteId = 0;

                if (channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    targetSiteId = channelInfo.Additional.TransSiteId;
                }
                else if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                {
                    targetSiteId = siteInfo.Id;
                }
                else if (channelInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
                {
                    targetSiteId = SiteManager.GetParentSiteId(siteInfo.Id);
                }

                if (targetSiteId > 0)
                {
                    var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
                    if (targetSiteInfo != null)
                    {
                        var targetChannelIdArrayList = TranslateUtils.StringCollectionToIntList(channelInfo.Additional.TransChannelIds);
                        if (targetChannelIdArrayList.Count > 0)
                        {
                            foreach (var targetChannelId in targetChannelIdArrayList)
                            {
                                CrossSiteTransUtility.TransContentInfo(siteInfo, channelInfo, contentId, targetSiteInfo, targetChannelId);
                                isTranslated = true;
                            }
                        }
                    }
                }
            }

            foreach (var service in PluginManager.Services)
            {
                try
                {
                    service.OnContentAddCompleted(new ContentEventArgs(siteInfo.Id, channelInfo.Id, contentId));
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentAddCompleted));
                }
            }

            return isTranslated;
        }

        public static void Translate(SiteInfo siteInfo, int channelId, int contentId, string translateCollection, ETranslateContentType translateType, string administratorName)
        {
            var translateList = TranslateUtils.StringCollectionToStringList(translateCollection);
            foreach (var translate in translateList)
            {
                if (string.IsNullOrEmpty(translate)) continue;

                var translates = translate.Split('_');
                if (translates.Length != 2) continue;

                var targetSiteId = TranslateUtils.ToInt(translates[0]);
                var targetChannelId = TranslateUtils.ToInt(translates[1]);

                Translate(siteInfo, channelId, contentId, targetSiteId, targetChannelId, translateType);
            }
        }

        public static void Translate(SiteInfo siteInfo, int channelId, int contentId, int targetSiteId, int targetChannelId, ETranslateContentType translateType)
        {
            if (siteInfo == null || channelId <= 0 || contentId <= 0 || targetSiteId <= 0 || targetChannelId <= 0) return;

            var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
            var targetChannelInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);
            var targetTableName = ChannelManager.GetTableName(targetSiteInfo, targetChannelInfo);

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);

            if (contentInfo == null) return;

            if (translateType == ETranslateContentType.Copy)
            {
                FileUtility.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.Cut)
            {
                FileUtility.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());

                var newContentId = DataProvider.ContentDao.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);
                DataProvider.ContentDao.DeleteContents(siteInfo.Id, tableName, TranslateUtils.ToIntList(contentId), channelId);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, newContentId));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }

                    try
                    {
                        service.OnContentDeleteCompleted(new ContentEventArgs(siteInfo.Id, channelInfo.Id, contentId));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentDeleteCompleted));
                    }
                }

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, newContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.Reference)
            {
                if (contentInfo.ReferenceId != 0) return;

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                int theContentId = DataProvider.ContentDao.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.ReferenceContent)
            {
                if (contentInfo.ReferenceId != 0) return;

                FileUtility.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetSiteInfo, targetChannelInfo, contentInfo);

                foreach (var service in PluginManager.Services)
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(siteInfo.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                CreateManager.CreateContent(targetSiteInfo.Id, contentInfo.ChannelId, theContentId);
                CreateManager.TriggerContentChangedEvent(targetSiteInfo.Id, contentInfo.ChannelId);
            }
        }

        public static Dictionary<int, List<int>> GetIDsDictionary(NameValueCollection queryString)
        {
            var dic = new Dictionary<int, List<int>>();

            if (!string.IsNullOrEmpty(queryString["IDsCollection"]))
            {
                foreach (var ids in TranslateUtils.StringCollectionToStringList(queryString["IDsCollection"]))
                {
                    var channelId = TranslateUtils.ToIntWithNagetive(ids.Split('_')[0]);
                    var contentId = TranslateUtils.ToInt(ids.Split('_')[1]);
                    var contentIdList = new List<int>();
                    if (dic.ContainsKey(channelId))
                    {
                        contentIdList = dic[channelId];
                    }
                    if (!contentIdList.Contains(contentId))
                    {
                        contentIdList.Add(contentId);
                    }

                    dic[channelId] = contentIdList;
                }
            }
            else
            {
                var channelId = TranslateUtils.ToInt(queryString["channelId"]);
                dic[channelId] = TranslateUtils.StringCollectionToIntList(queryString["contentIdCollection"]);
            }

            return dic;
        }

        public static string GetTitleHtml(string titleFormat, string titleAjaxUrl)
        {
            var builder = new StringBuilder();
            var formatStrong = false;
            var formatEm = false;
            var formatU = false;
            var formatColor = string.Empty;
            if (titleFormat != null)
            {
                SetTitleFormatControls(titleFormat, out formatStrong, out formatEm, out formatU, out formatColor);
            }

            builder.Append(
                $@"<a class=""btn"" href=""javascript:;"" onclick=""$('#div_{ContentAttribute.Title}').toggle();return false;""><i class=""icon-text-height""></i></a>
<script type=""text/javascript"">
function {ContentAttribute.Title}_strong(e){{
var e = $(e);
if ($('#{ContentAttribute.Title}_formatStrong').val() == 'true'){{
$('#{ContentAttribute.Title}_formatStrong').val('false');
e.removeClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_formatStrong').val('true');
e.addClass('btn-success');
}}
}}
function {ContentAttribute.Title}_em(e){{
var e = $(e);
if ($('#{ContentAttribute.Title}_formatEM').val() == 'true'){{
$('#{ContentAttribute.Title}_formatEM').val('false');
e.removeClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_formatEM').val('true');
e.addClass('btn-success');
}}
}}
function {ContentAttribute.Title}_u(e){{
var e = $(e);
if ($('#{ContentAttribute.Title}_formatU').val() == 'true'){{
$('#{ContentAttribute.Title}_formatU').val('false');
e.removeClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_formatU').val('true');
e.addClass('btn-success');
}}
}}
function {ContentAttribute.Title}_color(){{
if ($('#{ContentAttribute.Title}_formatColor').val()){{
$('#{ContentAttribute.Title}_colorBtn').css('color', $('#{ContentAttribute.Title}_formatColor').val());
$('#{ContentAttribute.Title}_colorBtn').addClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_colorBtn').css('color', '');
$('#{ContentAttribute.Title}_colorBtn').removeClass('btn-success');
}}
$('#{ContentAttribute.Title}_colorContainer').hide();
}}
</script>
");

            builder.Append($@"
<div class=""btn-group btn-group-sm"" style=""float:left;"">
    <button class=""btn{(formatStrong ? @" btn-success" : string.Empty)}"" style=""font-weight:bold"" onclick=""{ContentAttribute.Title}_strong(this);return false;"">粗体</button>
    <button class=""btn{(formatEm ? " btn-success" : string.Empty)}"" style=""font-style:italic"" onclick=""{ContentAttribute.Title}_em(this);return false;"">斜体</button>
    <button class=""btn{(formatU ? " btn-success" : string.Empty)}"" style=""text-decoration:underline"" onclick=""{ContentAttribute.Title}_u(this);return false;"">下划线</button>
    <button class=""btn{(!string.IsNullOrEmpty(formatColor) ? " btn-success" : string.Empty)}"" id=""{ContentAttribute.Title}_colorBtn"" onclick=""$('#{ContentAttribute.Title}_colorContainer').toggle();return false;"">颜色</button>
</div>
<div id=""{ContentAttribute.Title}_colorContainer"" class=""input-append"" style=""float:left;display:none"">
    <input id=""{ContentAttribute.Title}_formatColor"" name=""{ContentAttribute.Title}_formatColor"" class=""input-mini color {{required:false}}"" type=""text"" value=""{formatColor}"" placeholder=""颜色值"">
    <button class=""btn"" type=""button"" onclick=""Title_color();return false;"">确定</button>
</div>
<input id=""{ContentAttribute.Title}_formatStrong"" name=""{ContentAttribute.Title}_formatStrong"" type=""hidden"" value=""{formatStrong.ToString().ToLower()}"" />
<input id=""{ContentAttribute.Title}_formatEM"" name=""{ContentAttribute.Title}_formatEM"" type=""hidden"" value=""{formatEm.ToString().ToLower()}"" />
<input id=""{ContentAttribute.Title}_formatU"" name=""{ContentAttribute.Title}_formatU"" type=""hidden"" value=""{formatU.ToString().ToLower()}"" />
");

            builder.Append(@"
<script type=""text/javascript"">
function getTitles(title){
	$.get('[url]&title=' + encodeURIComponent(title) + '&channelID=' + $('#channelID').val() + '&r=' + Math.random(), function(data) {
		if(data !=''){
			var arr = data.split('|');
			var temp='';
			for(i=0;i<arr.length;i++)
			{
				temp += '<li><a>'+arr[i].replace(title,'<b>' + title + '</b>') + '</a></li>';
			}
			var myli='<ul>'+temp+'</ul>';
			$('#titleTips').html(myli);
			$('#titleTips').show();
		}else{
            $('#titleTips').hide();
        }
		$('#titleTips li').click(function () {
			$('#Title').val($(this).text());
			$('#titleTips').hide();
		})
	});	
}
$(document).ready(function () {
$('#Title').keyup(function (e) {
    if (e.keyCode != 40 && e.keyCode != 38) {
        var title = $('#Title').val();
        if (title != ''){
            window.setTimeout(""getTitles('"" + title + ""');"", 200);
        }else{
            $('#titleTips').hide();
        }
    }
}).blur(function () {
	window.setTimeout(""$('#titleTips').hide();"", 200);
})});
</script>
<div id=""titleTips"" class=""inputTips""></div>");
            builder.Replace("[url]", titleAjaxUrl);

            return builder.ToString();
        }

        public static string GetTagsHtml(string tagsAjaxUrl)
        {
            const string tagScript = @"
<script type=""text/javascript"">
function getTags(tag){
	$.get('[url]&tag=' + encodeURIComponent(tag) + '&r=' + Math.random(), function(data) {
		if(data !=''){
			var arr = data.split('|');
			var temp='';
			for(i=0;i<arr.length;i++)
			{
				temp += '<li><a>'+arr[i].replace(tag,'<b>' + tag + '</b>') + '</a></li>';
			}
			var myli='<ul>'+temp+'</ul>';
			$('#tagTips').html(myli);
			$('#tagTips').show();
		}else{
            $('#tagTips').hide();
        }
		$('#tagTips li').click(function () {
			var tag = $('#TbTags').val();
			var i = tag.lastIndexOf(' ');
			if (i > 0)
			{
				tag = tag.substring(0, i) + ' ' + $(this).text();
			}else{
				tag = $(this).text();	
			}
			$('#TbTags').val(tag);
			$('#tagTips').hide();
		})
	});	
}
$(document).ready(function () {
$('#TbTags').keyup(function (e) {
    if (e.keyCode != 40 && e.keyCode != 38) {
        var tag = $('#TbTags').val();
		var i = tag.lastIndexOf(' ');
		if (i > 0){ tag = tag.substring(i + 1);}
        if (tag != '' && tag != ' '){
            window.setTimeout(""getTags('"" + tag + ""');"", 200);
        }else{
            $('#tagTips').hide();
        }
    }
}).blur(function () {
	window.setTimeout(""$('#tagTips').hide();"", 200);
})});
</script>
<div id=""tagTips"" class=""inputTips""></div>
";
            return tagScript.Replace("[url]", tagsAjaxUrl);
        }
    }
}
