﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Pingpp.Net;

namespace Pingpp.Models
{
    public class Customs : Pingpp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("trade_no")]
        public string TradeNo { get; set; }

        [JsonProperty("customs_code")]
        public string CustomsCode { get; set; }

        [JsonProperty("amount")]
        public int? Amount { get; set; }

        [JsonProperty("charge")]
        public string Charge { get; set; }

        [JsonProperty("transport_amount")]
        public int? TransportAmount { get; set; }

        [JsonProperty("is_split")]
        public bool IsSplit { get; set; }

        [JsonProperty("sub_order_no")]
        public string SubOrderNo { get; set; }

        [JsonProperty("extra")]
        public Dictionary<string, object> Extra { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("time_succeeded")]
        public int? TimeSucceeded { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("failure_code")]
        public string FailureCode { get; set; }

        [JsonProperty("failure_msg")]
        public string FailureMsg { get; set; }

        [JsonProperty("transaction_no")]
        public string TransactionNo { get; set; }

        private const string BaseUrl = "/v1/customs";

        public static Customs Create(Dictionary<string, object> cuParams)
        {
            var customs = Requestor.DoRequest(BaseUrl, "POST", cuParams, false);
            return Mapper<Customs>.MapFromJson(customs);
        }

        public static Customs Retrieve(string cuId)
        {
            var url = string.Format("{0}/{1}", BaseUrl, cuId);
            var customs = Requestor.DoRequest(url, "GET");
            return Mapper<Customs>.MapFromJson(customs);
        }
    }
}