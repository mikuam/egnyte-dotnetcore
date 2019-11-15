namespace Egnyte.Api.Common
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.IO;

    public class ServiceHandler<T> where T : class 
    {
        readonly HttpClient httpClient;

        /// <summary>
        /// Insertion point for logging of requests to API.
        /// Occurs just prior to sending the request.
        /// </summary>
        /// <value>Returns a unique value related to the request if needed in the After functions</value>
        public static Func<HttpRequestMessage, object> BeforeRequest { get; set; }
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

        public ServiceHandler(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<ServiceResponse<T>> SendRequestAsync(HttpRequestMessage request)
        {
            object o = null;
            try
            {
                request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);

                if (BeforeRequest != null)
                {
                    o = BeforeRequest.Invoke(request);
                }

                var response = await httpClient.SendAsync(request).ConfigureAwait(false);
                var rawContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (AfterResponse != null)
                {
                    AfterResponse.Invoke(o, request, response, rawContent);
                }

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        if (typeof(T) == typeof(string))
                        {
                            return new ServiceResponse<T>
                            {
                                Data = rawContent as T,
                                Headers = response.GetResponseHeaders()
                            };
                        }

                        return new ServiceResponse<T>
                        {
                            Data = JsonConvert.DeserializeObject<T>(rawContent),
                            Headers = response.GetResponseHeaders()
                        };
                    }
                    catch (Exception e)
                    {
                        throw new EgnyteApiException(
                            rawContent,
                            response,
                            e);
                    }
                }

                throw new EgnyteApiException(
                        rawContent,
                        response);
            }
            catch(Exception e)
            {
                if (AfterException != null)
                {
                    AfterException.Invoke(o, request, e);
                }

                throw;
            }
        }

        public async Task<ServiceResponse<byte[]>> GetFileToDownload(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request);
            var bytes = await response.Content.ReadAsByteArrayAsync();
            return new ServiceResponse<byte[]>
                       {
                           Data = bytes,
                           Headers = response.GetResponseHeaders()
                       };
        }

        public async Task<ServiceResponse<Stream>> GetFileToDownloadAsStream(HttpRequestMessage request)
        {
            request.RequestUri = ApplyAdditionalUrlMapping(request.RequestUri);
            var response = await this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            var stream = await response.Content.ReadAsStreamAsync();
            
            return new ServiceResponse<Stream>
            {
                Data = stream,
                Headers = response.GetResponseHeaders()
            };
        }

        Uri ApplyAdditionalUrlMapping(Uri requestUri)
        {
            var url = requestUri.ToString();
            url = url.Replace("[", "%5B")
                     .Replace("]", "%5D");
            return new Uri(url);
        }
    }
}
