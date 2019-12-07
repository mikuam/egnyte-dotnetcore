namespace Egnyte.Api.Audit
{
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Egnyte.Api.Common;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public class AuditClient : BaseClient
    {
        const string AuditReportMethod = "/pubapi/v1/audit";

        internal AuditClient(HttpClient httpClient, string domain = "", string host = "") : base(httpClient, domain, host) { }

        /// <summary>
        /// Creates login audit report
        /// </summary>
        /// <param name="format">Required. Determines format of audit report data</param>
        /// <param name="startDate">Required. Start of date range for report</param>
        /// <param name="endDate">Required. End of date range for report</param>
        /// <param name="events">Required. List of events to report on. At least one event must be specified</param>
        /// <param name="accessPoints">Optional. List of Egnyte access points covered by report.
        /// If not specified or empty then report will cover all access points</param>
        /// <param name="users">Optional. List of usernames to report on.
        /// If not specified or empty then report will cover all users</param>
        /// <returns>Id fo an audit report</returns>
        public async Task<AuditReport> CreateLoginAuditReport(
            AuditReportFormat format,
            DateTime startDate,
            DateTime endDate,
            List<string> events,
            List<AuditReportAccessPoint> accessPoints = null,
            List<string> users = null)
        {
            if (events == null || events.Count < 1)
            {
                throw new ArgumentException("At least one event must be specified.", nameof(events));
            }

            var uriBuilder = BuildUri(AuditReportMethod + "/logins");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetCreateLoginAuditReportContent(
                        format,
                        startDate,
                        endDate,
                        events,
                        accessPoints,
                        users),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<CreateLoginAuditReportResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new AuditReport(AuditReportEnum.Login, response.Data.Id);
        }

        string GetCreateLoginAuditReportContent(
            AuditReportFormat format,
            DateTime startDate,
            DateTime endDate,
            List<string> events,
            List<AuditReportAccessPoint> accessPoints = null,
            List<string> users = null)
        {
            var eventsContent = "["
                    + string.Join(",", events.Select(e => "\"" + e + "\""))
                    + "]";

            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append("\"format\": \"" + MapAuditReportFormat(format) + "\",")
                .Append(string.Format("\"date_start\": \"{0:yyyy-MM-ddTHH:mm:ssZ}\",", startDate))
                .Append(string.Format("\"date_end\": \"{0:yyyy-MM-ddTHH:mm:ssZ}\",", endDate))
                .Append("\"events\": " + eventsContent);

            if (accessPoints != null && accessPoints.Count > 0)
            {
                var accessPointsContent = "["
                + string.Join(",", accessPoints.Select(a => "\"" + MapAuditReportAccessPoint(a) + "\""))
                + "]";
                builder.Append(",\"access_points\": " + accessPointsContent);
            }

            if (users != null && users.Count > 0)
            {
                var usersContent = "["
                    + string.Join(",", users.Select(u => "\"" + u + "\""))
                    + "]";
                builder.Append(",\"users\": " + usersContent);
            }

            builder.Append("}");

            return builder.ToString();
        }

        string MapAuditReportFormat(AuditReportFormat format)
        {
            if (format == AuditReportFormat.CSV)
            {
                return "csv";
            }

            return "json";
        }

        string MapAuditReportAccessPoint(AuditReportAccessPoint accessPoint)
        {
            switch (accessPoint)
            {
                case AuditReportAccessPoint.Web:
                    return "web";
                case AuditReportAccessPoint.Mobile:
                    return "mobile";
                default:
                    return "ftp";
            }
        }



        /// <summary>
        /// Creates login audit report
        /// </summary>
        /// <param name="format">Required. Determines format of audit report data</param>
        /// <param name="startDate">Required. Start of date range for report</param>
        /// <param name="endDate">Required. End of date range for report</param>
        /// <param name="folders">Required. List of folders to report on. At least one folder must be specified</param>
        /// <param name="file">Optional. Name of file to report on</param>
        /// <param name="accessPoints">Optional. List of Egnyte access points covered by report.
        /// If not specified or empty then report will cover all access points</param>
        /// <param name="users">Optional. List of usernames to report on.
        /// If not specified or empty then report will cover all users</param>
        /// <returns>Id fo an audit report</returns>
        public async Task<AuditReport> CreateFileAuditReport(
            AuditReportFormat format,
            DateTime startDate,
            DateTime endDate,
            List<string> folders,
            string file = null,
            List<string> users = null,
            List<string> transactionTypes = null,
            bool suppress_emails = false)
        {
            if (string.IsNullOrWhiteSpace(file) && (folders == null || !folders.Any()))
            {
                throw new ArgumentException("Either a filename or at least one folder must be specified.", nameof(folders));
            }

            var uriBuilder = BuildUri(AuditReportMethod + "/files");
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri)
            {
                Content = new StringContent(
                    GetCreateFileAuditReportContent(
                        format,
                        startDate,
                        endDate,
                        folders,
                        file,
                        users,
                        transactionTypes,
                        suppress_emails),
                    Encoding.UTF8,
                    "application/json")
            };

            var serviceHandler = new ServiceHandler<CreateFileAuditReportResponse>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return new AuditReport(AuditReportEnum.File, response.Data.Id);
        }

        string GetCreateFileAuditReportContent(
            AuditReportFormat format,
            DateTime startDate,
            DateTime endDate,
            List<string> folders,
            string file,
            List<string> users = null,
            List<string> transactionTypes = null,
            bool suppress_emails = false)
        {
            if(string.IsNullOrWhiteSpace(file) && (folders == null || !folders.Any()))
            {
                throw new ArgumentException("Either folders or file must be specified");
            }

            var builder = new StringBuilder();
            builder
                .Append("{")
                .Append($"\"format\": \"{MapAuditReportFormat(format)}\",")
                .Append($"\"date_start\": \"{startDate:yyyy-MM-ddTHH:mm:ssZ}\",")
                .Append($"\"date_end\": \"{endDate:yyyy-MM-ddTHH:mm:ssZ}\"");

            if (!string.IsNullOrWhiteSpace(file))
            {
                builder.Append($",\"file\": \"{file}\"");
            }
            else
            {
                var foldersContent = "["
                    + string.Join(",", folders.Select(e => "\"" + e + "\""))
                    + "]";

                builder.Append($",\"folders\": {foldersContent}");
            }

            if (transactionTypes != null && transactionTypes.Count > 0)
            {
                var transactionTypesContent = "["
                    + string.Join(",", transactionTypes.Select(tt => $"\"{tt}\""))
                    + "]";
                builder.Append($",\"transaction_type\": {transactionTypesContent}");
            }

            if (users != null && users.Count > 0)
            {
                var usersContent = "["
                    + string.Join(",", users.Select(u => $"\"{u}\""))
                    + "]";
                builder.Append($",\"users\": {usersContent}");
            }

            builder.Append("}");

            return builder.ToString();
        }

        public async Task<string> CheckAuditReportStatus(string id)
        {
            var uriBuilder = BuildUri(AuditReportMethod + $"/jobs/{id}");
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            var serviceHandler = new ServiceHandler<string>(httpClient);
            var response = await serviceHandler.SendRequestAsync(httpRequest).ConfigureAwait(false);

            return response.Data;
        }

        public async Task<IAuditReportResponse> CheckAuditReportStatus(AuditReport report)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            var json = await CheckAuditReportStatus(report.Id);

            if(json.StartsWith("{\"status\""))
            {
                return JsonConvert.DeserializeObject<AuditReportStatusResponse>(json);
            }

            switch (report.Report)
            {
                case AuditReportEnum.Login:
                    return JsonConvert.DeserializeObject<LoginAuditReportResponse>(json);

                case AuditReportEnum.File:
                    return JsonConvert.DeserializeObject<FileAuditReportResponse>(json);

                default:
                    break;
            }

            return null;
        }

        string GetResultQuery(
            int? offset,
            int? count)
        {
            var queryParams = new List<string>();

            if (offset.HasValue)
            {
                queryParams.Add("offset=" + offset);
            }

            if (count.HasValue)
            {
                queryParams.Add("count=" + count);
            }

            return string.Join("&", queryParams);
        }
        public async Task<IAuditReportResponse> RetrieveAuditReportResults(AuditReport report, int? offset = null, int? count = null)
        {
            if (report == null)
            {
                throw new ArgumentNullException(nameof(report));
            }

            var uriBuilder = BuildUri(AuditReportMethod + $"/json/{report.Id}", GetResultQuery(offset, count));
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

            switch (report.Report)
            {
                case AuditReportEnum.Login:
                    var serviceHandlerLogin = new ServiceHandler<LoginAuditReportResponse>(httpClient);
                    var responseLogin = await serviceHandlerLogin.SendRequestAsync(httpRequest).ConfigureAwait(false);

                    return responseLogin.Data;

                case AuditReportEnum.File:
                    var serviceHandlerFile = new ServiceHandler<FileAuditReportResponse>(httpClient);
                    var responseFile = await serviceHandlerFile.SendRequestAsync(httpRequest).ConfigureAwait(false);

                    return responseFile.Data;

                default:
                    break;
            }

            throw new NotSupportedException();
        }
    }
}
