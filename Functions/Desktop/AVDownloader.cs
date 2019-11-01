using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    public partial class AVDownloader
    {
        //Download string async with timeout
        public static async Task<string> DownloadStringAsync(int TimeOut, string UserAgent, string[][] Header, Uri DownloadUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                    if (Header != null)
                    {
                        foreach (string[] StringArray in Header) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                    }

                    using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                    {
                        CancelToken.CancelAfter(TimeOut);

                        HttpResponseMessage HttpConnectAsync = await httpClient.GetAsync(DownloadUrl, CancelToken.Token);
                        string ConnectTask = HttpConnectAsync.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine("DownloadStringAsync succeeded for url: " + DownloadUrl + " / " + ConnectTask.Length + "bytes");
                        return ConnectTask;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadStringAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return string.Empty;
            }
        }

        //Download byte async with timeout
        internal async static Task<byte[]> DownloadByteAsync(int TimeOut, string UserAgent, string[][] Header, Uri DownloadUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                    if (Header != null)
                    {
                        foreach (string[] StringArray in Header) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                    }

                    using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                    {
                        CancelToken.CancelAfter(TimeOut);

                        HttpResponseMessage HttpConnectAsync = await httpClient.GetAsync(DownloadUrl, CancelToken.Token);
                        byte[] ConnectTask = HttpConnectAsync.Content.ReadAsByteArrayAsync().Result;

                        Debug.WriteLine("DownloadByteAsync succeeded for url: " + DownloadUrl + " / " + ConnectTask.Length + "bytes");
                        return ConnectTask;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadByteAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return null;
            }
        }

        //Download input stream async with timeout
        internal async static Task<Stream> DownloadStreamAsync(int TimeOut, string UserAgent, string[][] Header, Uri DownloadUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                    if (Header != null)
                    {
                        foreach (string[] StringArray in Header) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                    }

                    using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                    {
                        CancelToken.CancelAfter(TimeOut);

                        HttpResponseMessage HttpConnectAsync = await httpClient.GetAsync(DownloadUrl, CancelToken.Token);
                        Stream ConnectTask = HttpConnectAsync.Content.ReadAsStreamAsync().Result;

                        Debug.WriteLine("DownloadStreamAsync succeeded for url: " + DownloadUrl);
                        return ConnectTask;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadStreamAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return null;
            }
        }

        //Send head request with timeout
        internal async static Task<HttpResponseMessage> SendHeadRequestAsync(int TimeOut, string UserAgent, string[][] Header, Uri RequestUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                    if (Header != null)
                    {
                        foreach (string[] StringArray in Header) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                    }

                    using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                    {
                        CancelToken.CancelAfter(TimeOut);

                        HttpRequestMessage RequestMsg = new HttpRequestMessage(HttpMethod.Head, RequestUrl);
                        HttpResponseMessage ConnectTask = await httpClient.SendAsync(RequestMsg, CancelToken.Token);

                        Debug.WriteLine("SendHeadRequestAsync succeeded for url: " + RequestUrl);
                        return ConnectTask;
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
        public static async Task<string> SendPostRequestAsync(int TimeOut, string UserAgent, string[][] Header, Uri PostUrl, HttpContent PostContent)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                    if (Header != null)
                    {
                        foreach (string[] StringArray in Header) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                    }

                    using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                    {
                        CancelToken.CancelAfter(TimeOut);

                        HttpResponseMessage HttpConnectAsync = await httpClient.PostAsync(PostUrl, PostContent, CancelToken.Token);
                        string ConnectTask = HttpConnectAsync.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine("SendPostRequestAsync succeeded for url: " + PostUrl + " / " + ConnectTask.Length + "bytes");
                        return ConnectTask;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SendPostRequestAsync exception for url: " + PostUrl + " HRESULT 0x" + e.HResult.ToString("x"));
                return null;
            }
        }

        //Send delete request with timeout
        public static async Task<string> SendDeleteRequestAsync(int TimeOut, string UserAgent, string[][] Header, Uri PostUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");
                    if (Header != null)
                    {
                        foreach (string[] StringArray in Header) { httpClient.DefaultRequestHeaders.Add(StringArray[0], StringArray[1]); }
                    }

                    using (CancellationTokenSource CancelToken = new CancellationTokenSource())
                    {
                        CancelToken.CancelAfter(TimeOut);

                        HttpResponseMessage HttpConnectAsync = await httpClient.DeleteAsync(PostUrl, CancelToken.Token);
                        string ConnectTask = HttpConnectAsync.Content.ReadAsStringAsync().Result;

                        Debug.WriteLine("SendDeleteRequestAsync succeeded for url: " + PostUrl + " / " + ConnectTask.Length + "bytes");
                        return ConnectTask;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SendDeleteRequestAsync exception for url: " + PostUrl + " HRESULT 0x" + e.HResult.ToString("x"));
                return null;
            }
        }
    }
}