using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.HttpHelper
{
    public interface IHttpClientHelper : IDisposable
    {
        string GetHttp(string url);
        string GetHttp(string url, bool changeAgent = false, string[] agents = null);
        string GetHttpAsync(string url);
        string GetHttpAsync(string url, bool changeAgent = false, string[] agents = null);
    }
}
