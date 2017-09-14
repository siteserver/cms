using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pingpp.Net;


namespace Pingpp.Models
{
    public class Card : Pingpp
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("object")]
        public string Object { get; set; }

        [JsonProperty("created")]
        public int? Created { get; set; }

        [JsonProperty("last4")]
        public string Last4 { get; set; }

        [JsonProperty("funding")]
        public string Funding { get; set; }

        [JsonProperty("brand")]
        public string Brand { get; set; }

        [JsonProperty("bank")]
        public string Bank { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        private const string BaseUrl = "/v1/customers";

        public static Card Create(string cusId, Dictionary<string, object> cardParams)
        {
            var url = string.Format("{0}/{1}/sources", BaseUrl, cusId);
            var card = Requestor.DoRequest(url, "POST", cardParams);
            return Mapper<Card>.MapFromJson(card);
        }

        public static Card Retrieve(string cusId, string cardId)
        {
            var url = string.Format("{0}/{1}/sources/{2}", BaseUrl, cusId, cardId);
            var card = Requestor.DoRequest(url, "GET");
            return Mapper<Card>.MapFromJson(card);
        }

        public static CardList List(string cusId, Dictionary<string, object> listParams = null)
        {
            var url = Requestor.FormatUrl(string.Format("{0}/{1}/sources", BaseUrl, cusId), Requestor.CreateQuery(listParams));
            var card = Requestor.DoRequest(url, "GET");
            return Mapper<CardList>.MapFromJson(card);
        }

        public static object Delete(string cusId, string cardId)
        {
            var url = string.Format("{0}/{1}/sources/{2}", BaseUrl, cusId, cardId);
            var card = Requestor.DoRequest(url, "DELETE");
            return JObject.Parse(card);
        }
    }

}

