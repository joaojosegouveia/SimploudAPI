using Newtonsoft.Json;

namespace Simploud.Api.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Quota
    {
        [JsonProperty(PropertyName = "quota")]
        public long Total { get; internal set; }

        [JsonProperty(PropertyName = "shared")]
        public long Shared { get; internal set; }

        [JsonProperty(PropertyName = "normal")]
        public long Normal { get; internal set; }

        [JsonProperty(PropertyName = "available")]
        public long Available { get; internal set; }
    }
}