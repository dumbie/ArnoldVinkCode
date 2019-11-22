using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArnoldVinkCode
{
    class AVDownloader
    {
        //Download string async with timeout
        internal static async Task<string> DownloadStringAsync(Int32 TimeOut, string UserAgent, Uri DownloadUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = new TimeSpan(0, 0, TimeOut);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                    string HttpConnectAsync = await httpClient.GetStringAsync(DownloadUrl);

                    Debug.WriteLine("DownloadStringAsync succeeded for url: " + DownloadUrl + " / " + HttpConnectAsync.Length + "bytes");
                    return HttpConnectAsync;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadStringAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return String.Empty;
            }
        }

        //Download byte async with timeout
        internal async static Task<Byte[]> DownloadByteAsync(Int32 TimeOut, string UserAgent, Uri DownloadUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = new TimeSpan(0, 0, TimeOut);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                    Byte[] HttpConnectAsync = await httpClient.GetByteArrayAsync(DownloadUrl);

                    Debug.WriteLine("DownloadByteAsync succeeded for url: " + DownloadUrl + " / " + HttpConnectAsync.Length + "bytes");
                    return HttpConnectAsync;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadByteAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return null;
            }
        }

        //Download input stream async with timeout
        internal async static Task<Stream> DownloadStreamAsync(Int32 TimeOut, string UserAgent, Uri DownloadUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = new TimeSpan(0, 0, TimeOut);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                    Stream HttpConnectAsync = await httpClient.GetStreamAsync(DownloadUrl);

                    Debug.WriteLine("DownloadStreamAsync succeeded for url: " + DownloadUrl);
                    return HttpConnectAsync;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("DownloadStreamAsync exception for url: " + DownloadUrl + " / " + ex.Message);
                return null;
            }
        }

        //Send head request with timeout
        internal async static Task<HttpResponseMessage> SendHeadRequestAsync(Int32 TimeOut, string UserAgent, Uri RequestUrl)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = new TimeSpan(0, 0, TimeOut);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
                    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache, no-store");

                    HttpRequestMessage RequestMsg = new HttpRequestMessage(HttpMethod.Head, RequestUrl);
                    HttpResponseMessage HttpConnectAsync = await httpClient.SendAsync(RequestMsg);

                    Debug.WriteLine("SendHeadRequestAsync succeeded for url: " + RequestUrl);
                    return HttpConnectAsync;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("SendHeadRequestAsync exception for url: " + RequestUrl + " HRESULT 0x" + e.HResult.ToString("x"));
                return null;
            }
        }
    }
}