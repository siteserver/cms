using System;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using System.Text;
using BaiRong.Core;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;
using BaiRong.Core.AuxiliaryTable;
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

        public static bool SetTitleFormatControls(string titleFormatString, out bool formatStrong, out bool formatEm, out bool formatU, out string formatColor)
        {
            var isTitleFormatted = false;

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
                    if (formatStrong || formatEm || formatU || !string.IsNullOrEmpty(formatColor))
                    {
                        isTitleFormatted = true;
                    }
                }
            }
            return isTitleFormatted;
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

        public static void PutImagePaths(PublishmentSystemInfo publishmentSystemInfo, BackgroundContentInfo contentInfo, NameValueCollection collection)
        {
            if (contentInfo != null)
            {
                if (!string.IsNullOrEmpty(contentInfo.ImageUrl) && PageUtility.IsVirtualUrl(contentInfo.ImageUrl))
                {
                    collection[contentInfo.ImageUrl] = PathUtility.MapPath(publishmentSystemInfo, contentInfo.ImageUrl);
                }
                if (!string.IsNullOrEmpty(contentInfo.VideoUrl) && PageUtility.IsVirtualUrl(contentInfo.VideoUrl))
                {
                    collection[contentInfo.VideoUrl] = PathUtility.MapPath(publishmentSystemInfo, contentInfo.VideoUrl);
                }
                if (!string.IsNullOrEmpty(contentInfo.FileUrl) && PageUtility.IsVirtualUrl(contentInfo.FileUrl))
                {
                    collection[contentInfo.FileUrl] = PathUtility.MapPath(publishmentSystemInfo, contentInfo.FileUrl);
                }

                var srcArrayList = RegexUtils.GetOriginalImageSrcs(contentInfo.Content);
                foreach (string src in srcArrayList)
                {
                    if (PageUtility.IsVirtualUrl(src))
                    {
                        collection[src] = PathUtility.MapPath(publishmentSystemInfo, src);
                    }
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

        public static int GetRealContentId(ETableStyle tableStyle, string tableName, int contentId)
        {
            string linkUrl;
            var referenceId = BaiRongDataProvider.ContentDao.GetReferenceId(tableStyle, tableName, contentId, out linkUrl);
            return referenceId > 0 ? referenceId : contentId;
        }

        public static ContentInfo GetContentInfo(ETableStyle tableStyle)
        {
            switch (tableStyle)
            {
                case ETableStyle.BackgroundContent:
                    return new BackgroundContentInfo();
            }
            return new ContentInfo();
        }

        public static List<TableStyleInfo> GetAllTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, List<TableStyleInfo> tableStyleInfoList)
        {
            var arraylist = new List<TableStyleInfo>();

            var styleInfo = new TableStyleInfo
            {
                AttributeName = ContentAttribute.Id,
                DisplayName = "编号"
            };
            arraylist.Add(styleInfo);

            arraylist.AddRange(tableStyleInfoList);

            styleInfo = new TableStyleInfo
            {
                AttributeName = ContentAttribute.Hits,
                DisplayName = "点击量"
            };
            arraylist.Add(styleInfo);

            if (tableStyle == ETableStyle.BackgroundContent)
            {
                if (publishmentSystemInfo.Additional.IsRelatedByTags)
                {
                    styleInfo = new TableStyleInfo
                    {
                        AttributeName = ContentAttribute.Tags,
                        DisplayName = "标签"
                    };
                    arraylist.Add(styleInfo);
                }

                styleInfo = new TableStyleInfo
                {
                    AttributeName = BackgroundContentAttribute.Star,
                    DisplayName = "评分"
                };
                arraylist.Add(styleInfo);
            }
            else
            {
                styleInfo = new TableStyleInfo
                {
                    AttributeName = ContentAttribute.AddDate,
                    DisplayName = "添加时间"
                };
                arraylist.Add(styleInfo);
            }

            return arraylist;
        }

        public static List<TableStyleInfo> GetColumnTableStyleInfoList(PublishmentSystemInfo publishmentSystemInfo, ETableStyle tableStyle, List<TableStyleInfo> tableStyleInfoArrayList)
        {
            var arraylist = new List<TableStyleInfo>();

            if (tableStyleInfoArrayList != null)
            {
                foreach (var tableStyleInfo in tableStyleInfoArrayList)
                {
                    arraylist.Add(tableStyleInfo);
                }
            }

            var styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.AddUserName, 0, "添加者", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastEditUserName, 0, "修改者", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastEditDate, 0, "修改时间", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckUserName, 0, "审核者", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckCheckDate, 0, "审核时间", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.CheckReasons, 0, "审核原因", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.SourceId, 0, "来源标识", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            if (publishmentSystemInfo.Additional.IsRelatedByTags)
            {
                styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.Tags, 0, "标签", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
                arraylist.Add(styleInfo);
            }

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.ContentGroupNameCollection, 0, "所属内容组", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.Hits, 0, "点击量", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByDay, 0, "日点击", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByWeek, 0, "周点击", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.HitsByMonth, 0, "月点击", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            styleInfo = new TableStyleInfo(0, 0, string.Empty, ContentAttribute.LastHitsDate, 0, "最后点击时间", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
            arraylist.Add(styleInfo);

            if (tableStyle == ETableStyle.BackgroundContent)
            {
                styleInfo = new TableStyleInfo(0, 0, string.Empty, BackgroundContentAttribute.Star, 0, "评分", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
                arraylist.Add(styleInfo);

                styleInfo = new TableStyleInfo(0, 0, string.Empty, BackgroundContentAttribute.Digg, 0, "Digg", string.Empty, true, true, false, InputTypeUtils.GetValue(InputType.Text), string.Empty, false, string.Empty);
                arraylist.Add(styleInfo);
            }

            return arraylist;
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

            var pluginChannels = PluginCache.GetChannelFeatures(nodeInfo);
            foreach (var pluginId in pluginChannels.Keys)
            {
                var pluginChannel = pluginChannels[pluginId];

                if (pluginChannel.OnContentAdded == null) continue;

                try
                {
                    pluginChannel.OnContentAdded(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId);
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(ex, $"插件：{pluginId} OnContentAdded");
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
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);

            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);

            if (contentInfo == null) return;

            if (translateType == ETranslateContentType.Copy)
            {
                FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);

                contentInfo.PublishmentSystemId = targetPublishmentSystemId;
                contentInfo.SourceId = contentInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);

                var pluginChannels = PluginCache.GetChannelFeatures(nodeInfo);
                foreach (var pluginId in pluginChannels.Keys)
                {
                    var pluginChannel = pluginChannels[pluginId];

                    if (pluginChannel.OnContentTranslated == null) continue;

                    try
                    {
                        pluginChannel.OnContentTranslated(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId, targetPublishmentSystemId, targetNodeId, theContentId);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(ex, $"插件：{pluginId} OnContentTranslated");
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
                contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());
                //contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Cut.ToString());

                var newContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);
                DataProvider.ContentDao.DeleteContents(publishmentSystemInfo.PublishmentSystemId, tableName, TranslateUtils.ToIntList(contentId), nodeId);

                DataProvider.NodeDao.UpdateContentNum(publishmentSystemInfo, nodeId, true);
                DataProvider.NodeDao.UpdateContentNum(targetPublishmentSystemInfo, targetNodeId, true);

                var pluginChannels = PluginCache.GetChannelFeatures(nodeInfo);
                foreach (var pluginId in pluginChannels.Keys)
                {
                    var pluginChannel = pluginChannels[pluginId];

                    if (pluginChannel.OnContentTranslated != null)
                    {
                        try
                        {
                            pluginChannel.OnContentTranslated(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId, targetPublishmentSystemId, targetNodeId, newContentId);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(ex, $"插件：{pluginId} OnContentTranslated");
                        }
                    }
                    if (pluginChannel.OnContentDeleted != null)
                    {
                        try
                        {
                            pluginChannel.OnContentDeleted(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId);
                        }
                        catch (Exception ex)
                        {
                            LogUtils.AddErrorLog(ex, $"插件：{pluginId} OnContentDeleted");
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
                contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
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
                contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
                var theContentId = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);

                var pluginChannels = PluginCache.GetChannelFeatures(nodeInfo);
                foreach (var pluginId in pluginChannels.Keys)
                {
                    var pluginChannel = pluginChannels[pluginId];

                    if (pluginChannel.OnContentTranslated == null) continue;

                    try
                    {
                        pluginChannel.OnContentTranslated(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId, contentId, targetPublishmentSystemId, targetNodeId, theContentId);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddErrorLog(ex, $"插件：{pluginId} OnContentTranslated");
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

        public static Dictionary<string, object> ContentToDictionary(ContentInfo contentInfo, ETableStyle tableStyle, string tableName, List<int> relatedIdentities)
        {
            var dict = TranslateUtils.ObjectToDictionary(contentInfo);
            dict.Remove("Attributes");

            var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
            foreach (var styleInfo in styleInfoList)
            {
                if (!styleInfo.IsVisible) continue;
                if (!dict.ContainsKey(styleInfo.AttributeName))
                {
                    dict[styleInfo.AttributeName] = contentInfo.GetExtendedAttribute(styleInfo.AttributeName);
                }
                if (InputTypeUtils.Equals(styleInfo.InputType, InputType.Image))
                {
                    var extendName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    var extendValue = contentInfo.GetExtendedAttribute(extendName);
                    if (!string.IsNullOrEmpty(extendValue))
                    {
                        dict[extendName] = extendValue;
                    }
                }
            }

            return dict;
        }
    }
}
