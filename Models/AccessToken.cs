using Newtonsoft.Json;
using System;

namespace Simploud.Api.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AccessToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string access_token { get; internal set; }

        [JsonProperty(PropertyName = "expires_in")]
        public int expires_in { get; internal set; }

        [JsonProperty(PropertyName = "scope")]
        public string scope { get; internal set; }

        [JsonProperty(PropertyName = "token_type")]
        public string token_type { get; internal set; }
    }
}
