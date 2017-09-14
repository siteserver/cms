using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pingpp.Net;

namespace Pingpp.Models
{
    public class Refund : Pingpp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("order_no")]
        public string OrderNo { get; set; }

        [JsonProperty("amount")]
        public int? Amount { get; set; }

        [JsonProperty("succeed")]
        public bool Succeed { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("time_succeed")]
        public int? TimeSucceed { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("failure_code")]
        public string FailureCode { get; set; }

        [JsonProperty("failure_msg")]
        public string FailureMsg { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("charge")]
        public string Charge { get; set; }

        private const string BaseUrl = "/v1/charges";

        public static Refund Create(string id, Dictionary<string, object> reParams)
        {
            var url = string.Format("{0}/{1}/refunds", BaseUrl, id);
            var re = Requestor.DoRequest(url, "POST", reParams, false);
            return Mapper<Refund>.MapFromJson(re);
        }

        public static Refund Retrieve(string chId, string reId)
        {
            var url = string.Format("/v1/charges/{0}/refunds/{1}", chId, reId);
            var re = Requestor.DoRequest(url, "Get");
            return Mapper<Refund>.MapFromJson(re);
        }

        public static RefundList List(string id, Dictionary<string, object> listParams = null)
        {
            var url = Requestor.FormatUrl(string.Format("/v1/charges/{0}/refunds", id), Requestor.CreateQuery(listParams));
            var refundList = Requestor.DoRequest(url, "Get");
            return Mapper<RefundList>.MapFromJson(refundList);
        }

    }
}
