using System;
using System.Collections.Generic;
using System.Text;

namespace Egnyte.Api.Audit
{
    public class AuditReport
    {
        internal AuditReport(AuditReportEnum report, string id)
        {
            Report = report;
            Id = id;
        }

        public AuditReportEnum Report { get; }
        public string Id { get; }
    }
}
