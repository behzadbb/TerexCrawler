﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TerexCrawler.Common;
using TerexCrawler.DataLayer.Repository;
using TerexCrawler.HttpHelper;
using TerexCrawler.Models;
using TerexCrawler.Models.DTO.Comment;
using TerexCrawler.Models.DTO.Digikala;
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
            System.Threading.Thread.Sleep(250);
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
            List<CommentDTO> CommentsList = new List<CommentDTO>();
            int? cmPageCount = (int?)null;
            int DKP = getDKPWithUrl(url);
            string firstCmUrl = GetReviewUrl(DKP);
            var firstCmPage = GetPage(firstCmUrl);

            var doc = new HtmlDocument();
            doc.LoadHtml(firstCmPage);

            var rate = doc.DocumentNode.SelectNodes("//h2[@class='c-comments__headline']//span//span");
            using (HtmlHelper html = new HtmlHelper())
            {
                List<CommentDTO> comments = new List<CommentDTO>();
                int rate2 = int.Parse(html.NumberEN(rate[2].InnerText.Trim()));
                int rate3 = int.Parse(html.NumberEN(rate[3].InnerText.Replace("/", "").Trim()));
                int rate4 = int.Parse(html.NumberEN(rate[4].InnerText.Replace("(", "").Replace(")", "").Replace("\n", "").Replace("نفر", "").Trim()));

                if (doc.GetElementbyId("comment-pagination").SelectNodes("//ul[@class='c-pager__items']//li[@class='js-pagination-item']") != null)
                {
                    cmPageCount = Int16.Parse(doc.GetElementbyId("comment-pagination").SelectNodes("//ul[@class='c-pager__items']//li[@class='js-pagination-item']").LastOrDefault().SelectNodes("//a[@class='c-pager__next']").Select(x => x.Attributes["data-page"].Value.ToString()).FirstOrDefault());
                }

                if (cmPageCount.HasValue)
                {
                    for (int i = 1; i <= cmPageCount; i++)
                    {
                        var _cmPage = GetPage(GetReviewListUrl(DKP, i));
                        var _cmDoc = new HtmlDocument();
                        _cmDoc.LoadHtml(_cmPage);

                        var sections = _cmDoc.DocumentNode.SelectSingleNode("//ul[@class='c-comments__list']").SelectNodes("//li//section").ToList();
                        foreach (var section in sections)
                        {
                            var _section = new HtmlDocument();
                            _section.LoadHtml(section.InnerHtml);
                            CommentDTO cm = new CommentDTO();
                            cm.Purchased = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//div[@class='c-message-light c-message-light--purchased']") != null;
                            var isDeatils = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//ul[@class='c-comments__user-shopping']") != null;
                            var Positive = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//div[@class='c-message-light c-message-light--opinion-positive']") != null;
                            var Negative = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//div[@class='c-message-light c-message-light--opinion-negative']") != null;
                            var Noidea = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//div[@class='c-message-light c-message-light--opinion-noidea']") != null;
                            if (isDeatils)
                            {
                                var _colorCell = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//ul[@class='c-comments__user-shopping']//li//div[@class='cell color-cell']").InnerText;
                                cm.Color = string.IsNullOrEmpty(_colorCell) ? string.Empty : _colorCell.Replace("\n", "").Trim();
                                var _sellerCell = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//ul[@class='c-comments__user-shopping']//li//div[@class='cell seller-cell']").InnerText;
                                cm.Seller = string.IsNullOrEmpty(_sellerCell) ? string.Empty : _sellerCell.Replace("\n", "").Trim();
                                var _boughtPrice = _section.DocumentNode.SelectSingleNode("//div[@class='aside']//ul[@class='c-comments__user-shopping']//li//div[@class='cell bought-price']").InnerText;
                                long? boughtPrice = string.IsNullOrEmpty(_boughtPrice) ? (long?)null : long.Parse(html.NumberEN(_boughtPrice.Replace("\n", "").Replace(",", "").Replace("تومان", "").Trim()));
                                if (boughtPrice.HasValue)
                                {
                                    cm.BoughtPrice = boughtPrice.Value;
                                }
                            }
                            var article = _section.DocumentNode.SelectSingleNode("//div[@class='article']");
                            if (article != null)
                            {
                                var header = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='header']//div");
                                var Title = header != null && header.ChildNodes.Count() > 0 ? header.ChildNodes[0].InnerText.Replace("\n", "").Trim() : "";
                                var authorDeatils = header != null && header.ChildNodes.Count() > 1 ? header.ChildNodes[1].InnerText.Replace("\n", "").Trim() : "";
                                if (!string.IsNullOrEmpty(authorDeatils))
                                {
                                    string[] _t = authorDeatils.Replace("در تاریخ", "|").Replace("توسط", "").Replace("\n","").Trim().Split('|');
                                    if (_t.Length>1)
                                    {
                                        _t[1] = html.MountToNum(html.NumberEN(_t[1]));
                                        cm.Author = _t[0].Trim();
                                        cm.CommentDate = html.JalaliToMiladi(_t[1]);
                                    }
                                }

                                var evaluation= _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='c-comments__evaluation']");
                                if (evaluation != null)
                                {
                                    var evaluationPositive = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-positive']");
                                    if (evaluationPositive != null)
                                    {
                                        cm.PositiveAspect = _section.DocumentNode.SelectNodes("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-positive']//ul//li").Select(x=> x.InnerText.Replace("\n","").Trim()).ToArray();
                                    }

                                    var evaluationNegative = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-negative']");
                                    if (evaluationNegative != null)
                                    {
                                        cm.NegativeAspect = _section.DocumentNode.SelectNodes("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-negative']//ul//li").Select(x=> x.InnerText.Replace("\n","").Trim()).ToArray();
                                    }

                                }
                            }


                            comments.Add(cm);
                        }
                    }
                }
                var s = JsonConvert.SerializeObject(comments);
            }
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
        public T GetProduct<T>(string content, string url)
        {
            DigikalaProductDTO dto = new DigikalaProductDTO();
            dto.Url = url;
            dto.DKP = getDKPWithUrl(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var body = doc.DocumentNode.SelectSingleNode("//body");
            var main = body.SelectSingleNode("//main");

            #region Container
            var divContainer = main.SelectSingleNode("//div[@id='content']//div[@class='o-page c-product-page']//div[@class='container']");
            var _cats = divContainer.SelectNodes("//div[@class='c-product__nav-container']//nav//ul//li[@property='itemListElement']");
            dto.Categories = _cats.Any() && _cats.Count() > 0 ? _cats.Select(x => x.InnerText).ToList() : new List<string>();
            ////
            dto.Title = divContainer.SelectNodes("//div[@class='c-product__nav-container']//nav//ul//li//span[@property='name']").Last().InnerText;

            var article_info = divContainer.SelectSingleNode("//article//section[@class='c-product__info']");
            var title_fa_1 = article_info.SelectSingleNode("//div[@class='c-product__headline']//h1[@class='c-product__title']").ChildNodes["#Text"].InnerText.Replace("\n", "").Trim();
            dto.TitleEN = article_info.SelectSingleNode("//div[@class='c-product__headline']//h1[@class='c-product__title']//span[@class='c-product__title-en']").InnerHtml.Replace("\n", "").Trim();
            var product__guaranteed = article_info.SelectSingleNode("//div[@class='c-product__headline']//div[@class='c-product__guaranteed']//span").InnerText.Replace("\n", "").Trim();

            var productWrapper = article_info.SelectSingleNode("//div[@class='c-product__attributes js-product-attributes']//div[@class='c-product__config']//div[@class='c-product__config-wrapper']");
            dto.Brand = productWrapper.SelectSingleNode("//div[@class='c-product__directory']//ul//li" +
                "//a[@class='btn-link-spoiler product-brand-title']").InnerText;
            dto.Category = productWrapper.SelectSingleNode("//div[@class='c-product__directory']//ul//li//a[@class='btn-link-spoiler']").InnerText;
            List<string> colors = new List<string>();
            bool isColors = productWrapper.SelectNodes("//div[@class='c-product__variants']") != null;
            if (isColors)
            {
                colors.AddRange(productWrapper.SelectNodes("//div[@class='c-product__variants']//ul//li").Select(x => x.InnerText).ToList());
                dto.Colors = colors;
            }

            var feature_list = productWrapper.SelectNodes("//div[@class='c-product__params js-is-expandable']//ul//li").Select(x => new { name = x.FirstChild.InnerText.Replace(":", "").Trim(), val = x.LastChild.InnerText.Replace("\n", "").Trim() }).ToList();

            var c_box = article_info.SelectSingleNode("//div[@class='c-product__attributes js-product-attributes']//div[@class='c-product__summary js-product-summary']//div[@class='c-box']");
            string priceQuery = "//div[@class='c-product__seller-info js-seller-info']" +
                "//div[@class='js-seller-info-changable c-product__seller-box']" +
                "//div[@class='c-product__seller-row c-product__seller-row--price']" +
                "//div[@class='c-product__seller-price-prev js-rrp-price u-hidden']";
            var isExistPrice = article_info.SelectSingleNode(priceQuery) != null;
            if (isExistPrice)
            {
                using (HtmlHelper html = new HtmlHelper())
                {
                    dto.Price = Int64.Parse(html.NumberEN(article_info.SelectSingleNode(priceQuery).InnerText.Replace("\n", "").Replace(",", "").Trim()));
                }
            }

            #endregion

            #region Tabs
            string tabsQuery =
                "//div[@id='tabs']" +
                "//div[@class='c-box c-box--tabs p-tabs__content']" +
                "//div[@class='c-params']" +
                "//article" +
                "//section";
            var tabSections = main.SelectNodes(tabsQuery).ToArray();
            List<ProductFeatures> features = new List<ProductFeatures>();
            foreach (var feat in tabSections)
            {
                ProductFeatures p = new ProductFeatures();
                p.Title = feat.ChildNodes[0].InnerText;
                p.Features = feat.ChildNodes[1].ChildNodes
                    .Select(x => new Feature
                    {
                        Key = x.ChildNodes[0].InnerText.Replace("\n", "").Trim(),
                        Value = x.ChildNodes[1].InnerText.Replace("\n", "")
                        .Replace("          ", " ").Replace("     ", " ").Replace("     ", " ").Replace("  ", " ").Replace("  ", " ").Trim(),
                    }).ToArray();
                features.Add(p);
            }
            dto.Features = features;
            #endregion
            //var jjj = JsonConvert.SerializeObject(dto);
            return (T)Convert.ChangeType(dto, typeof(DigikalaProductDTO));
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

        private int getDKPWithUrl(string url)
        {
            var indexDKP = url.LastIndexOf("/") + 1;
            var lenghtDKP = url.Length - indexDKP;
            return int.Parse(url.Substring(indexDKP, lenghtDKP).Replace("dkp-", ""));
        }
        public string GetReviewUrl(int DKP, int Page = 1)
        {
            return string.Format("https://www.digikala.com/ajax/product/comments/{0}/?page={1}&mode=buyers", DKP, Page);
        }
        public string GetReviewListUrl(int DKP, int Page = 1)
        {
            return string.Format("https://www.digikala.com/ajax/product/comments/list/{0}/?page={1}&mode=buyers", DKP, Page);
        }
    }
}