using Newtonsoft.Json;

namespace SiteServer.CMS.Api.V1
{
    public class OResponse
    {
        public OResponse(object value)
        {
            Value = value;
        }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "first", NullValueHandling = NullValueHandling.Ignore)]
        public string First { get; set; }

        [JsonProperty(PropertyName = "prev", NullValueHandling = NullValueHandling.Ignore)]
        public string Prev { get; set; }

        [JsonProperty(PropertyName = "next", NullValueHandling = NullValueHandling.Ignore)]
        public string Next { get; set; }

        [JsonProperty(PropertyName = "last", NullValueHandling = NullValueHandling.Ignore)]
        public string Last { get; set; }

        [JsonProperty(PropertyName = "count", NullValueHandling = NullValueHandling.Ignore)]
        public int? Count { get; set; }
    }
}