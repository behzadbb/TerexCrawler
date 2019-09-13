using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.Interfaces
{
    public interface IWebsiteCrawler : IDisposable
    {
        string WebsiteName { get; }
        string WebsiteUrl { get; }
        string GetPage(string url);
        string[] GetComments(string url);
        bool AddPages();
        string[] GetSitemapFromAddress(string url);
        string[] GetSitemapFromFile(string path);
        string[] GetSitemapFromFiles(string[] path);
    }
}
