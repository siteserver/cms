using System;
using Newtonsoft.Json;
using Pingpp.Net;

namespace Pingpp.Models
{
    public class SmsCode : Pingpp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("validated")]
        public bool Validated { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        private const string BaseUrl = "/v1/sms_codes";

        public static SmsCode Retrieve(string smsId)
        {
            var url = string.Format("{0}/{1}", BaseUrl, smsId);
            var sms = Requestor.DoRequest(url, "GET");
            return Mapper<SmsCode>.MapFromJson(sms);
        }
    }
}
