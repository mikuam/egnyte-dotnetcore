using System.Collections.Generic;
using Newtonsoft.Json;

namespace Egnyte.Api.Audit
{
    public class AuditReportStatusResponse : IAuditReportResponse
    {
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }

        int IAuditReportResponse.TotalCount => 0;

        int IAuditReportResponse.Offset => 0;

        int IAuditReportResponse.Count => 0;

        List<IAuditResponseItem> IAuditReportResponse.Events => new List<IAuditResponseItem>();
    }
}
