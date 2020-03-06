using System.Collections.Specialized;
using Newtonsoft.Json;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.StlParser.Model
{
    public partial class DynamicInfo
    {
        private ISettingsManager _settingsManager;

        public static DynamicInfo GetDynamicInfo(ISettingsManager settingsManager, string value, int page, User user, string pathAndQuery)
        {
            var dynamicInfo = TranslateUtils.JsonDeserialize<DynamicInfo>(settingsManager.Decrypt(value));
            dynamicInfo._settingsManager = settingsManager;
            if (dynamicInfo.ChannelId == 0)
            {
                dynamicInfo.ChannelId = dynamicInfo.SiteId;
            }
            dynamicInfo.User = user;
            dynamicInfo.QueryString =
                PageUtils.GetQueryStringFilterXss(pathAndQuery);
            dynamicInfo.QueryString.Remove("siteId");

            dynamicInfo.Page = page;

            return dynamicInfo;
        }

        public string ElementName { get; set; }
        public int SiteId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public int TemplateId { get; set; }
        public string AjaxDivId { get; set; }
        public string LoadingTemplate { get; set; }
        public string SuccessTemplate { get; set; }
        public string FailureTemplate { get; set; }
        public string OnBeforeSend { get; set; }
        public string OnSuccess { get; set; }
        public string OnComplete { get; set; }
        public string OnError { get; set; }
        public string ElementValues { get; set; }

        [JsonIgnore]
        public int Page { get; set; }

        [JsonIgnore]
        public NameValueCollection QueryString { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
