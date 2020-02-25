using System.Collections.Specialized;
using Newtonsoft.Json;
using SS.CMS.Abstractions;

namespace SS.CMS.StlParser.Model
{
    public partial class DynamicInfo
    {
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
