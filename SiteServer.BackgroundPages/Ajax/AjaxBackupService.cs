using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.UI;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Repositories;

namespace SiteServer.BackgroundPages.Ajax
{
    public class AjaxBackupService : Page
    {
        private const string TypeGetCountArray = "GetCountArray";
        private const string TypeBackup = "Backup";
        private const string TypeRecovery = "Recovery";

        public static string GetCountArrayUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxBackupService), new NameValueCollection
            {
                {"type", TypeGetCountArray }
            });
        }

        public static string GetBackupUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxBackupService), new NameValueCollection
            {
                {"type", TypeBackup }
            });
        }

        public static string GetBackupParameters(int siteId, string backupType, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"siteID", siteId.ToString()},
                {"backupType", backupType},
                {"userKeyPrefix", userKeyPrefix}
            });
        }

        public static string GetRecoveryUrl()
        {
            return PageUtils.GetAjaxUrl(nameof(AjaxBackupService), new NameValueCollection
            {
                {"type", TypeRecovery }
            });
        }

        public static string GetRecoveryParameters(int siteId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"siteID", siteId.ToString()},
                {"isDeleteChannels", isDeleteChannels.ToString()},
                {"isDeleteTemplates", isDeleteTemplates.ToString()},
                {"isDeleteFiles", isDeleteFiles.ToString()},
                {"isZip", isZip.ToString()},
                {"path", path},
                {"isOverride", isOverride.ToString()},
                {"isUseTable", isUseTable.ToString()},
                {"userKeyPrefix", userKeyPrefix}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var type = Request.QueryString["type"];
            var userKeyPrefix = Request["userKeyPrefix"];
            var retVal = new NameValueCollection();
            var request = AuthenticatedRequest.GetAuthAsync().GetAwaiter().GetResult();

            if (type == TypeBackup)
            {
                var siteId = TranslateUtils.ToInt(Request.Form["siteID"]);
                var backupType = Request.Form["backupType"];
                retVal = BackupAsync(siteId, backupType, userKeyPrefix).GetAwaiter().GetResult();
            }
            else if (type == TypeRecovery)
            {
                var siteId = TranslateUtils.ToInt(Request.Form["siteID"]);
                var isDeleteChannels = TranslateUtils.ToBool(Request.Form["isDeleteChannels"]);
                var isDeleteTemplates = TranslateUtils.ToBool(Request.Form["isDeleteTemplates"]);
                var isDeleteFiles = TranslateUtils.ToBool(Request.Form["isDeleteFiles"]);
                var isZip = TranslateUtils.ToBool(Request.Form["isZip"]);
                var path = Request.Form["path"];
                var isOverride = TranslateUtils.ToBool(Request.Form["isOverride"]);
                var isUseTable = TranslateUtils.ToBool(Request.Form["isUseTable"]);
                retVal = RecoveryAsync(siteId, isDeleteChannels, isDeleteTemplates, isDeleteFiles, isZip, path, isOverride, isUseTable, userKeyPrefix, request).GetAwaiter().GetResult();
            }

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(retVal);
            Page.Response.Write(jsonString);
            Page.Response.End();
        }

        public async Task<NameValueCollection> BackupAsync(int siteId, string backupType, string userKeyPrefix)
        {
            //返回“运行结果”和“错误信息”的字符串数组
            NameValueCollection retVal;
            var request = AuthenticatedRequest.GetAuthAsync().GetAwaiter().GetResult();

            try
            {
                var eBackupType = EBackupTypeUtils.GetEnumType(backupType);

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var filePath = PathUtility.GetBackupFilePath(site, eBackupType);
                DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                FileUtils.DeleteFileIfExists(filePath);

                if (eBackupType == EBackupType.Templates)
                {
                    await BackupUtility.BackupTemplatesAsync(siteId, filePath, request.AdminName);
                }
                else if (eBackupType == EBackupType.ChannelsAndContents)
                {
                    await BackupUtility.BackupChannelsAndContentsAsync(siteId, filePath, request.AdminName);
                }
                else if (eBackupType == EBackupType.Files)
                {
                    await BackupUtility.BackupFilesAsync(siteId, filePath, request.AdminName);
                }
                else if (eBackupType == EBackupType.Site)
                {
                    await BackupUtility.BackupSiteAsync(siteId, filePath, request.AdminName);
                }

                string resultString =
                    $"任务完成，备份地址：<br /><strong> {filePath} </strong>&nbsp;<a href='{ApiRouteActionsDownload.GetUrl(ApiManager.InnerApiUrl, filePath)}'><img src='{SiteServerAssets.GetIconUrl("download.gif")}' />下载</a>。";

                retVal = AjaxManager.GetWaitingTaskNameValueCollection(resultString, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                retVal = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                await LogUtils.AddErrorLogAsync(ex);
            }

            return retVal;
        }

        public async Task<NameValueCollection> RecoveryAsync(int siteId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string userKeyPrefix, AuthenticatedRequest request)
        {
            //返回“运行结果”和“错误信息”的字符串数组
            NameValueCollection retVal;

            try
            {
                await BackupUtility.RecoverySiteAsync(siteId, isDeleteChannels, isDeleteTemplates, isDeleteFiles, isZip, PageUtils.UrlDecode(path), isOverride, isUseTable, request.AdminName);

                await request.AddSiteLogAsync(siteId, "恢复备份数据", request.AdminName);

                retVal = AjaxManager.GetWaitingTaskNameValueCollection("数据恢复成功!", string.Empty, string.Empty);

                //retVal = new string[] { "数据恢复成功!", string.Empty, string.Empty };
            }
            catch (Exception ex)
            {
                //retVal = new string[] { string.Empty, ex.Message, string.Empty };
                retVal = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                await LogUtils.AddErrorLogAsync(ex);
            }

            return retVal;
        }
    }
}
