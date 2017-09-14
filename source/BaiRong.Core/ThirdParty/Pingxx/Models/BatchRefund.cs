using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pingpp.Net;


namespace Pingpp.Models
{
    /// <summary>
    /// batch_refunds Batch refund批量退款
    /// </summary>
    public class BatchRefund : Pingpp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("batch_no")]
        public string BatchNo { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("charges")]
        public List<BatchRefundCharge> BatchRefundCharge { get; set; }

        [JsonProperty("refunds")]
        public RefundList Refunds { get; set; }

        [JsonProperty("refund_url")]
        public string RefundUrl { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("time_succeeded")]
        public string TimeSucceeded { get; set; }
        [JsonProperty("livemode")]
        public bool Livemode { get; set; }

        private const string BaseUrl = "/v1/batch_refunds";
        public static BatchRefund Create(Dictionary<string, object> bfParams)
        {
            var re = Requestor.DoRequest(BaseUrl, "POST", bfParams, false);
            return Mapper<BatchRefund>.MapFromJson(re);
        }

        public static BatchRefund Retrieve(string batchRefundId)
        {
            var url = string.Format("{0}/{1}", BaseUrl, batchRefundId);
            var re = Requestor.DoRequest(url, "GET");
            return Mapper<BatchRefund>.MapFromJson(re);
        }

        public static BatchRefundList List(Dictionary<string, object> listParams = null)
        {
            var url = Requestor.FormatUrl(BaseUrl, Requestor.CreateQuery(listParams));
            var re = Requestor.DoRequest(url, "GET");
            return Mapper<BatchRefundList>.MapFromJson(re);
        }
    }

    public class BatchRefundCharge
    {
        [JsonProperty("charge")]
        public string Charge { get; set; }
        [JsonProperty("amount")]
        public int Amount { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("funding_source", NullValueHandling = NullValueHandling.Ignore)]
        public string FundingSource { get; set; }
    }
}
