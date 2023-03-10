using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class FormManager : IFormManager
    {
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ICacheManager _cacheManager;
        private readonly ISmsManager _smsManager;
        private readonly IMailManager _mailManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFormDataRepository _formDataRepository;

        public FormManager(IPathManager pathManager, IDatabaseManager databaseManager, ICacheManager cacheManager, ISmsManager smsManager, IMailManager mailManager, IChannelRepository channelRepository, IContentRepository contentRepository, ITableStyleRepository tableStyleRepository, IFormRepository formRepository, IFormDataRepository formDataRepository)
        {
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _cacheManager = cacheManager;
            _smsManager = smsManager;
            _mailManager = mailManager;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
            _formRepository = formRepository;
            _formDataRepository = formDataRepository;
        }

        public async Task SendNotifyAsync(Form form, List<TableStyle> styles, FormData data)
        {
            if (form.IsAdministratorSmsNotify &&
                !string.IsNullOrEmpty(form.AdministratorSmsNotifyTplId) &&
                !string.IsNullOrEmpty(form.AdministratorSmsNotifyMobile))
            {
                var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
                if (isSmsEnabled)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(form.AdministratorSmsNotifyKeys))
                    {
                        var keys = form.AdministratorSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (StringUtils.EqualsIgnoreCase(key, nameof(FormData.Id)))
                            {
                                parameters.Add(key, data.Id.ToString());
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, nameof(FormData.CreatedDate)))
                            {
                                if (data.CreatedDate.HasValue)
                                {
                                    parameters.Add(key, data.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = data.Get<string>(key);
                                parameters.Add(key, value);
                            }
                        }
                    }

                    await _smsManager.SendSmsAsync(form.AdministratorSmsNotifyMobile,
                        form.AdministratorSmsNotifyTplId, parameters);
                }
            }

            if (form.IsAdministratorMailNotify &&
                !string.IsNullOrEmpty(form.AdministratorMailNotifyAddress))
            {
                var isMailEnabled = await _mailManager.IsMailEnabledAsync();
                if (isMailEnabled)
                {
                    var templateHtml = await _pathManager.GetMailTemplateHtmlAsync();
                    var listHtml = await _pathManager.GetMailListHtmlAsync();

                    var keyValueList = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("编号", data.Guid)
                    };
                    if (data.CreatedDate.HasValue)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>("提交时间",
                            data.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm")));
                    }

                    foreach (var style in styles)
                    {
                        keyValueList.Add(new KeyValuePair<string, string>(style.DisplayName,
                            data.Get<string>(style.AttributeName)));
                    }

                    if (data.ChannelId > 0)
                    {
                        var channelName = await _channelRepository.GetChannelNameNavigationAsync(data.SiteId, data.ChannelId);
                        keyValueList.Add(new KeyValuePair<string, string>("所在栏目页", channelName));
                    }
                    if (data.ContentId > 0)
                    {
                        var content = await _contentRepository.GetAsync(data.SiteId, data.ChannelId, data.ContentId);
                        var title = content != null ? content.Title : string.Empty;
                        keyValueList.Add(new KeyValuePair<string, string>("所在内容页", title));
                    }

                    var list = new StringBuilder();
                    foreach (var kv in keyValueList)
                    {
                        list.Append(listHtml.Replace("{{key}}", kv.Key).Replace("{{value}}", kv.Value));
                    }

                    var subject = !string.IsNullOrEmpty(form.AdministratorMailTitle) ? form.AdministratorMailTitle : "[SSCMS] 通知邮件";
                    var htmlBody = templateHtml
                        .Replace("{{title}}", form.Title)
                        .Replace("{{list}}", list.ToString());
                    htmlBody = "<style>.link-content{display: none;}</style>" + htmlBody;

                    await _mailManager.SendMailAsync(form.AdministratorMailNotifyAddress, subject,
                        htmlBody);
                }
            }

            if (form.IsUserSmsNotify &&
                !string.IsNullOrEmpty(form.UserSmsNotifyTplId) &&
                !string.IsNullOrEmpty(form.UserSmsNotifyMobileName))
            {
                var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
                if (isSmsEnabled)
                {
                    var parameters = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(form.UserSmsNotifyKeys))
                    {
                        var keys = form.UserSmsNotifyKeys.Split(',');
                        foreach (var key in keys)
                        {
                            if (StringUtils.EqualsIgnoreCase(key, nameof(FormData.Id)))
                            {
                                parameters.Add(key, data.Id.ToString());
                            }
                            else if (StringUtils.EqualsIgnoreCase(key, nameof(FormData.CreatedDate)))
                            {
                                if (data.CreatedDate.HasValue)
                                {
                                    parameters.Add(key, data.CreatedDate.Value.ToString("yyyy-MM-dd HH:mm"));
                                }
                            }
                            else
                            {
                                var value = data.Get<string>(key);
                                parameters.Add(key, value);
                            }
                        }
                    }

                    var mobile = data.Get<string>(form.UserSmsNotifyMobileName);
                    if (!string.IsNullOrEmpty(mobile))
                    {
                        await _smsManager.SendSmsAsync(mobile, form.UserSmsNotifyTplId, parameters);
                    }
                }
            }
        }
    }
}