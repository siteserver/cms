using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pingpp.Net;

namespace Pingpp.Models
{
    public class Event : Pingpp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("data")]
        public Dictionary<string, object> Data { get; set; }

        [JsonProperty("pending_webhooks")]
        public int? PendingWebhooks { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }

        private const string BaseUrl = "/v1/events";

        public static Event Retrieve(string id)
        {
            var url = string.Format("{0}/{1}", BaseUrl, id);
            var evt = Requestor.DoRequest(url, "Get");
            return Mapper<Event>.MapFromJson(evt);
        }
     
    }
    

}
