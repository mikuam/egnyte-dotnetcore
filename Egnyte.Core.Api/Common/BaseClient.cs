using System;
using System.Net.Http;

namespace Egnyte.Api.Common
{
    public class BaseClient
    {
        const string basePath = "{0}.egnyte.com";
        const string baseSchema = "https";
        const int basePort = 443;

        internal readonly HttpClient httpClient;

        internal readonly string domain;

        internal readonly string host;

        /// <summary>
        /// Insertion point for logging of requests to API.
        /// Occurs just prior to sending the request.
        /// </summary>
        /// <value>Returns a unique value related to the request if needed in the After functions</value>
        public static Func<HttpRequestMessage, HttpClient, object> BeforeRequest { get; set; }
        /// <summary>
        /// Insertion point for logging of requests to API. 
        /// Occurs after the response is received before any handling of status or content.
        /// </summary>
        public static Action<object, HttpRequestMessage, HttpResponseMessage, string> AfterResponse { get; set; }
        /// <summary>
        /// Insertion point for logging of requests to API. 
        /// Occurs after an exception due to status or content of the response.
        /// </summary>
        public static Action<object, HttpRequestMessage, Exception> AfterException { get; set; }


        internal BaseClient(HttpClient httpClient, string domain = "", string host = "")
        {
            this.httpClient = httpClient;
            this.domain = domain;
            this.host = host;
        }

        internal UriBuilder BuildUri(string method, string query = null)
        {
            var userHost = string.IsNullOrWhiteSpace(host)
                ? string.Format(basePath, domain)
                : host;

            UriBuilder ub = new UriBuilder(baseSchema, userHost, basePort, method);
            if (query != null)
                ub.Query = query;

            return ub;
        }
    }
}
