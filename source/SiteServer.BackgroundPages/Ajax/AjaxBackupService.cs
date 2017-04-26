using System;
using System.Collections.Specialized;
using System.Web.UI;
using BaiRong.Core;
using BaiRong.Core.Text;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
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

        public static string GetBackupParameters(int publishmentSystemId, string backupType, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
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

        public static string GetRecoveryParameters(int publishmentSystemId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string userKeyPrefix)
        {
            return TranslateUtils.NameValueCollectionToString(new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()},
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
            var retval = new NameValueCollection();
            var body = new RequestBody();

            if (type == TypeBackup)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.Form["publishmentSystemID"]);
                var backupType = Request.Form["backupType"];
                retval = Backup(publishmentSystemId, backupType, userKeyPrefix);
            }
            else if (type == TypeRecovery)
            {
                var publishmentSystemId = TranslateUtils.ToInt(Request.Form["publishmentSystemID"]);
                var isDeleteChannels = TranslateUtils.ToBool(Request.Form["isDeleteChannels"]);
                var isDeleteTemplates = TranslateUtils.ToBool(Request.Form["isDeleteTemplates"]);
                var isDeleteFiles = TranslateUtils.ToBool(Request.Form["isDeleteFiles"]);
                var isZip = TranslateUtils.ToBool(Request.Form["isZip"]);
                var path = Request.Form["path"];
                var isOverride = TranslateUtils.ToBool(Request.Form["isOverride"]);
                var isUseTable = TranslateUtils.ToBool(Request.Form["isUseTable"]);
                retval = Recovery(publishmentSystemId, isDeleteChannels, isDeleteTemplates, isDeleteFiles, isZip, path, isOverride, isUseTable, userKeyPrefix, body);
            }

            var jsonString = TranslateUtils.NameValueCollectionToJsonString(retval);
            Page.Response.Write(jsonString);
            Page.Response.End();
        }

        public NameValueCollection Backup(int publishmentSystemId, string backupType, string userKeyPrefix)
        {
            //返回“运行结果”和“错误信息”的字符串数组
            NameValueCollection retval;

            try
            {
                var eBackupType = EBackupTypeUtils.GetEnumType(backupType);

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var filePath = PathUtility.GetBackupFilePath(publishmentSystemInfo, eBackupType);
                DirectoryUtils.CreateDirectoryIfNotExists(filePath);
                FileUtils.DeleteFileIfExists(filePath);

                if (eBackupType == EBackupType.Templates)
                {
                    BackupUtility.BackupTemplates(publishmentSystemId, filePath);
                }
                else if (eBackupType == EBackupType.ChannelsAndContents)
                {
                    BackupUtility.BackupChannelsAndContents(publishmentSystemId, filePath);
                }
                else if (eBackupType == EBackupType.Files)
                {
                    BackupUtility.BackupFiles(publishmentSystemId, filePath);
                }
                else if (eBackupType == EBackupType.Site)
                {
                    BackupUtility.BackupSite(publishmentSystemId, filePath);
                }

                string resultString =
                    $"任务完成，备份地址：<br /><strong> {filePath} </strong>&nbsp;<a href='{ActionsDownload.GetUrl(publishmentSystemInfo.Additional.ApiUrl, filePath)}'><img src='{SiteServerAssets.GetIconUrl("download.gif")}' />下载</a>。";

                retval = AjaxManager.GetWaitingTaskNameValueCollection(resultString, string.Empty, string.Empty);
            }
            catch (Exception ex)
            {
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            return retval;
        }

        public NameValueCollection Recovery(int publishmentSystemId, bool isDeleteChannels, bool isDeleteTemplates, bool isDeleteFiles, bool isZip, string path, bool isOverride, bool isUseTable, string userKeyPrefix, RequestBody body)
        {
            //返回“运行结果”和“错误信息”的字符串数组
            NameValueCollection retval;

            try
            {
                BackupUtility.RecoverySite(publishmentSystemId, isDeleteChannels, isDeleteTemplates, isDeleteFiles, isZip, PageUtils.UrlDecode(path), isOverride, isUseTable, body.AdministratorName);

                body.AddSiteLog(publishmentSystemId, "恢复备份数据", body.AdministratorName);

                retval = AjaxManager.GetWaitingTaskNameValueCollection("数据恢复成功!", string.Empty, string.Empty);

                //retval = new string[] { "数据恢复成功!", string.Empty, string.Empty };
            }
            catch (Exception ex)
            {
                //retval = new string[] { string.Empty, ex.Message, string.Empty };
                retval = AjaxManager.GetWaitingTaskNameValueCollection(string.Empty, ex.Message, string.Empty);
                LogUtils.AddErrorLog(ex);
            }

            return retval;
        }
    }
}
