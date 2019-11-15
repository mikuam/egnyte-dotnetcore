using Newtonsoft.Json;
using System;

namespace Egnyte.Api.Audit
{
    public class LoginAuditResponseItem : IAuditResponseItem
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "user_id")]
        public int UserID { get; set; }
        [JsonProperty(PropertyName = "access")]
        public string Access { get; set; }
        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
        [JsonProperty(PropertyName = "event")]
        public string Event { get; set; }
        [JsonProperty(PropertyName = "ip_address")]
        public string IPAddress { get; set; }
    }
}
