using System;
using System.Collections.Specialized;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Net;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Core;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxCreateService : Page
    {
        public const string CacheTotalCount = "_TotalCount";
        public const string CacheCurrentCount = "_CurrentCount";
        public const string CacheMessage = "_Message";

        private const string TypeGetCountArray = "GetCountArray";
        private const string TypeCreatePublishmentSystem = "CreatePublishmentSystem";

        public static string GetCountArrayUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCreateService), new NameValueCollection
            {
                {"type", TypeGetCountArray }
            });
        }

        public static string GetCreatePublishmentSystemUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCreateService), new NameValueCollection
            {
                {"type", TypeCreatePublishmentSystem }
            });
        }

        public static string GetCreatePublishmentSystemParameters(int publishmentSystemId, bool isImportContents, bool isImportTableStyles, string siteTemplateDir, string onlineTemplateName, bool isUseTables, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"isImportContents", isImportContents.ToString()},
                {"isImportTableStyles", isImportTableStyles.ToString()},
                {"siteTemplateDir", siteTemplateDir},
                {"onlineTemplateName", onlineTemplateName},
                {"isUseTables", isUseTables.ToString()},
                {"userKeyPrefix", userKeyPrefix}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request.QueryString["type"];
            var userKeyPrefix = Request["userKeyPrefix"];
            var retval = new NameValueCollection();
            var context = new RequestContext();

            if (type == TypeGetCountArray)
            {
                retval = GetCountArray(userKeyPrefix);
            }
            
            if (type == TypeCreatePublishmentSystem)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.Form["publishmentSystemId"]);
                var isImportContents = TranslateUtils.ToBool(Request.Form["isImportContents"]);
                var isImportTableStyles = TranslateUtils.ToBool(Request.Form["isImportTableStyles"]);
                var siteTemplateDir = Request.Form["siteTemplateDir"];
                var onlineTemplateName = Request.Form["onlineTemplateName"];
                var isUseTables = TranslateUtils.ToBool(Request.Form["isUseTables"]);

                if (!string.IsNullOrEmpty(siteTemplateDir))
                {
                    retval = CreatePublishmentSystemBySiteTemplateDir(publishmentSystemId, isImportContents, isImportTableStyles, siteTemplateDir, isUseTables, userKeyPrefix, context.AdminName);
                }
                else if (!string.IsNullOrEmpty(onlineTemplateName))
                {
                    retval = CreatePublishmentSystemByOnlineTemplateName(publishmentSystemId, isImportContents, isImportTableStyles, onlineTemplateName, isUseTables, userKeyPrefix, context.AdminName);
                }
                else
                {
                    retval = CreatePublishmentSystem(publishmentSystemId, userKeyPrefix, context.AdminName);
                }
            }

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(retval);
            Page.Response.Write(jsonString);
            Page.Response.End();
        }

        public NameValueCollection GetCountArray(string userKeyPrefix)//进度及显示
        {
            var retval = new NameValueCollection();
            if (CacheUtils.Get(userKeyPrefix + CacheTotalCount) != null && CacheUtils.Get(userKeyPrefix + CacheCurrentCount) != null && CacheUtils.Get(userKeyPrefix + CacheMessage) != null)
            {
                var totalCount = TranslateUtils.ToInt((string)CacheUtils.Get(userKeyPrefix + CacheTotalCount));
                var currentCount = TranslateUtils.ToInt((string)CacheUtils.Get(userKeyPrefix + CacheCurrentCount));
                var message = (string)CacheUtils.Get(userKeyPrefix + CacheMessage);

                retval = AjaxManager.GetCountArrayNameValueCollection(totalCount, currentCount, message);
            }
            return retval;
        }

        public NameValueCollection CreatePublishmentSystemBySiteTemplateDir(int publishmentSystemId, bool isImportContents, bool isImportTableStyles, string siteTemplateDir, bool isUseTables, string userKeyPrefix, string administratorName)
        {
            var cacheTotalCountKey = userKeyPrefix + CacheTotalCount;
            var cacheCurrentCountKey = userKeyPrefix + CacheCurrentCount;
            var cacheMessageKey = userKeyPrefix + CacheMessage;

            CacheUtils.Insert(cacheTotalCountKey, "3");//存储需要的页面总数
            CacheUtils.Insert(cacheCurrentCountKey, "0");//存储当前的页面总数
            CacheUtils.Insert(cacheMessageKey, string.Empty);//存储消息

            //返回“运行结果”、“错误信息”及“执行JS脚本”的字符串数组
            NameValueCollection retval;

            try
            {
                CacheUtils.Insert(cacheCurrentCountKey, "1");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "正在创建站点...");//存储消息
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

                CacheUtils.Insert(cacheCurrentCountKey, "2");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "正在导入数据...");//存储消息
                SiteTemplateManager.Instance.ImportSiteTemplateToEmptyPublishmentSystem(publishmentSystemId, siteTemplateDir, isUseTables, isImportContents, isImportTableStyles, administratorName);
                CreateManager.CreateAll(publishmentSystemId);

                CacheUtils.Insert(cacheCurrentCountKey, "3");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "创建成功！");//存储消息
                retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{publishmentSystemInfo.PublishmentSystemName}<strong> 创建成功!", string.Empty,
                        $"top.location.href='{PageInitialization.GetRedirectUrl()}';");
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddSystemErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.ClearAll();

            return retval;
        }

        public NameValueCollection CreatePublishmentSystemByOnlineTemplateName(int publishmentSystemId, bool isImportContents, bool isImportTableStyles, string onlineTemplateName, bool isUseTables, string userKeyPrefix, string administratorName)
        {
            var cacheTotalCountKey = userKeyPrefix + CacheTotalCount;
            var cacheCurrentCountKey = userKeyPrefix + CacheCurrentCount;
            var cacheMessageKey = userKeyPrefix + CacheMessage;

            CacheUtils.Insert(cacheTotalCountKey, "4");//存储需要的页面总数
            CacheUtils.Insert(cacheCurrentCountKey, "0");//存储当前的页面总数
            CacheUtils.Insert(cacheMessageKey, string.Empty);//存储消息

            //返回“运行结果”、“错误信息”及“执行JS脚本”的字符串数组
            NameValueCollection retval;

            try
            {
                CacheUtils.Insert(cacheCurrentCountKey, "1");
                CacheUtils.Insert(cacheMessageKey, "开始下载模板压缩包，可能需要几分钟，请耐心等待...");

                var filePath = PathUtility.GetSiteTemplatesPath($"T_{onlineTemplateName}.zip");
                FileUtils.DeleteFileIfExists(filePath);
                var downloadUrl = OnlineTemplateManager.GetDownloadUrl(onlineTemplateName);
                WebClientUtils.SaveRemoteFileToLocal(downloadUrl, filePath);

                CacheUtils.Insert(cacheCurrentCountKey, "2");
                CacheUtils.Insert(cacheMessageKey, "模板压缩包下载成功，开始解压缩，可能需要几分钟，请耐心等待...");

                var siteTemplateDir = $"T_{onlineTemplateName}";
                var directoryPath = PathUtility.GetSiteTemplatesPath(siteTemplateDir);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);
                ZipUtils.UnpackFiles(filePath, directoryPath);

                CacheUtils.Insert(cacheCurrentCountKey, "3");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "站点模板下载成功，正在导入数据...");//存储消息

                SiteTemplateManager.Instance.ImportSiteTemplateToEmptyPublishmentSystem(publishmentSystemId, siteTemplateDir, isUseTables, isImportContents, isImportTableStyles, administratorName);
                CreateManager.CreateAll(publishmentSystemId);

                CacheUtils.Insert(cacheCurrentCountKey, "4");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "创建成功！");//存储消息

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                retval = AjaxManager.GetWaitingTaskNameValueCollection($"站点 <strong>{publishmentSystemInfo.PublishmentSystemName}<strong> 创建成功!", string.Empty,
                        $"top.location.href='{PageInitialization.GetRedirectUrl()}';");
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddSystemErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.ClearAll();

            return retval;
        }

        public NameValueCollection CreatePublishmentSystem(int publishmentSystemId, string userKeyPrefix, string administratorName)
        {
            var cacheTotalCountKey = userKeyPrefix + CacheTotalCount;
            var cacheCurrentCountKey = userKeyPrefix + CacheCurrentCount;
            var cacheMessageKey = userKeyPrefix + CacheMessage;

            CacheUtils.Insert(cacheTotalCountKey, "2");//存储需要的页面总数
            CacheUtils.Insert(cacheCurrentCountKey, "0");//存储当前的页面总数
            CacheUtils.Insert(cacheMessageKey, string.Empty);//存储消息

            //返回“运行结果”、“错误信息”及“执行JS脚本”的字符串数组
            NameValueCollection retval;

            try
            {
                CacheUtils.Insert(cacheCurrentCountKey, "1");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "正在创建站点...");//存储消息
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

                CacheUtils.Insert(cacheCurrentCountKey, "2");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "创建成功！");//存储消息
                retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{publishmentSystemInfo.PublishmentSystemName}<strong> 创建成功!", string.Empty,
                        $"top.location.href='{PageInitialization.GetRedirectUrl()}';");
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddSystemErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.ClearAll();

            return retval;
        }
    }
}
