using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.Interfaces
{
    public interface IWebsiteCrawler : IDisposable
    {
        string WebsiteName { get; set; }
        string WebsiteUrl { get; set; }
        string GetPage(string url);
        string GetComment(string url);
    }
}
