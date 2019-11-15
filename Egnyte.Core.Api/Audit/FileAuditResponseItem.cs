using Newtonsoft.Json;
using System;

namespace Egnyte.Api.Audit
{
    public class FileAuditResponseItem : IAuditResponseItem
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public int UserID { get; set; }
        [JsonProperty(PropertyName = "access")]
        public string Access { get; set; }
        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
        [JsonProperty(PropertyName = "file/folder")]
        public string File { get; set; }
        [JsonProperty(PropertyName = "target_path")]
        public string TargetPath { get; set; }
        [JsonProperty(PropertyName = "transaction")]
        public string Transaction { get; set; }
        [JsonProperty(PropertyName = "actionInfo")]
        public string ActionInfo { get; set; }

        public string CurrentPath => (TargetPath == "N/A" ? File : TargetPath);
    }
}
