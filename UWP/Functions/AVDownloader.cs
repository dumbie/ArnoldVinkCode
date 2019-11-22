using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace ArnoldVinkCode
{
    class AVDownloader
    {
        internal static async Task<string> DownloadStringAsyncCookie(Int32 TimeOut, string UserAgent, string[][] RequestHeader, Uri DownloadUrl, string cookieString)
        {
            try
            {
                //Set the cookies to container
                System.Net.CookieContainer cookieContainer = new System.Net.CookieContainer();
                foreach (string cookieSingle in cookieString.Split(','))
                {
                    Match cookieMatch = Regex.Match(cookieSingle, "(.+?)=(.+?);");
                    if (cookieMatch.Captures.Count > 0)
                    {
                        string CookieName = cookieMatch.Groups[1].ToString().Replace(" ", String.Empty);
                        string CookieValue = cookieMatch.Groups[2].ToString().Replace(" ", String.Empty);
                        cookieContainer.Add(DownloadUrl, new System.Net.Cookie(CookieName, CookieValue));
                    }
                }

                using (System.Net.Http.HttpClientHandler httpHandler = new System.Net.Http.HttpClientHandler())
                {
                    httpHandler.CookieContainer = cookieContainer;
                    using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient(httpHandler))
                    {
                        httpClient.Timeout = TimeSpan.FromMilliseconds(TimeOut);
                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                        if (RequestHeader != null)
                        {
                            foreach (String[] StringArray in RequestHeader) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                        }

                        string ConnectTask = await httpClient.GetStringAsync(DownloadUrl);
                        Debug.WriteLine("DownloadStringAsyncCookie succeeded for url: " + DownloadUrl + " / " + ConnectTask.Length + "bytes");
                        return ConnectTask;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadStringAsyncCookie exception for url: " + DownloadUrl + " / " + ex.Message);
                return String.Empty;
            }
        }

        //Download string async with timeout
        internal static async Task<string> DownloadStringAsync(Int32 TimeOut, string UserAgent, string[][] RequestHeader, Uri DownloadUrl)
        {
            try
            {
                using (HttpBaseProtocolFilter ProtocolFilter = new HttpBaseProtocolFilter())
                {
                    ProtocolFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    using (HttpClient httpClient = new HttpClient(ProtocolFilter))
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                        if (RequestHeader != null)
                        {
                            foreach (String[] StringArray in RequestHeader) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                        }

                        using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                        {
                            CancelToken.CancelAfter(TimeOut);
                            IAsyncOperationWithProgress<string, HttpProgress> HttpConnectAsync = httpClient.GetStringAsync(DownloadUrl);
                            string ConnectTask = await HttpConnectAsync.AsTask(CancelToken.Token);

                            Debug.WriteLine("DownloadStringAsync succeeded for url: " + DownloadUrl + " / " + ConnectTask.Length + "bytes");
                            return ConnectTask;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadStringAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return String.Empty;
            }
        }

        //Download buffer async with timeout
        internal async static Task<IBuffer> DownloadBufferAsync(Int32 TimeOut, string UserAgent, Uri DownloadUrl)
        {
            try
            {
                using (HttpBaseProtocolFilter ProtocolFilter = new HttpBaseProtocolFilter())
                {
                    ProtocolFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    using (HttpClient httpClient = new HttpClient(ProtocolFilter))
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                        using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                        {
                            CancelToken.CancelAfter(TimeOut);
                            IAsyncOperationWithProgress<IBuffer, HttpProgress> HttpConnectAsync = httpClient.GetBufferAsync(DownloadUrl);
                            IBuffer ConnectTask = await HttpConnectAsync.AsTask(CancelToken.Token);

                            //Debug.WriteLine("DownloadBufferAsync succeeded for url: " + DownloadUrl + " / " + ConnectTask.Length + "bytes");
                            return ConnectTask;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadBufferAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return null;
            }
        }

        //Download input stream async with timeout
        internal async static Task<IInputStream> DownloadInputStreamAsync(Int32 TimeOut, string UserAgent, Uri DownloadUrl)
        {
            try
            {
                using (HttpBaseProtocolFilter ProtocolFilter = new HttpBaseProtocolFilter())
                {
                    ProtocolFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    using (HttpClient httpClient = new HttpClient(ProtocolFilter))
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                        using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                        {
                            CancelToken.CancelAfter(TimeOut);
                            IAsyncOperationWithProgress<IInputStream, HttpProgress> HttpConnectAsync = httpClient.GetInputStreamAsync(DownloadUrl);
                            IInputStream ConnectTask = await HttpConnectAsync.AsTask(CancelToken.Token);

                            Debug.WriteLine("DownloadInputStreamAsync succeeded for url: " + DownloadUrl);
                            return ConnectTask;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadInputStreamAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return null;
            }
        }

        //Send options request with timeout
        internal async static Task<HttpResponseMessage> SendOptionsRequestAsync(Int32 TimeOut, string UserAgent, Uri OptionsUrl)
        {
            try
            {
                using (HttpBaseProtocolFilter ProtocolFilter = new HttpBaseProtocolFilter())
                {
                    ProtocolFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    using (HttpClient httpClient = new HttpClient(ProtocolFilter))
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                        using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                        {
                            CancelToken.CancelAfter(TimeOut);

                            HttpRequestMessage RequestMsg = new HttpRequestMessage(HttpMethod.Options, OptionsUrl);
                            IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> HttpConnectAsync = httpClient.SendRequestAsync(RequestMsg);
                            HttpResponseMessage ConnectTask = await HttpConnectAsync.AsTask(CancelToken.Token);

                            Debug.WriteLine("SendOptionsRequestAsync succeeded for url: " + OptionsUrl);
                            return ConnectTask;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SendOptionsRequestAsync exception for url: " + OptionsUrl + " HRESULT 0x" + e.HResult.ToString("x"));
                return null;
            }
        }

        //Send head request with timeout
        internal async static Task<HttpResponseMessage> SendHeadRequestAsync(Int32 TimeOut, string UserAgent, Uri RequestUrl)
        {
            try
            {
                using (HttpBaseProtocolFilter ProtocolFilter = new HttpBaseProtocolFilter())
                {
                    ProtocolFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    using (HttpClient httpClient = new HttpClient(ProtocolFilter))
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                        using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                        {
                            CancelToken.CancelAfter(TimeOut);

                            HttpRequestMessage RequestMsg = new HttpRequestMessage(HttpMethod.Head, RequestUrl);
                            IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> HttpConnectAsync = httpClient.SendRequestAsync(RequestMsg);
                            HttpResponseMessage ConnectTask = await HttpConnectAsync.AsTask(CancelToken.Token);

                            Debug.WriteLine("SendHeadRequestAsync succeeded for url: " + RequestUrl);
                            return ConnectTask;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SendHeadRequestAsync exception for url: " + RequestUrl + " HRESULT 0x" + e.HResult.ToString("x"));
                return null;
            }
        }

        //Send post request with timeout
        internal static async Task<HttpResponseMessage> SendPostRequestAsync(Int32 TimeOut, string UserAgent, string[][] RequestHeader, Uri PostUrl, IHttpContent PostContent)
        {
            try
            {
                using (HttpBaseProtocolFilter ProtocolFilter = new HttpBaseProtocolFilter())
                {
                    ProtocolFilter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
                    using (HttpClient httpClient = new HttpClient(ProtocolFilter))
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                        httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                        if (RequestHeader != null)
                        {
                            foreach (String[] StringArray in RequestHeader) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                        }

                        using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                        {
                            CancelToken.CancelAfter(TimeOut);

                            IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> HttpConnectAsync = httpClient.PostAsync(PostUrl, PostContent);
                            HttpResponseMessage ConnectTask = await HttpConnectAsync.AsTask(CancelToken.Token);

                            Debug.WriteLine("SendPostRequestAsync succeeded for url: " + PostUrl);
                            return ConnectTask;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SendPostRequestAsync exception for url: " + PostUrl + " HRESULT 0x" + e.HResult.ToString("x"));
                return null;
            }
        }
    }
}