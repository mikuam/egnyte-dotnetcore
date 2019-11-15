using Newtonsoft.Json;

namespace Egnyte.Api.Audit
{
    class CreateFileAuditReportResponse
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
