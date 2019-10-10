using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO.XmlSitemap;

namespace TerexCrawler.Models.Interfaces
{
    public interface IWebsiteCrawler : IDisposable
    {
        string WebsiteName { get; }
        string WebsiteUrl { get; }
        string GetPage(string url);
        T GetProduct<T>(string content);
        string[] GetComments(string url);
        bool AddPages();
        string[] GetSitemapFromAddress(string url);
        string[] GetSitemapFromFile(string path);
        string[] GetSitemapFromFiles(string[] path);
        void AddBasePage(B5_Url dto);
        void AddBasePages(List<B5_Url> dtos);
    }
}
