using System;
using System.Collections.Specialized;
using System.Web.UI;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxCreateService : Page
    {
        public const string CacheTotalCount = "_TotalCount";
        public const string CacheCurrentCount = "_CurrentCount";
        public const string CacheMessage = "_Message";

        private const string TypeGetCountArray = "GetCountArray";
        private const string TypeCreateSite = "CreateSite";

        public static string GetCountArrayUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCreateService), new NameValueCollection
            {
                {"type", TypeGetCountArray }
            });
        }

        public static string GetCreateSiteUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxCreateService), new NameValueCollection
            {
                {"type", TypeCreateSite }
            });
        }

        public static string GetCreateSiteParameters(int siteId, bool isImportContents, bool isImportTableStyles, string siteTemplateDir, string onlineTemplateName, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"isImportContents", isImportContents.ToString()},
                {"isImportTableStyles", isImportTableStyles.ToString()},
                {"siteTemplateDir", siteTemplateDir},
                {"onlineTemplateName", onlineTemplateName},
                {"userKeyPrefix", userKeyPrefix}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request.QueryString["type"];
            var userKeyPrefix = Request["userKeyPrefix"];
            var retval = new NameValueCollection();
            var request = new RequestImpl();

            if (type == TypeGetCountArray)
            {
                retval = GetCountArray(userKeyPrefix);
            }
            
            if (type == TypeCreateSite)
            {
                var siteId = TranslateUtils.ToInt(Request.Form["siteId"]);
                var isImportContents = TranslateUtils.ToBool(Request.Form["isImportContents"]);
                var isImportTableStyles = TranslateUtils.ToBool(Request.Form["isImportTableStyles"]);
                var siteTemplateDir = Request.Form["siteTemplateDir"];
                var onlineTemplateName = Request.Form["onlineTemplateName"];

                if (!string.IsNullOrEmpty(siteTemplateDir))
                {
                    retval = CreateSiteBySiteTemplateDir(siteId, isImportContents, isImportTableStyles, siteTemplateDir, userKeyPrefix, request.AdminName);
                }
                else if (!string.IsNullOrEmpty(onlineTemplateName))
                {
                    retval = CreateSiteByOnlineTemplateName(siteId, isImportContents, isImportTableStyles, onlineTemplateName, userKeyPrefix, request.AdminName);
                }
                else
                {
                    retval = CreateSite(siteId, userKeyPrefix, request.AdminName);
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

        public NameValueCollection CreateSiteBySiteTemplateDir(int siteId, bool isImportContents, bool isImportTableStyles, string siteTemplateDir, string userKeyPrefix, string administratorName)
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
                var siteInfo = SiteManager.GetSiteInfo(siteId);

                CacheUtils.Insert(cacheCurrentCountKey, "2");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "正在导入数据...");//存储消息
                SiteTemplateManager.Instance.ImportSiteTemplateToEmptySite(siteId, siteTemplateDir, isImportContents, isImportTableStyles, administratorName);
                CreateManager.CreateByAll(siteId);

                CacheUtils.Insert(cacheCurrentCountKey, "3");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "创建成功！");//存储消息
                retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{siteInfo.SiteName}<strong> 创建成功!", string.Empty,
                        $"top.location.href='{PageUtils.GetMainUrl(siteId)}';");
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.ClearAll();

            return retval;
        }

        public NameValueCollection CreateSiteByOnlineTemplateName(int siteId, bool isImportContents, bool isImportTableStyles, string onlineTemplateName, string userKeyPrefix, string administratorName)
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
                ZipUtils.ExtractZip(filePath, directoryPath);

                CacheUtils.Insert(cacheCurrentCountKey, "3");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "模板压缩包解压成功，正在导入数据...");//存储消息

                SiteTemplateManager.Instance.ImportSiteTemplateToEmptySite(siteId, siteTemplateDir, isImportContents, isImportTableStyles, administratorName);
                CreateManager.CreateByAll(siteId);

                CacheUtils.Insert(cacheCurrentCountKey, "4");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "创建成功！");//存储消息

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                retval = AjaxManager.GetWaitingTaskNameValueCollection($"站点 <strong>{siteInfo.SiteName}<strong> 创建成功!", string.Empty,
                        $"top.location.href='{PageUtils.GetMainUrl(siteId)}';");
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.ClearAll();

            return retval;
        }

        public NameValueCollection CreateSite(int siteId, string userKeyPrefix, string administratorName)
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
                var siteInfo = SiteManager.GetSiteInfo(siteId);

                CacheUtils.Insert(cacheCurrentCountKey, "2");//存储当前的页面总数
                CacheUtils.Insert(cacheMessageKey, "创建成功！");//存储消息
                retval = AjaxManager.GetWaitingTaskNameValueCollection(
                        $"站点 <strong>{siteInfo.SiteName}<strong> 创建成功!", string.Empty,
                        $"top.location.href='{PageUtils.GetMainUrl(siteId)}';");
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            CacheUtils.Remove(cacheTotalCountKey);//取消存储需要的页面总数
            CacheUtils.Remove(cacheCurrentCountKey);//取消存储当前的页面总数
            CacheUtils.Remove(cacheMessageKey);//取消存储消息
            CacheUtils.ClearAll();

            return retval;
        }
    }
}
