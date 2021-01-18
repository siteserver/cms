using System.Collections.Specialized;
using Newtonsoft.Json;
using SqlKata;
using SSCMS.Models;

namespace SSCMS.Parse
{
    public class Dynamic
    {
        public int SiteId { get; set; }
        public int ChannelId { get; set; }
        public int ContentId { get; set; }
        public int TemplateId { get; set; }
        public string ElementId { get; set; }
        public string LoadingTemplate { get; set; }
        public string YesTemplate { get; set; }
        public string NoTemplate { get; set; }
        public bool IsInline { get; set; }
        public string OnBeforeSend { get; set; }
        public string OnSuccess { get; set; }
        public string OnComplete { get; set; }
        public string OnError { get; set; }
        public string Settings { get; set; }

        [JsonIgnore]
        public int Page { get; set; }

        [JsonIgnore]
        public NameValueCollection QueryString { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public Query Query { get; set; }
    }
}
