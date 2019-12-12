using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TerexCrawler.Models.DTO.Comment;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Models.DTO.Page;
using TerexCrawler.Models.DTO.XmlSitemap;

namespace TerexCrawler.Models.Interfaces
{
    public interface IWebsiteCrawler : IDisposable
    {
        string WebsiteName { get; }
        string WebsiteUrl { get; }
        Task<string> GetPage(string url);
        Task<T> GetProduct<T>(string url);
        List<CommentDTO> GetComments(string url, int count);
        bool AddPages();
        string[] GetSitemapFromAddress(string url);
        string[] GetSitemapFromFile(string path);
        string[] GetSitemapFromFiles(string[] path);
        void AddBasePage(B5_Url dto);
        void AddBasePages(List<B5_Url> dtos);
        void AddProduct<T>(T dto);
        void AddProducts<T>(T dto);
        Task<T> GetAllBasePage<T>();
        void CrawledProduct(string id);
        void CrawledProducts(string[] ids);
        Task<T> GetFirstProductByCategory<T>(GetFirstProductByCategoryParam param);
        bool AddReviewToDB(AddReviewToDBParam param);
        Task<T> GetAllReviews<T>();
    }
}
