using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using NoteOne_Utility.Converters;
using NoteOne_Utility.Extensions;
using Windows.Foundation;

namespace NoteOne_Utility.Helpers
{
    public class HttpClientHelper
    {
        private static HttpClientHelper _instance;

        private HttpClientHelper()
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
            }
            MessageHandler = new HttpClientHandler();
            MessageHandler = new PlugInHandler(MessageHandler);
            CustomHeaderParameters = new Dictionary<string, string>();

            // Set User-Agent
            CustomHeaderParameters["user-agent"] = AppSettings.Instance[AppSettings.GLOBAL_USER_AGENT_CHROME].ToString();

            (MessageHandler as PlugInHandler).CustomHeaderParameters = CustomHeaderParameters;
            Client = new HttpClient(MessageHandler);
        }

        public static HttpClientHelper Instance
        {
            get
            {
                if (_instance == null) _instance = new HttpClientHelper();
                return _instance;
            }
        }

        public IDictionary<string, string> CustomHeaderParameters { get; private set; }
        public HttpMessageHandler MessageHandler { get; private set; }

        private HttpClient Client { get; set; }
        
        /// <summary>
        /// HTTP Get response
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IAsyncOperation<ResponseBody<string>> GetResponseStringAsync(Uri uri)
        {
            return AsyncInfo.Run(async token =>
                {
                    var body = new ResponseBody<string>();

                    if (CheckNetworkStatus() == false)
                    {
                        body.StatusCode = HttpStatusCode.BadRequest;
                        return body;
                    }

                    try
                    {
                        int retryTimes =
                            AppSettings.Instance[AppSettings.GLOBAL_NETWORK_RETRY_TIMES].ToString().StringToInt();
                        int times = 0;
                        while (retryTimes - times > 0)
                        {
                            if (times++ > 0)
                                ("Retry to connect " + uri + " " + times + " times").WriteLog(LogType.Message);

                            var cts = new CancellationTokenSource();
                            cts.CancelAfter((int) AppSettings.Instance[AppSettings.GLOBAL_NETWORK_TIMEOUT]);
                            try
                            {
                                if (CheckNetworkStatus() == false)
                                {
                                    body.StatusCode = HttpStatusCode.BadRequest;
                                    cts.Cancel();
                                }

                                HttpResponseMessage response = await Client.GetAsync(uri, cts.Token);
                                body.StatusCode = response.StatusCode;
                                if (response.StatusCode == HttpStatusCode.OK)
                                    body.Body = await response.Content.ReadAsStringAsync();
                                break;
                            }
                            catch (TaskCanceledException cancelEx)
                            {
                                body.StatusCode = HttpStatusCode.GatewayTimeout;
                                cancelEx.WriteLog();
                            }
                            catch (Exception ex)
                            {
                                body.StatusCode = HttpStatusCode.NotFound;
                                ex.WriteLog();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        body.StatusCode = HttpStatusCode.NotFound;
                        ex.WriteLog();
                    }
                    return body;
                });
        }
        
        /// <summary>
        /// HTTP Post response
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public IAsyncOperation<ResponseBody<string>> PostResponseStringAsync(Uri uri, string bodyContent)
        {
            return AsyncInfo.Run(async token =>
            {
                var body = new ResponseBody<string>();

                if (CheckNetworkStatus() == false)
                {
                    body.StatusCode = HttpStatusCode.BadRequest;
                    return body;
                }

                try
                {
                    int retryTimes =
                        AppSettings.Instance[AppSettings.GLOBAL_NETWORK_RETRY_TIMES].ToString().StringToInt();
                    int times = 0;
                    while (retryTimes - times > 0)
                    {
                        if (times++ > 0)
                            ("Retry to connect " + uri + " " + times + " times").WriteLog(LogType.Message);

                        var cts = new CancellationTokenSource();
                        cts.CancelAfter((int)AppSettings.Instance[AppSettings.GLOBAL_NETWORK_TIMEOUT]);
                        try
                        {
                            if (CheckNetworkStatus() == false)
                            {
                                body.StatusCode = HttpStatusCode.BadRequest;
                                cts.Cancel();
                            }

                            HttpResponseMessage response = await Client.PostAsync(uri, new StringContent(bodyContent), cts.Token);
                            body.StatusCode = response.StatusCode;
                            if (response.StatusCode == HttpStatusCode.OK)
                                body.Body = await response.Content.ReadAsStringAsync();
                            break;
                        }
                        catch (TaskCanceledException cancelEx)
                        {
                            body.StatusCode = HttpStatusCode.GatewayTimeout;
                            cancelEx.WriteLog();
                        }
                        catch (Exception ex)
                        {
                            body.StatusCode = HttpStatusCode.NotFound;
                            ex.WriteLog();
                        }
                    }
                }
                catch (Exception ex)
                {
                    body.StatusCode = HttpStatusCode.NotFound;
                    ex.WriteLog();
                }
                return body;
            });
        }
        private bool CheckNetworkStatus()
        {
            if ((bool) AppSettings.Instance.Settings[AppSettings.GLOBAL_INTERNET_STATUS] == false)
            {
                return false;
            }
            return true;
        }


        internal class PlugInHandler : MessageProcessingHandler
        {
            public PlugInHandler(HttpMessageHandler innerHandler)
                : base(innerHandler)
            {
                CustomHeaderParameters = new Dictionary<string, string>();
            }

            public IDictionary<string, string> CustomHeaderParameters { get; set; }

            // Process the request before sending it
            protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request,
                                                                 CancellationToken cancellationToken)
            {
                if (request.Method == HttpMethod.Get)
                {
                    if (CustomHeaderParameters != null)
                        foreach (var parameter in CustomHeaderParameters)
                            request.Headers.Add(parameter.Key, parameter.Value);
                }
                return request;
            }

            // Process the response before returning it to the user
            protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response,
                                                                   CancellationToken cancellationToken)
            {
                if (response.RequestMessage.Method == HttpMethod.Get)
                {
                    if (CustomHeaderParameters != null)
                        foreach (var parameter in CustomHeaderParameters)
                            response.Headers.Add(parameter.Key, parameter.Value);
                }
                return response;
            }
        }

        public class ResponseBody<T>
        {
            public HttpStatusCode StatusCode { get; set; }
            public T Body { get; set; }
        }
    }
}