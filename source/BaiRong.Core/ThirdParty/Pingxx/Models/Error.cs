using System;
using Newtonsoft.Json;

namespace Pingpp.Models
{
    public class Error
    {
        [JsonProperty("type")]
        public string ErrorType { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

    }
}
