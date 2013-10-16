using Newtonsoft.Json;
using System;

namespace Simploud.Api.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Account
    {
        [JsonProperty(PropertyName = "uid")]
        public string Id { get; internal set; }

        [JsonProperty(PropertyName = "referral_link")]
        public string ReferralLink { get; internal set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; internal set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; internal set; }

        [JsonProperty(PropertyName = "country")]
        public string Country { get; internal set; }

        [JsonProperty(PropertyName = "quota_info")]
        public Quota Quota { get; internal set; }
    }    
}