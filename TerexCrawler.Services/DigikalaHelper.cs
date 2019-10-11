using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerexCrawler.Common;
using TerexCrawler.DataLayer.Repository;
using TerexCrawler.HttpHelper;
using TerexCrawler.Models;
using TerexCrawler.Models.DTO.Page;
using TerexCrawler.Models.DTO.XmlSitemap;
using TerexCrawler.Models.Enums;
using TerexCrawler.Models.Interfaces;

namespace TerexCrawler.Services.Digikala
{
    public class DigikalaHelper : IWebsiteCrawler
    {
        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            client.Dispose();
            Dispose(true);
        }
        #endregion
        private const string sitename = "Digikala";
        public string WebsiteName => sitename;
        public string WebsiteUrl => "https://Digikala.com";
        IHttpClientHelper client = new RestSharpHelper();
        ILoger Logger = new LoggerHelper();

        public string GetComment(string url)
        {
            throw new NotImplementedException();
        }

        public string GetPage(string url)
        {
            var res = client.GetHttp(url);
            if (res.Success)
            {
                return res.Content;
            }
            else
            {
                LogDTO log = new LogDTO()
                {
                    DateTime = DateTime.Now,
                    Description = res.ExeptionErrorMessage,
                    ProjectId = (int)ProjectNames.HttpHelper,
                    Url = url,
                    MethodName = "Digikala - GetPage",
                    Title = "Get HTML Error"
                };
                Logger.AddLog(log);
                return string.Empty;
            }
        }

        public bool AddPages()
        {
            throw new NotImplementedException();
        }

        public string[] GetComments(string url)
        {
            throw new NotImplementedException();
        }

        public string[] GetSitemapFromAddress(string url)
        {
            throw new NotImplementedException();
        }

        public string[] GetSitemapFromFile(string path)
        {
            throw new NotImplementedException();
        }

        public string[] GetSitemapFromFiles(string[] path)
        {
            throw new NotImplementedException();
        }

        public void AddBasePage(B5_Url dto)
        {
            try
            {
                using (var digi = new DigikalaRepository())
                {
                    digi.AddDigikalaBasePage(new DigikalaPageBaseDTO(dto));
                }
            }
            catch (Exception ex)
            {
                LogDTO log = new LogDTO()
                {
                    DateTime = DateTime.Now,
                    Description = "Error Convert Model, Error= " + ex.ToString(),
                    Title = "Convert To Standard DTO, Digikala",
                    MethodName = "AddBasePage",
                    ProjectId = 1,
                    Url = dto.loc
                };
                Logger.AddLog(log);
            }
        }
        public T GetProduct<T>(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            var main = body.SelectSingleNode("//main");

            var divContainer = main.SelectSingleNode("//div[@id='content']//div[@class='o-page c-product-page']//div[@class='container']");
            var _cats = divContainer.SelectNodes("//div[@class='c-product__nav-container']//nav//ul//li[@property='itemListElement']");
            List<string> Cats = _cats.Any() && _cats.Count() > 0 ? _cats.Select(x => x.InnerText).ToList() : new List<string>();
            ////
            var title_fa = divContainer.SelectNodes("//div[@class='c-product__nav-container']//nav//ul//li//span[@property='name']").Last().InnerText;

            var article_info = divContainer.SelectSingleNode("//article//section[@class='c-product__info']");
            var title_fa_1 = article_info.SelectSingleNode("//div[@class='c-product__headline']//h1[@class='c-product__title']").InnerText.Replace("\n", "").Trim();

            var productWrapper = article_info.SelectSingleNode("//div[@class='c-product__attributes js-product-attributes']//div[@class='c-product__config']//div[@class='c-product__config-wrapper']");
            var brand = productWrapper.SelectSingleNode("//div[@class='c-product__directory']//ul//li//a[@class='btn-link-spoiler product-brand-title']").InnerText;
            var cat = productWrapper.SelectSingleNode("//div[@class='c-product__directory']//ul//li//a[@class='btn-link-spoiler']").InnerText;
            var colors = productWrapper.SelectNodes("//div[@class='c-product__variants']//ul//li").Select(x => x.InnerText).ToList();
            var feature_list = productWrapper.SelectNodes("//div[@class='c-product__params js-is-expandable']//ul//li").Select(x => new { name = x.FirstChild.InnerText.Replace(":","").Trim(), val = x.LastChild.InnerText.Replace("\n","").Trim() }).ToList();
            return (T)Convert.ChangeType(new DigikalaPageBaseDTO(), typeof(T));
        }
        public void AddBasePages(List<B5_Url> dtos)
        {
            var pageBases = new List<DigikalaPageBaseDTO>();
            foreach (var dto in dtos)
            {
                try
                {
                    pageBases.Add(new DigikalaPageBaseDTO(dto));
                }
                catch (Exception ex)
                {
                    LogDTO log = new LogDTO()
                    {
                        DateTime = DateTime.Now,
                        Description = "Error Convert Model, Error= " + ex.ToString(),
                        Title = "Convert To Standard DTO, Digikala",
                        MethodName = "AddBasePages",
                        ProjectId = 1,
                        Url = dto.loc
                    };
                    Logger.AddLog(log);
                }
            }
            dtos.Clear();
            using (var digi = new DigikalaRepository())
            {
                digi.AddDigikalaBasePages(pageBases);
            }
            pageBases.Clear();
        }


    }
}