using System;
using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.ImportExport;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace siteserver
{
    public class TaskBackup
    {
        public static bool Execute(TaskInfo taskInfo)
        {
            var taskBackupInfo = new TaskBackupInfo(taskInfo.ServiceParameters);

            if (taskInfo.PublishmentSystemID != 0)
            {
                return BackupByPublishmentSystemID(taskInfo, taskInfo.PublishmentSystemID, taskBackupInfo.BackupType);
            }
            else
            {
                List<int> publishmentSystemIDArrayList = null;
                if (taskBackupInfo.IsBackupAll)
                {
                    publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
                }
                else
                {
                    publishmentSystemIDArrayList = TranslateUtils.StringCollectionToIntList(taskBackupInfo.PublishmentSystemIDCollection);
                }
                foreach (int publishmentSystemID in publishmentSystemIDArrayList)
                {
                    BackupByPublishmentSystemID(taskInfo, publishmentSystemID, taskBackupInfo.BackupType);
                }
            }

            return true;
        }

        private static bool BackupByPublishmentSystemID(TaskInfo taskInfo, int publishmentSystemID, EBackupType backupType)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
            if (publishmentSystemInfo == null)
            {
                ExecutionUtils.LogError(taskInfo, new Exception("无法找到对应站点"));
                return false;
            }

            var filePath = PathUtility.GetBackupFilePath(publishmentSystemInfo, backupType);
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            FileUtils.DeleteFileIfExists(filePath);

            if (backupType == EBackupType.Templates)
            {
                BackupUtility.BackupTemplates(publishmentSystemInfo.PublishmentSystemId, filePath);
            }
            else if (backupType == EBackupType.ChannelsAndContents)
            {
                BackupUtility.BackupChannelsAndContents(publishmentSystemInfo.PublishmentSystemId, filePath);
            }
            else if (backupType == EBackupType.Files)
            {
                BackupUtility.BackupFiles(publishmentSystemInfo.PublishmentSystemId, filePath);
            }
            else if (backupType == EBackupType.Site)
            {
                BackupUtility.BackupSite(publishmentSystemInfo.PublishmentSystemId, filePath);
            }

            return true;
        }
    }
}
