using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    /// <summary>
    /// 云助手已移除。本类保留为不联网的本地空实现，类名、构造函数与公共成员保持不变，
    /// 以保证 ICloudManager 及 ICensorManager/ISpellManager/IMailManager/ISmsManager/IStorageManager/IVodManager
    /// 仍可被注入；插件可通过 IPluginConfigureServices 覆盖这些服务的注册。
    /// </summary>
    public partial class CloudManager : ICloudManager
    {
        internal const string RemovedMessage = "云助手已移除";
        public const string DomainDns = "https://a.sscms.net";
        public const string DataZipFileName = "sscms-data.zip";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ICacheManager _cacheManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IConfigRepository _configRepository;
        private readonly IStorageFileRepository _storageFileRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public CloudManager(ISettingsManager settingsManager, IPathManager pathManager, ICacheManager cacheManager, IDatabaseManager databaseManager, IConfigRepository configRepository, IStorageFileRepository storageFileRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _cacheManager = cacheManager;
            _databaseManager = databaseManager;
            _configRepository = configRepository;
            _storageFileRepository = storageFileRepository;
            _errorLogRepository = errorLogRepository;
        }

        [Obsolete(RemovedMessage)]
        public static string GetCloudUrl(string relatedUrl) => PageUtils.Combine(CloudUtils.CloudApiHost, "v7/cms", relatedUrl);

        // ---------- 账号 ----------

        public Task<bool> IsAuthenticationAsync() => Task.FromResult(false);

        public Task<CloudType> GetCloudTypeAsync() => Task.FromResult(CloudType.Free);

        [Obsolete(RemovedMessage)]
        public Task SetAuthenticationAsync(int userId, string userName, string mobile, string token) => Task.CompletedTask;

        [Obsolete(RemovedMessage)]
        public Task SetCloudTypeAsync(CloudType cloudType, DateTime expirationDate) => Task.CompletedTask;

        public Task RemoveAuthenticationAsync() => Task.CompletedTask;

        // ---------- 下载与存储凭证 ----------

        public class GetDownloadUrlRequest
        {
            public string ResourceType { get; set; }
            public string UserName { get; set; }
            public string Name { get; set; }
            public string Version { get; set; }
        }

        public class GetDownloadUrlResult
        {
            public string DownloadUrl { get; set; }
        }

        // 不再通过云助手获取付费下载地址，统一返回官方免费下载地址
        public Task<string> GetThemeDownloadUrlAsync(string userName, string name) =>
            Task.FromResult(CloudUtils.Dl.GetThemesDownloadUrl(userName, name));

        public Task<string> GetExtensionDownloadUrlAsync(string userName, string name, string version) =>
            Task.FromResult(CloudUtils.Dl.GetExtensionsDownloadUrl(userName, name, version));

        public Task<OssCredentials> GetOssCredentialsAsync() => throw new NotSupportedException(RemovedMessage);

        // ---------- 云备份 / 免版权图库 ----------

        public Task<bool> IsImagesAsync() => Task.FromResult(false);

        public Task BackupAsync(long size) => throw new NotSupportedException(RemovedMessage);

        public Task RestoreAsync(string restoreId, string backupId) => throw new NotSupportedException(RemovedMessage);

        public int GetRestoreProgress(string restoreId) => 0;

        public static string GetBackupPrefixKey(int userId, string backupId) => $"backups/{userId}/{backupId}/";

        // ---------- 违规检测 ----------

        public class CensorAddWhiteListRequest
        {
            public string Word { get; set; }
        }

        [Obsolete] public Task<bool> IsTextEnabledAsync() => Task.FromResult(false);
        [Obsolete] public Task<bool> IsImageEnabledAsync() => Task.FromResult(false);
        [Obsolete] public Task<CensorResult> ScanText(string text) => Task.FromResult(EmptyCensorResult());
        [Obsolete] public Task<CensorResult> ScanImage(string imagePath) => Task.FromResult(EmptyCensorResult());

        public Task<CensorSettings> GetCensorSettingsAsync() => Task.FromResult(new CensorSettings());

        public Task<CensorResult> CensorTextAsync(string text) => Task.FromResult(EmptyCensorResult());

        public Task<(bool success, string errorMessage)> AddCensorWhiteListAsync(string word) =>
            Task.FromResult((false, "未启用违规检测功能"));

        private static CensorResult EmptyCensorResult() => new CensorResult { Success = true, IsBadWords = false, BadWords = new List<BadWord>() };

        // ---------- 文本纠错 ----------

        public class SpellAddWhiteListRequest
        {
            public string Word { get; set; }
        }

        public Task<SpellSettings> GetSpellSettingsAsync() => Task.FromResult(new SpellSettings());

        public Task<SpellResult> SpellingCheckAsync(string text) =>
            Task.FromResult(new SpellResult { Success = true, IsErrorWords = false, ErrorWords = new List<ErrorWord>() });

        public Task<(bool success, string errorMessage)> AddSpellWhiteListAsync(string word) =>
            Task.FromResult((false, "未启用文本纠错功能"));

        // ---------- 短信 ----------

        private const string SmsDisabled = "未启用短信功能";

        [Obsolete] public Task<bool> IsEnabledAsync() => Task.FromResult(false);

        [Obsolete]
        public Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters) =>
            Task.FromResult((false, SmsDisabled));

        [Obsolete]
        public Task<(bool success, string errorMessage)> SendAsync(string phoneNumbers, SmsCodeType codeType, int code) =>
            Task.FromResult((false, SmsDisabled));

        public Task<bool> IsSmsEnabledAsync() => Task.FromResult(false);

        public Task<SmsSettings> GetSmsSettingsAsync() => Task.FromResult(new SmsSettings());

        public Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, string templateCode, Dictionary<string, string> parameters) =>
            Task.FromResult((false, SmsDisabled));

        public Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, Dictionary<string, string> parameters = null) =>
            Task.FromResult((false, SmsDisabled));

        public Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, int code) =>
            Task.FromResult((false, SmsDisabled));

        public Task<(bool success, string errorMessage)> SendSmsAsync(string phoneNumbers, SmsCodeType codeType, string templateCode, Dictionary<string, string> parameters) =>
            Task.FromResult((false, SmsDisabled));

        // ---------- 邮件 ----------

        private const string MailDisabled = "未启用邮件功能";

        public class SendMailRequest
        {
            public string FromAlias { get; set; }
            public string Mail { get; set; }
            public string Subject { get; set; }
            public string HtmlBody { get; set; }
        }

        [Obsolete]
        public Task<(bool success, string errorMessage)> SendAsync(string mail, string subject, string htmlBody) =>
            Task.FromResult((false, MailDisabled));

        public Task<bool> IsMailEnabledAsync() => Task.FromResult(false);

        public Task<MailSettings> GetMailSettingsAsync() => Task.FromResult(new MailSettings());

        public Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string htmlBody) =>
            Task.FromResult((false, MailDisabled));

        public Task<(bool success, string errorMessage)> SendMailAsync(string mail, string subject, string url, List<KeyValuePair<string, string>> items) =>
            Task.FromResult((false, MailDisabled));

        // 保留：使用传入的 IMailManager（可能由插件提供）发送内容变更通知
        public static async Task SendContentChangedMail(IPathManager pathManager, IMailManager mailManager, IErrorLogRepository errorLogRepository, Site site, Content content, string channelNames, string userName, bool isEdit)
        {
            try
            {
                var mailSettings = await mailManager.GetMailSettingsAsync();
                if (mailSettings == null || !mailSettings.IsMail) return;
                if (isEdit ? !mailSettings.IsMailContentEdit : !mailSettings.IsMailContentAdd) return;

                var action = isEdit ? "修改" : "添加";
                var url = await pathManager.GetContentUrlByIdAsync(site, content, false);
                var items = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("内容标题", content.Title),
                    new KeyValuePair<string, string>("栏目", channelNames),
                    new KeyValuePair<string, string>("内容Id", content.Id.ToString()),
                    new KeyValuePair<string, string>($"{action}时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm")),
                    new KeyValuePair<string, string>("执行人", userName)
                };

                await mailManager.SendMailAsync(mailSettings.MailAddress, $"{action}内容", url, items);
            }
            catch (Exception ex)
            {
                await errorLogRepository.AddErrorLogAsync(ex);
            }
        }

        // ---------- 云存储 ----------

        private static SyncTaskSummary EmptyTaskSummary() => new SyncTaskSummary(new List<SyncTaskSummaryItem>(), 0);

        [Obsolete] public void ClearAllTask(int siteId) { }
        [Obsolete] public SyncTaskSummary GetTaskSummary(int siteId) => EmptyTaskSummary();
        [Obsolete] public Task<bool> IsSyncAsync(int siteId, SyncType syncType) => Task.FromResult(false);
        [Obsolete] public Task<bool> IsAutoSyncAsync(int siteId, SyncType syncType) => Task.FromResult(false);
        [Obsolete] public Task SyncAllAsync(int siteId) => Task.CompletedTask;
        [Obsolete] public Task<(bool, string)> SyncAsync(int siteId, string filePath) => Task.FromResult((false, string.Empty));

        public Task<bool> IsStorageAsync() => Task.FromResult(false);
        public void ClearStorageTasks(int siteId) { }
        public SyncTaskSummary GetStorageTasks(int siteId) => EmptyTaskSummary();
        public Task<string> GetStorageUrlAsync(int siteId) => Task.FromResult(string.Empty);
        public Task<bool> IsStorageAsync(int siteId, SyncType syncType) => Task.FromResult(false);
        public Task<bool> IsAutoStorageAsync(int siteId, SyncType syncType) => Task.FromResult(false);
        public Task StorageAllAsync(int siteId) => Task.CompletedTask;
        public Task<(bool, string)> StorageAsync(int siteId, string filePath) => Task.FromResult((false, string.Empty));

        // ---------- 云视频 ----------

        public Task<VodSettings> GetVodSettingsAsync() => Task.FromResult(new VodSettings { IsVod = false });

        public Task<VodResult> UploadVodAsync(string filePath) =>
            Task.FromResult(new VodResult { Success = false, ErrorMessage = "未启用云视频功能" });
    }
}
