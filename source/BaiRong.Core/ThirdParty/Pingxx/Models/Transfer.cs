using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pingpp.Net;

namespace Pingpp.Models
{
    public class Transfer : Pingpp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("time_transferred")]
        public int? TimeTransferred { get; set; }

        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("order_no")]
        public string OrderNo { get; set; }

        [JsonProperty("amount")]
        public int? Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("transaction_no")]
        public string TransactionNo { get; set; }

        [JsonProperty("extra")]
        public Dictionary<string, object> Extra { get; set; }

        [JsonProperty("failure_code")]
        public string FailureCode { get; set; }

        [JsonProperty("failure_msg")]
        public string FailureMsg { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        private const string BaseUrl = "/v1/transfers";

        public static Transfer Create(Dictionary<string, object> trParams)
        {
            var transfer = Requestor.DoRequest(BaseUrl, "POST", trParams, false);
            return Mapper<Transfer>.MapFromJson(transfer);
        }

        public static Transfer Retrieve(string trId)
        {
            var url = string.Format("{0}/{1}", BaseUrl, trId);
            var transfer = Requestor.DoRequest(url, "GET");
            return Mapper<Transfer>.MapFromJson(transfer);
        }

        public static Transfer Cancel(string trId)
        {
            var url = string.Format("{0}/{1}", BaseUrl, trId);
            var transfer = Requestor.DoRequest(url, "PUT", new Dictionary<string,object>{{"status", "canceled"}});
            return Mapper<Transfer>.MapFromJson(transfer);
        }

        public static TransferList List(Dictionary<string, object> listParams = null)
        {
            var url = Requestor.FormatUrl(BaseUrl, Requestor.CreateQuery(listParams));
            var transferList = Requestor.DoRequest(url, "GET");
            return Mapper<TransferList>.MapFromJson(transferList);
        }   
    }
}
