using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pingpp.Net;

namespace Pingpp.Models
{
    public class Identification : Pingpp
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("result_code")]
        public string ResultCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("paid")]
        public bool Paid { get; set; }

        [JsonProperty("data")]
        public Dictionary<string, object> Data { get; set; }

        private const string BaseUrl = "/v1/identification";

        public static Identification Identify(Dictionary<string, object> iParams)
        {
            var identified = Requestor.DoRequest(BaseUrl, "POST", iParams, false);
            return Mapper<Identification>.MapFromJson(identified);
        }

    }
}
