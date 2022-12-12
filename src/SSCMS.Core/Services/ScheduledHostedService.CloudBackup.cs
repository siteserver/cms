using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Util;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService
    {
        private async Task CloudBackupAsync(ScheduledTask task)
        {
            var context = await CloudSyncAsync(task);
            if (context == null) return;

            // todo: delete
            // if (_settingsManager.DatabaseType != DatabaseType.SQLite)
            // {
            var console = new FakeConsoleUtils();
            var tree = new Tree(_settingsManager, "sscms-data");
            DirectoryUtils.DeleteDirectoryIfExists(tree.DirectoryPath);
            DirectoryUtils.CreateDirectoryIfNotExists(tree.DirectoryPath);
            var errorLogFilePath = PathUtils.Combine(tree.DirectoryPath, "sscms-task.error.log");
            FileUtils.DeleteFileIfExists(errorLogFilePath);

            await _databaseManager.BackupAsync(console, null, null, 0, 1000, tree, errorLogFilePath);

            // await AddSyncDirectoryTasksAsync(_settingsManager.ContentRootPath, tree.DirectoryPath, context);

            var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, CloudManager.DataZipFileName);
            FileUtils.DeleteFileIfExists(filePath);
            _pathManager.CreateZip(filePath, tree.DirectoryPath);

            var dataKey = StringUtils.TrimSlash(PageUtils.Combine(context.StoragePrefix, CloudManager.DataZipFileName));
            var result = context.Client.PutObject(context.Credentials.BucketName, dataKey, filePath);
            context.Client.SetObjectAcl(context.Credentials.BucketName, dataKey, CannedAccessControlList.Private);
            // }

            var theFile = new FileInfo(filePath);
            context.Size += theFile.Length;
            var size = context.Size / 1048576;

            await _cloudManager.BackupAsync(size);
        }
    }
}
