using System;
using System.Collections.Specialized;
using System.Web.UI;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Api;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model.Enumerations;

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
            var request = new AuthenticatedRequest();

            if (type == TypeBackup)
            {
                var siteId = TranslateUtils.ToInt(Request.Form["siteID"]);
                var backupType = Request.Form["backupType"];
                retVal = Backup(siteId, backupType, userKeyPrefix);
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
                retVal = Recovery(siteId, isDeleteChannels, isDeleteTemplates, isDeleteFiles, isZip, path, isOverride, isUseTable, userKeyPrefix, request);
            }

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(retVal);
            Page.Response.Write(jsonString);
            Page.Response.End();
        }

        public NameValueCollection Backup(int siteId, string backupType, string userKeyPrefix)
        {
            //返回“运行结果”和“错误信息”的字符串数组
            NameValueCollection retVal;
            var request = new AuthenticatedRequest(Request);

            try
            {
                var eBackupType = EBackupTypeUtils.GetEnumType(backupType);

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var filePath = PathUtility.GetBackupFilePath(siteInfo, eBackupType);
                DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                FileUtils.DeleteFileIfExists(filePath);

                if (eBackupType == EBackupType.Templates)
                {
                    BackupUtility.BackupTemplates(siteId, filePath, request.AdminName);
                }
                else if (eBackupType == EBackupType.ChannelsAndContents)
                {
                    BackupUtility.BackupChannelsAndContents(siteId, filePath, request.AdminName);
                }
                else if (eBackupType == EBackupType.Files)
                {
                    BackupUtility.BackupFiles(siteId, filePath, request.AdminName);
                }
                else if (eBackupType == EBackupType.Site)
                {
                    BackupUtility.BackupSite(siteId, filePath, request.AdminName);
                }

                string resultString =
                    $"任务完成，备份地址：<br /><strong> {filePath} </strong>&nbsp;<a href='{ApiRouteActionsDownload.GetUrl(ApiManager.InnerApiUrl, filePath)}'><img src='{SiteServerAssets.GetIconUrl("download.gif")}' />下载</a>。";

                retVal = AjaxManager.GetWaitingTaskNameValueCollection(resultString, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                retVal = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            return retVal;
        }

        public NameValueCollection Recovery(int siteId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string userKeyPrefix, AuthenticatedRequest request)
        {
            //返回“运行结果”和“错误信息”的字符串数组
            NameValueCollection retVal;

            try
            {
                BackupUtility.RecoverySite(siteId, isDeleteChannels, isDeleteTemplates, isDeleteFiles, isZip, PageUtils.UrlDecode(path), isOverride, isUseTable, request.AdminName);

                request.AddSiteLog(siteId, "恢复备份数据", request.AdminName);

                retVal = AjaxManager.GetWaitingTaskNameValueCollection("数据恢复成功!", string.Empty, string.Empty);

                //retVal = new string[] { "数据恢复成功!", string.Empty, string.Empty };
            }
            catch (Exception ex)
            {
                //retVal = new string[] { string.Empty, ex.Message, string.Empty };
                retVal = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            return retVal;
        }
    }
}
