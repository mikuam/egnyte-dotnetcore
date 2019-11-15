using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Egnyte.Api.Audit
{
    public class LoginAuditReportResponse : IAuditReportResponse
    {
        public string Status { get; set; } = "completed";

        [JsonProperty(PropertyName = "total_count")]
        public int TotalCount { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; }
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "events")]
        public List<LoginAuditResponseItem> Events { get; set; }

        List<IAuditResponseItem> IAuditReportResponse.Events => Events.OfType<IAuditResponseItem>().ToList();
    }
}
