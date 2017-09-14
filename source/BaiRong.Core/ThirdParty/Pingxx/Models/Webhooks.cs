using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pingpp.Exception;
using Pingpp.Net;

namespace Pingpp.Models
{
    public class Webhooks : Pingpp
    {
        public static Event ParseWebhook(string events)
        {
            var evt = JObject.Parse(events);
            var obj = evt.SelectToken("object");
            if (events.Contains("object") && obj.ToString().Equals("event"))
            {
                return Mapper<Event>.MapFromJson(events);       
            }
            throw new PingppException("It isn't a json string of event object");
        }
    }
}
