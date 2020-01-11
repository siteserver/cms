using System;
using SiteServer.Abstractions;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Repositories;
using Content = SiteServer.Abstractions.Content;
using TableStyle = SiteServer.Abstractions.TableStyle;

namespace SiteServer.CMS.Core
{
    public static class ContentUtility
    {
        public static string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页占位符

        public static async Task<string> TextEditorContentEncodeAsync(Site site, string content)
        {
            if (site == null) return content;
            
            if (site.IsSaveImageInTextEditor && !string.IsNullOrEmpty(content))
            {
                content = await PathUtility.SaveImageAsync(site, content);
            }

            var builder = new StringBuilder(content);

            var url = site.GetWebUrl();
            if (!string.IsNullOrEmpty(url) && url != "/")
            {
                StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            }
            //if (!string.IsNullOrEmpty(url))
            //{
            //    StringUtils.ReplaceHrefOrSrc(builder, url, "@");
            //}

            var relatedSiteUrl = PageUtils.ParseNavigationUrl($"~/{site.SiteDir}");
            StringUtils.ReplaceHrefOrSrc(builder, relatedSiteUrl, "@");

            builder.Replace("@'@", "'@");
            builder.Replace("@\"@", "\"@");

            return builder.ToString();
        }

        public static string TextEditorContentDecode(Site site, string content, bool isLocal)
        {
            if (site == null) return content;
            
            var builder = new StringBuilder(content);

            var virtualAssetsUrl = $"@/{site.AssetsDir}";
            string assetsUrl;
            if (isLocal)
            {
                assetsUrl = PageUtility.GetSiteUrl(site,
                    site.AssetsDir, true);
            }
            else
            {
                assetsUrl = site.GetAssetsUrl();
            }
            StringUtils.ReplaceHrefOrSrc(builder, virtualAssetsUrl, assetsUrl);
            StringUtils.ReplaceHrefOrSrc(builder, "@/", site.GetWebUrl()+ "/");
            StringUtils.ReplaceHrefOrSrc(builder, "@", site.GetWebUrl() + "/");
            StringUtils.ReplaceHrefOrSrc(builder, "//", "/");

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

        public static void PutImagePaths(Site site, Content content, NameValueCollection collection)
        {
            if (content == null) return;

            var imageUrl = content.Get<string>(ContentAttribute.ImageUrl);
            var videoUrl = content.Get<string>(ContentAttribute.VideoUrl);
            var fileUrl = content.Get<string>(ContentAttribute.FileUrl);
            var body = content.Get<string>(ContentAttribute.Content);

            if (!string.IsNullOrEmpty(imageUrl) && PageUtility.IsVirtualUrl(imageUrl))
            {
                collection[imageUrl] = PathUtility.MapPath(site, imageUrl);
            }
            if (!string.IsNullOrEmpty(videoUrl) && PageUtility.IsVirtualUrl(videoUrl))
            {
                collection[videoUrl] = PathUtility.MapPath(site, videoUrl);
            }
            if (!string.IsNullOrEmpty(fileUrl) && PageUtility.IsVirtualUrl(fileUrl))
            {
                collection[fileUrl] = PathUtility.MapPath(site, fileUrl);
            }

            var srcList = RegexUtils.GetOriginalImageSrcs(body);
            foreach (var src in srcList)
            {
                if (PageUtility.IsVirtualUrl(src))
                {
                    collection[src] = PathUtility.MapPath(site, src);
                }
                else if (PageUtility.IsRelativeUrl(src))
                {
                    collection[src] = WebUtils.MapPath(src);
                }
            }

            var hrefList = RegexUtils.GetOriginalLinkHrefs(body);
            foreach (var href in hrefList)
            {
                if (PageUtility.IsVirtualUrl(href))
                {
                    collection[href] = PathUtility.MapPath(site, href);
                }
                else if (PageUtility.IsRelativeUrl(href))
                {
                    collection[href] = WebUtils.MapPath(href);
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

        public static List<TableStyle> GetAllTableStyleList(List<TableStyle> tableStyleList)
        {
            var taxis = 1;
            var list = new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = ContentAttribute.Id,
                    DisplayName = "内容Id",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.Title,
                    DisplayName = "标题",
                    Taxis = taxis++
                }
            };

            if (tableStyleList != null)
            {
                foreach (var tableStyle in tableStyleList)
                {
                    if (!list.Exists(t => t.AttributeName == tableStyle.AttributeName))
                    {
                        list.Add(new TableStyle
                        {
                            AttributeName = tableStyle.AttributeName,
                            DisplayName = tableStyle.DisplayName,
                            InputType = tableStyle.InputType,
                            Taxis = taxis++
                        });
                    }
                }
            }

            list.AddRange(new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = nameof(Content.LinkUrl),
                    DisplayName = "外部链接",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.AddDate),
                    DisplayName = "添加时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.LastEditDate),
                    DisplayName = "最后修改时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.AddUserName),
                    DisplayName = "添加人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.LastEditUserName),
                    DisplayName = "最后修改人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.GroupNameCollection),
                    DisplayName = "内容组",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.Tags),
                    DisplayName = "标签",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.AdminId),
                    DisplayName = "管理员",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.UserId),
                    DisplayName = "投稿用户",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.SourceId),
                    DisplayName = "来源标识",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.Hits),
                    DisplayName = "点击量",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.HitsByDay),
                    DisplayName = "日点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.HitsByWeek),
                    DisplayName = "周点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.HitsByMonth),
                    DisplayName = "月点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.LastHitsDate),
                    DisplayName = "最后点击时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.Downloads),
                    DisplayName = "下载量",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.CheckUserName),
                    DisplayName = "审核人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.CheckDate),
                    DisplayName = "审核时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = nameof(Content.CheckReasons),
                    DisplayName = "审核原因",
                    Taxis = taxis
                },
            });

            return list.OrderBy(styleInfo => styleInfo.Taxis == 0 ? int.MaxValue : styleInfo.Taxis).ToList();
        }

        public static List<TableStyle> GetEditableTableStyleList(List<TableStyle> tableStyleList)
        {
            var list = new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = ContentAttribute.Title,
                    Type = InputType.Text,
                    DisplayName = "标题"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.LinkUrl,
                    Type = InputType.Text,
                    DisplayName = "外部链接"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.AddDate,
                    Type = InputType.DateTime,
                    DisplayName = "添加时间"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.GroupNameCollection,
                    Type = InputType.CheckBox,
                    DisplayName = "内容组"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.Tags,
                    Type = InputType.CheckBox,
                    DisplayName = "标签"
                }
            };

            if (tableStyleList != null)
            {
                list.InsertRange(2, tableStyleList);
            }

            return list;
        }

        public static async Task<bool> AfterContentAddedAsync(Site site, Channel channel, int contentId, bool isCrossSiteTrans, bool isAutomatic)
        {
            var isTranslated = false;
            if (isCrossSiteTrans && isAutomatic)
            {
                var targetSiteId = 0;

                if (channel.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    targetSiteId = channel.TransSiteId;
                }
                else if (channel.TransType == ECrossSiteTransType.SelfSite)
                {
                    targetSiteId = site.Id;
                }
                else if (channel.TransType == ECrossSiteTransType.ParentSite)
                {
                    targetSiteId = await DataProvider.SiteRepository.GetParentSiteIdAsync(site.Id);
                }

                if (targetSiteId > 0)
                {
                    var targetSite = await DataProvider.SiteRepository.GetAsync(targetSiteId);
                    if (targetSite != null)
                    {
                        var targetChannelIdArrayList = StringUtils.GetIntList(channel.TransChannelIds);
                        if (targetChannelIdArrayList.Count > 0)
                        {
                            foreach (var targetChannelId in targetChannelIdArrayList)
                            {
                                await CrossSiteTransUtility.TransContentInfoAsync(site, channel, contentId, targetSite, targetChannelId);
                                isTranslated = true;
                            }
                        }
                    }
                }
            }

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnContentAddCompleted(new ContentEventArgs(site.Id, channel.Id, contentId));
                }
                catch (Exception ex)
                {
                    await LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentAddCompleted));
                }
            }

            return isTranslated;
        }

        public static async Task TranslateAsync(Site site, int channelId, int contentId, string translateCollection, ETranslateContentType translateType, string administratorName)
        {
            var translateList = StringUtils.GetStringList(translateCollection);
            foreach (var translate in translateList)
            {
                if (string.IsNullOrEmpty(translate)) continue;

                var translates = translate.Split('_');
                if (translates.Length != 2) continue;

                var targetSiteId = TranslateUtils.ToInt(translates[0]);
                var targetChannelId = TranslateUtils.ToInt(translates[1]);

                await TranslateAsync(site, channelId, contentId, targetSiteId, targetChannelId, translateType);
            }
        }

        public static async Task DeleteAsync(string tableName, Site site, int channelId, int contentId)
        {
            if (string.IsNullOrEmpty(tableName) || site == null || contentId <= 0) return;
            
            await DataProvider.ContentRepository.DeleteAsync(tableName, site.Id, contentId);

            await ContentTagUtils.RemoveTagsAsync(site.Id, contentId);

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnContentDeleteCompleted(new ContentEventArgs(site.Id, channelId, contentId));
                }
                catch (Exception ex)
                {
                    await LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentDeleteCompleted));
                }
            }
        }

        public static async Task TranslateAsync(Site site, int channelId, int contentId, int targetSiteId, int targetChannelId, ETranslateContentType translateType)
        {
            if (site == null || channelId <= 0 || contentId <= 0 || targetSiteId <= 0 || targetChannelId <= 0) return;

            var targetSite = await DataProvider.SiteRepository.GetAsync(targetSiteId);
            var targetChannelInfo = await ChannelManager.GetChannelAsync(targetSiteId, targetChannelId);

            var channelInfo = await ChannelManager.GetChannelAsync(site.Id, channelId);
            var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);

            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);

            if (contentInfo == null) return;

            if (translateType == ETranslateContentType.Copy)
            {
                FileUtility.MoveFileByContentInfo(site, targetSite, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.TranslateContentType = ETranslateContentTypeUtils.GetValue(ETranslateContentType.Copy);
                var theContentId = await DataProvider.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                foreach (var service in await PluginManager.GetServicesAsync())
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(site.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        await LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                await CreateManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, theContentId);
                await CreateManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.Cut)
            {
                FileUtility.MoveFileByContentInfo(site, targetSite, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.TranslateContentType = ETranslateContentTypeUtils.GetValue(ETranslateContentType.Cut);

                var newContentId = await DataProvider.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                foreach (var service in await PluginManager.GetServicesAsync())
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(site.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, newContentId));
                    }
                    catch (Exception ex)
                    {
                        await LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                await DeleteAsync(tableName, site, channelId, contentId);

                //DataProvider.ContentRepository.DeleteContents(site.Id, tableName, TranslateUtils.ToIntList(contentId), channelId);

                await CreateManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, newContentId);
                await CreateManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.Reference)
            {
                if (contentInfo.ReferenceId != 0) return;

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.TranslateContentType = ETranslateContentTypeUtils.GetValue(ETranslateContentType.Reference);
                //content.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                int theContentId = await DataProvider.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                await CreateManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, theContentId);
                await CreateManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
            else if (translateType == ETranslateContentType.ReferenceContent)
            {
                if (contentInfo.ReferenceId != 0) return;

                FileUtility.MoveFileByContentInfo(site, targetSite, contentInfo);

                contentInfo.SiteId = targetSiteId;
                contentInfo.SourceId = contentInfo.ChannelId;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.TranslateContentType = ETranslateContentTypeUtils.GetValue(ETranslateContentType.ReferenceContent);
                var theContentId = await DataProvider.ContentRepository.InsertAsync(targetSite, targetChannelInfo, contentInfo);

                foreach (var service in await PluginManager.GetServicesAsync())
                {
                    try
                    {
                        service.OnContentTranslateCompleted(new ContentTranslateEventArgs(site.Id, channelInfo.Id, contentId, targetSiteId, targetChannelId, theContentId));
                    }
                    catch (Exception ex)
                    {
                        await LogUtils.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentTranslateCompleted));
                    }
                }

                await CreateManager.CreateContentAsync(targetSite.Id, contentInfo.ChannelId, theContentId);
                await CreateManager.TriggerContentChangedEventAsync(targetSite.Id, contentInfo.ChannelId);
            }
        }

        public static Dictionary<int, List<int>> GetIDsDictionary(NameValueCollection queryString)
        {
            var dic = new Dictionary<int, List<int>>();

            if (!string.IsNullOrEmpty(queryString["IDsCollection"]))
            {
                foreach (var ids in StringUtils.GetStringList(queryString["IDsCollection"]))
                {
                    var channelId = TranslateUtils.ToIntWithNegative(ids.Split('_')[0]);
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
                dic[channelId] = StringUtils.GetIntList(queryString["contentIdCollection"]);
            }

            return dic;
        }

        public static string GetTitleHtml(string titleFormat)
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

        public static bool IsCreatable(Channel channel, Content content)
        {
            if (channel == null || content == null) return false;

            //引用链接，不需要生成内容页；引用内容，需要生成内容页；
            if (content.ReferenceId > 0 &&
                !ETranslateContentTypeUtils.Equals(ETranslateContentType.ReferenceContent, content.TranslateContentType))
            {
                return false;
            }

            return string.IsNullOrEmpty(content.LinkUrl) && content.Checked && content.SourceId != SourceManager.Preview && content.ChannelId > 0;
        }
    }
}
