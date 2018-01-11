using System;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using System.Text;
using BaiRong.Core;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Table;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Core
{
    public class ContentUtility
    {
        private ContentUtility()
        {
        }

        public static string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页展位符

        public static string TextEditorContentEncode(PublishmentSystemInfo publishmentSystemInfo, string content)
        {
            if (publishmentSystemInfo == null) return content;

            var url = publishmentSystemInfo.Additional.WebUrl;
            if (publishmentSystemInfo.Additional.IsSaveImageInTextEditor && !string.IsNullOrEmpty(content))
            {
                content = PathUtility.SaveImage(publishmentSystemInfo, content);
            }

            var builder = new StringBuilder(content);

            if (url == "/")
            {
                url = string.Empty;
            }

            StringUtils.ReplaceHrefOrSrc(builder, url, "@");

            builder.Replace("@'@", "'@");
            builder.Replace("@\"@", "\"@");

            return builder.ToString();
        }

        public static string TextEditorContentDecode(PublishmentSystemInfo publishmentSystemInfo, string content, bool isLocal)
        {
            if (publishmentSystemInfo == null) return content;
            
            var builder = new StringBuilder(content);

            var virtualAssetsUrl = $"@/{publishmentSystemInfo.Additional.AssetsDir}";
            string assetsUrl;
            if (isLocal)
            {
                assetsUrl = PageUtility.GetLocalPublishmentSystemUrl(publishmentSystemInfo,
                    publishmentSystemInfo.Additional.AssetsDir);
            }
            else
            {
                assetsUrl = publishmentSystemInfo.Additional.AssetsUrl;
            }
            StringUtils.ReplaceHrefOrSrc(builder, virtualAssetsUrl, assetsUrl);
            StringUtils.ReplaceHrefOrSrc(builder, "@", publishmentSystemInfo.Additional.WebUrl);

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

        public static void PutImagePaths(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, NameValueCollection collection)
        {
            if (contentInfo == null) return;

            var imageUrl = contentInfo.GetString(BackgroundContentAttribute.ImageUrl);
            var videoUrl = contentInfo.GetString(BackgroundContentAttribute.VideoUrl);
            var fileUrl = contentInfo.GetString(BackgroundContentAttribute.FileUrl);
            var content = contentInfo.GetString(BackgroundContentAttribute.Content);

            if (!string.IsNullOrEmpty(imageUrl) && PageUtility.IsVirtualUrl(imageUrl))
            {
                collection[imageUrl] = PathUtility.MapPath(publishmentSystemInfo, imageUrl);
            }
            if (!string.IsNullOrEmpty(videoUrl) && PageUtility.IsVirtualUrl(videoUrl))
            {
                collection[videoUrl] = PathUtility.MapPath(publishmentSystemInfo, videoUrl);
            }
            if (!string.IsNullOrEmpty(fileUrl) && PageUtility.IsVirtualUrl(fileUrl))
            {
                collection[fileUrl] = PathUtility.MapPath(publishmentSystemInfo, fileUrl);
            }

            var srcList = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var src in srcList)
            {
                if (PageUtility.IsVirtualUrl(src))
                {
                    collection[src] = PathUtility.MapPath(publishmentSystemInfo, src);
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

        public static int GetRealContentId(string tableName, int contentId)
        {
            string linkUrl;
            var referenceId = DataProvider.ContentDao.GetReferenceId(tableName, contentId, out linkUrl);
            return referenceId > 0 ? referenceId : contentId;
        }

        public static List<TableStyleInfo> GetAllTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, List<TableStyleInfo> tableStyleInfoList)
        {
            var list = new List<TableStyleInfo>
            {
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Id,
                    DisplayName = "编号"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Hits,
                    DisplayName = "点击量"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.Tags,
                    DisplayName = "标签"
                },
                new TableStyleInfo
                {
                    AttributeName = BackgroundContentAttribute.Star,
                    DisplayName = "评分"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.LinkUrl,
                    DisplayName = "外部链接"
                },
                new TableStyleInfo
                {
                    AttributeName = ContentAttribute.AddDate,
                    DisplayName = "添加时间"
                }
            };

            list.AddRange(tableStyleInfoList);

            return list;
        }

        public static List<TableStyleInfo> GetColumnTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, List<TableStyleInfo> tableStyleInfoList)
        {
            var list = new List<TableStyleInfo>
            {
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.AddUserName, 0, "添加者", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastEditUserName, 0, "修改者", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastEditDate, 0, "修改时间", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckUserName, 0, "审核者", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckCheckDate, 0, "审核时间", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckReasons, 0, "审核原因", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.SourceId, 0, "来源标识", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.Tags, 0, "标签", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.ContentGroupNameCollection, 0, "所属内容组",
                    string.Empty, true, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.Hits, 0, "点击量", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByDay, 0, "日点击", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByWeek, 0, "周点击", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByMonth, 0, "月点击", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastHitsDate, 0, "最后点击时间", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, BackgroundContentAttribute.Star, 0, "评分", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty),
                new TableStyleInfo(0, 0, string.Empty, BackgroundContentAttribute.Digg, 0, "Digg", string.Empty, true,
                    InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty)
            };

            if (tableStyleInfoList != null)
            {
                list.InsertRange(0, tableStyleInfoList);
            }

            return list;
        }

        public static bool AfterContentAdded(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, int contentId, bool isCrossSiteTrans, bool isAutomatic)
        {
            var isTranslated = false;
            if (isCrossSiteTrans && isAutomatic)
            {
                var targetPublishmentSystemId = 0;

                if (nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    targetPublishmentSystemId = nodeInfo.Additional.TransPublishmentSystemId;
                }
                else if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                {
                    targetPublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
                }
                else if (nodeInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
                {
                    targetPublishmentSystemId = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                }

                if (targetPublishmentSystemId > 0)
                {
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    if (targetPublishmentSystemInfo != null)
                    {
                        var targetNodeIdArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.TransNodeIds);
                        if (targetNodeIdArrayList.Count > 0)
                        {
                            foreach (var targetNodeId in targetNodeIdArrayList)
                            {
                                CrossSiteTransUtility.TransContentInfo(publishmentSystemInfo, nodeInfo, contentId, targetPublishmentSystemInfo, targetNodeId);
                                isTranslated = true;
                            }
                        }
                    }
                }
            }

            var pluginChannels = PluginManager.GetContentRelatedFeatures(nodeInfo);
            foreach (var pluginId in pluginChannels.Keys)
            {
                var pluginChannel = pluginChannels[pluginId];

                if (pluginChannel.ContentAddCompleted == null) continue;

                try
                {
                    pluginChannel.ContentAddCompleted(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId);
                }
                catch (Exception ex)
                {
                    LogUtils.AddPluginErrorLog(pluginId, ex, "ContentAddCompleted");
                }
            }

            return isTranslated;
        }

        public static void Translate(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, string translateCollection, ETranslateContentType translateType, string administratorName)
        {
            var translateArrayList = TranslateUtils.StringCollectionToStringList(translateCollection);
            foreach (var translate in translateArrayList)
            {
                if (string.IsNullOrEmpty(translate)) continue;

                var translates = translate.Split('_');
                if (translates.Length != 2) continue;

                var targetPublishmentSystemId = TranslateUtils.ToInt(translates[0]);
                var targetNodeId = TranslateUtils.ToInt(translates[1]);

                Translate(administratorName, publishmentSystemInfo, nodeId, contentId, targetPublishmentSystemId, targetNodeId, translateType);
            }
        }

        public static void Translate(string administratorName, PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int targetPublishmentSystemId, int targetNodeId, ETranslateContentType translateType)
        {
            if (publishmentSystemInfo == null || nodeId <= 0 || contentId <= 0 || targetPublishmentSystemId <= 0 || targetNodeId <= 0) return;

            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);

            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeId);

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, contentId);

            if (contentInfo == null) return;

            if (translateType == ETranslateContentType.Copy)
            {
                FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);

                var pluginChannels = PluginManager.GetContentRelatedFeatures(nodeInfo);
                foreach (var pluginId in pluginChannels.Keys)
                {
                    var pluginChannel = pluginChannels[pluginId];

                    if (pluginChannel.ContentTranslateCompleted == null) continue;

                    try
                    {
                        pluginChannel.ContentTranslateCompleted(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId, targetPublishmentSystemId, targetNodeId, theContentId);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddPluginErrorLog(pluginId, ex, "ContentTranslateCompleted");
                    }
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentId);
                }
            }
            else if (translateType == ETranslateContentType.Cut)
            {
                FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());

                var newContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                DataProvider.ContentDao.DeleteContents(publishmentSystemInfo.PublishmentSystemId, tableName, TranslateUtils.ToIntList(contentId), nodeId);

                DataProvider.NodeDao.UpdateContentNum(publishmentSystemInfo, nodeId, true);
                DataProvider.NodeDao.UpdateContentNum(targetPublishmentSystemInfo, targetNodeId, true);

                var pluginChannels = PluginManager.GetContentRelatedFeatures(nodeInfo);
                foreach (var pluginId in pluginChannels.Keys)
                {
                    var pluginChannel = pluginChannels[pluginId];

                    if (pluginChannel.ContentTranslateCompleted != null)
                    {
                        try
                        {
                            pluginChannel.ContentTranslateCompleted(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId, targetPublishmentSystemId, targetNodeId, newContentId);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddPluginErrorLog(pluginId, ex, "ContentTranslateCompleted");
                        }
                    }
                    if (pluginChannel.ContentDeleteCompleted != null)
                    {
                        try
                        {
                            pluginChannel.ContentDeleteCompleted(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddPluginErrorLog(pluginId, ex, "ContentDeleteCompleted");
                        }
                    }
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, newContentId);
                }
            }
            else if (translateType == ETranslateContentType.Reference)
            {
                if (contentInfo.ReferenceId != 0) return;

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
                DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
            }
            else if (translateType == ETranslateContentType.ReferenceContent)
            {
                if (contentInfo.ReferenceId != 0) return;

                FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);

                var pluginChannels = PluginManager.GetContentRelatedFeatures(nodeInfo);
                foreach (var pluginId in pluginChannels.Keys)
                {
                    var pluginChannel = pluginChannels[pluginId];

                    if (pluginChannel.ContentTranslateCompleted == null) continue;

                    try
                    {
                        pluginChannel.ContentTranslateCompleted(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId, targetPublishmentSystemId, targetNodeId, theContentId);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddPluginErrorLog(pluginId, ex, "ContentTranslateCompleted");
                    }
                }

                if (contentInfo.IsChecked)
                {
                    CreateManager.CreateContentAndTrigger(targetPublishmentSystemInfo.PublishmentSystemId, contentInfo.NodeId, theContentId);
                }
            }
        }

        public static Dictionary<int, List<int>> GetIDsDictionary(NameValueCollection queryString)
        {
            var dic = new Dictionary<int, List<int>>();

            if (!string.IsNullOrEmpty(queryString["IDsCollection"]))
            {
                foreach (var ids in TranslateUtils.StringCollectionToStringList(queryString["IDsCollection"]))
                {
                    var nodeId = TranslateUtils.ToInt(ids.Split('_')[0]);
                    var contentId = TranslateUtils.ToInt(ids.Split('_')[1]);
                    var contentIdList = new List<int>();
                    if (dic.ContainsKey(nodeId))
                    {
                        contentIdList = dic[nodeId];
                    }
                    if (!contentIdList.Contains(contentId))
                    {
                        contentIdList.Add(contentId);
                    }

                    dic[nodeId] = contentIdList;
                }
            }
            else
            {
                var nodeId = TranslateUtils.ToInt(queryString["NodeID"]);
                dic[nodeId] = TranslateUtils.StringCollectionToIntList(queryString["ContentIDCollection"]);
            }

            return dic;
        }

        public static Dictionary<string, object> ContentToDictionary(ContentInfo contentInfo, string tableName, List<int> relatedIdentities)
        {
            var dict = TranslateUtils.ObjectToDictionary(contentInfo);
            dict.Remove("Attributes");

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (!dict.ContainsKey(styleInfo.AttributeName))
                {
                    dict[styleInfo.AttributeName] = contentInfo.GetString(styleInfo.AttributeName);
                }
                if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Image))
                {
                    var extendName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValue = contentInfo.GetString(extendName);
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        dict[extendName] = extendValue;
                    }
                }
            }

            return dict;
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
