using Newtonsoft.Json;
using System.Collections.Generic;

namespace Egnyte.Api.Audit
{
    public interface IAuditReportResponse
    {
        string Status { get; }
        int TotalCount { get; }
        int Offset { get; }
        int Count { get; }
        List<IAuditResponseItem> Events { get; }
    }
}
