using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Simploud.Api.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class File
    {
        [JsonProperty(PropertyName = "size")]
        public string Size { get; internal set; }

        [JsonProperty(PropertyName = "rev")]
        public string Revision { get; internal set; }

        [JsonProperty(PropertyName = "thumb_exists")]
        public bool ThumbnailExists { get; internal set; }

        [JsonProperty(PropertyName = "bytes")]
        public long Bytes { get; internal set; }

        [JsonProperty(PropertyName = "is_dir")]
        public bool IsDirectory { get; internal set; }

        [JsonProperty(PropertyName = "root")]
        public string Root { get; internal set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; internal set; }

        [JsonProperty(PropertyName = "mime_type")]
        public string MimeType { get; internal set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; internal set; }

        [JsonProperty(PropertyName = "contents")]
        public IEnumerable<File> Contents { get; internal set; }

        [JsonProperty(PropertyName = "modified")]
        public DateTime Modified { get; internal set; }

        //skydrive params

        [JsonProperty(PropertyName = "data")]
        public IEnumerable<File> Data { get; internal set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }

        [JsonProperty(PropertyName = "from")]
        public From From_data { get; internal set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        [JsonProperty(PropertyName = "description")]
        public string Description { get; internal set; }

        [JsonProperty(PropertyName = "parent_id")]
        public string ParentId { get; internal set; }

        [JsonProperty(PropertyName = "upload_location")]
        public string Upload_location { get; internal set; }

        [JsonProperty(PropertyName = "comments_count")]
        public long Comments_count { get; internal set; }

        [JsonProperty(PropertyName = "comments_enabled")]
        public bool Comments_enabled { get; internal set; }

        [JsonProperty(PropertyName = "is_embeddable")]
        public bool Is_embeddable { get; internal set; }

        [JsonProperty(PropertyName = "source")]
        public string Source { get; internal set; }

        [JsonProperty(PropertyName = "link")]
        public string Link { get; internal set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; internal set; }

        [JsonProperty(PropertyName = "shared_with")]
        public Shared_with Shared_with { get; internal set; }

        [JsonProperty(PropertyName = "created_time")]
        public string Created_time { get; internal set; }

        [JsonProperty(PropertyName = "updated_time")]
        public string Updated_time { get; internal set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class From
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; internal set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; internal set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Shared_with
    {
        [JsonProperty(PropertyName = "access")]
        public string Access { get; internal set; }
    }
}
