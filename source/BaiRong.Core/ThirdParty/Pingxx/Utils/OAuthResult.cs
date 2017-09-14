using Newtonsoft.Json;

namespace Pingpp.Utils
{
    internal class OAuthResult
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public int? ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("openid")]
        public string Openid { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

    }
}