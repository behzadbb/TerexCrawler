using HtmlAgilityPack;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerexCrawler.Common;
using TerexCrawler.DataLayer.Repository;
using TerexCrawler.Entites.Digikala;
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
            Logger.Dispose();
            Dispose(true);
        }
        #endregion
        #region Element Query
        const string _sellerCellEQ = "//div[@class='aside']//ul[@class='c-comments__user-shopping']//li//div[@class='cell seller-cell']";
        const string _colorCellEQ = "//div[@class='aside']//ul[@class='c-comments__user-shopping']//li//div[@class='cell color-cell']";
        const string _sizeCellEQ = "//div[@class='aside']//ul[@class='c-comments__user-shopping']//li//div[@class='cell']";
        #endregion
        string[] user_agent = {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362"
        };
        private const string sitename = "Digikala";
        public string WebsiteName => sitename;
        public string WebsiteUrl => "https://Digikala.com";
        readonly IHttpClientHelper client = new HttpClientHelper();
        ILoger Logger = new MongoDBLoggerHelper();

        public string GetComment(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPage(string url)
        {
            //System.Threading.Thread.Sleep(50);
            var res = client.GetHttp(url, true, user_agent);
            if (res.Success)
            {
                return res.Content;
            }
            else
            {
                LogDTO log = new LogDTO()
                {
                    _id = ObjectId.GenerateNewId().ToString(),
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
        public async Task<string> GetPage1(string url)
        {
            IHttpClientHelper client1 = new RestSharpHelper();
            var res = client1.GetHttp(url, true, user_agent);
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

        public List<CommentDTO> GetComments(string url, int count)
        {
            List<CommentDTO> CommentsList = new List<CommentDTO>();
            int DKP = getDKPWithUrl(url);
            string _SelectPrice = "//div[@class='aside']//ul[@class='c-comments__user-shopping']//li//div[@class='cell bought-price']";
            using (HtmlHelper html = new HtmlHelper())
            {
                List<CommentDTO> comments = new List<CommentDTO>();

                for (int i = 1; i <= count; i++)
                {
                    string reviewListUrl = GetReviewListUrl(DKP, i);
                    string _cmPage = GetPage(reviewListUrl).Result;
                    if (string.IsNullOrEmpty(_cmPage))
                    {
                        System.Threading.Thread.Sleep(1500);
                        _cmPage = GetPage1(url).Result;
                    }
                    if (string.IsNullOrEmpty(_cmPage))
                    {
                        System.Threading.Thread.Sleep(4000);
                        _cmPage = GetPage(url).Result;
                    }
                    if (string.IsNullOrEmpty(_cmPage))
                    {
                        throw new ArgumentException($"DKP: {DKP} , index: {i} , URL: {reviewListUrl} , Content = Null", "Comment");
                    }

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
                        if (Positive)
                            cm.OpinionType = 100;
                        else if (Noidea)
                            cm.OpinionType = 50;
                        else if (Negative)
                            cm.OpinionType = -100;
                        else
                            cm.OpinionType = 0;

                        if (isDeatils)
                        {
                            var _colorCell = _section.DocumentNode.SelectSingleNode(_colorCellEQ) != null ? _section.DocumentNode.SelectSingleNode(_colorCellEQ).InnerText : "";
                            cm.Color = string.IsNullOrEmpty(_colorCell) ? string.Empty : _colorCell.Replace("\n", "").Trim();
                            var _sellerCell = _section.DocumentNode.SelectSingleNode(_sellerCellEQ).InnerText;
                            cm.Seller = string.IsNullOrEmpty(_sellerCell) ? string.Empty : _sellerCell.Replace("\n", "").Trim();
                            var _boughtPrice = _section.DocumentNode.SelectSingleNode(_SelectPrice);
                            //var _boughtPrice = _section.DocumentNode.SelectSingleNode(_SelectPrice) != null ? string.Empty : _section.DocumentNode.SelectSingleNode(_SelectPrice).InnerText;
                            long? boughtPrice = _boughtPrice == null ? (long?)null : long.Parse(html.NumberEN(_boughtPrice.InnerText.Replace("\n", "").Replace(",", "").Replace("تومان", "").Trim()));
                            if (boughtPrice.HasValue)
                            {
                                cm.BoughtPrice = boughtPrice.Value;
                            }
                            var _sizeCell = _section.DocumentNode.SelectSingleNode(_sizeCellEQ);
                            if (_sizeCell != null && _sizeCell.InnerText.Contains("سایز خریداری شده"))
                            {
                                cm.Size = _section.DocumentNode.SelectNodes(_sizeCellEQ)[1].InnerText.Replace("\n", "").Trim();
                            }
                        }
                        var article = _section.DocumentNode.SelectSingleNode("//div[@class='article']");
                        if (article != null)
                        {
                            var header = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='header']//div");
                            cm.Title = header != null && header.ChildNodes.Count() > 0 ? header.ChildNodes[0].InnerText.Replace("\n", "").Trim() : "";
                            var authorDeatils = header != null && header.ChildNodes.Count() > 1 ? header.ChildNodes[1].InnerText.Replace("\n", "").Trim() : "";
                            if (!string.IsNullOrEmpty(authorDeatils))
                            {
                                string[] _t = authorDeatils.Replace("در تاریخ", "|").Replace("توسط", "").Replace("\n", "").Trim().Split('|');
                                if (_t.Length > 1)
                                {
                                    _t[1] = html.MountToNum(html.NumberEN(_t[1]));
                                    cm.Author = _t[0].Trim();
                                    cm.CommentDate = html.JalaliToMiladi(_t[1]);
                                }
                            }

                            var evaluation = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='c-comments__evaluation']");
                            if (evaluation != null)
                            {
                                var evaluationPositive = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-positive']");
                                if (evaluationPositive != null)
                                {
                                    cm.PositiveAspect = _section.DocumentNode.SelectNodes("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-positive']//ul//li")
                                        .Select(x => x.InnerText.Replace("\n", "").Replace("\r", "").Replace("  ", " ").Replace("  ", " ").Trim()).ToArray();
                                }

                                var evaluationNegative = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-negative']");
                                if (evaluationNegative != null)
                                {
                                    cm.NegativeAspect = _section.DocumentNode.SelectNodes("//div[@class='article']//div[@class='c-comments__evaluation']//div[@class='c-comments__evaluation-negative']//ul//li").Select(x => x.InnerText.Replace("\n", "").Trim()).ToArray();
                                }

                            }

                            var paragraph = _section.DocumentNode.SelectSingleNode("//div[@class='article']//p");
                            if (paragraph != null)
                            {
                                cm.Review = paragraph.InnerText.Trim();
                            }

                            var footer = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='footer']//div[@class='c-comments__likes js-comment-like-container']");
                            if (footer != null)
                            {
                                var commentLike = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='footer']//div[@class='c-comments__likes js-comment-like-container']//button[@class='btn-like js-comment-like']").Attributes["data-counter"].Value;
                                cm.CommentLike = string.IsNullOrEmpty(commentLike) ? (short?)null : short.Parse(html.NumberEN(commentLike));
                                var commentDisLike = _section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='footer']//div[@class='c-comments__likes js-comment-like-container']//button[@class='btn-like js-comment-dislike']").Attributes["data-counter"].Value;
                                cm.CommentDisLike = string.IsNullOrEmpty(commentLike) ? (short?)null : short.Parse(html.NumberEN(commentDisLike));
                                if (cm.CommentLike.HasValue)
                                {
                                    cm.CommentId = long.Parse(_section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='footer']//div[@class='c-comments__likes js-comment-like-container']//button[@class='btn-like js-comment-like']").Attributes["data-comment"].Value.Trim());
                                }
                                if (!cm.CommentId.HasValue && cm.CommentDisLike.HasValue)
                                {
                                    cm.CommentId = long.Parse(_section.DocumentNode.SelectSingleNode("//div[@class='article']//div[@class='footer']//div[@class='c-comments__likes js-comment-like-container']//button[@class='btn-like js-comment-like']").Attributes["data-comment"].Value.Trim());
                                }
                            }
                        }
                        cm.CreateDate = DateTime.UtcNow;
                        comments.Add(cm);
                    }
                    if (i % 5 == 0)
                    {
                        System.Threading.Thread.Sleep(30);
                    }
                }
                return comments;
            }
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

        public async Task<T> GetProduct<T>(string url)
        {
            try
            {
                string content = string.Empty;
                var res = client.GetHttp(url, true, user_agent);
                if (res.Success)
                {
                    content = res.Content;
                }
                if (res.HttpStatusCode == (int)HttpStatusCode.NotFound)
                {
                    using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
                    {
                        db.RemoveBasePage(url);
                    }
                }
                DigikalaProductDTO dto = new DigikalaProductDTO();

                if (string.IsNullOrEmpty(content))
                {
                    System.Threading.Thread.Sleep(200);
                    content = await GetPage1(url);
                }
                if (string.IsNullOrEmpty(content))
                {
                    System.Threading.Thread.Sleep(400);
                    content = await GetPage(url);
                }

                if (string.IsNullOrEmpty(content))
                    return (T)Convert.ChangeType(null, typeof(DigikalaProductDTO));

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
                var enTitle = article_info.SelectSingleNode("//div[@class='c-product__headline']//h1[@class='c-product__title']//span[@class='c-product__title-en']");
                if (enTitle != null)
                {
                    dto.TitleEN = enTitle.InnerHtml.Replace("\n", "").Trim();
                }

                var product__guaranteed = article_info.SelectSingleNode("//div[@class='c-product__headline']//div[@class='c-product__guaranteed']//span");
                if (product__guaranteed != null)
                {
                    using (HtmlHelper html = new HtmlHelper())
                    {
                        dto.Guaranteed = short.Parse(html.NumberEN(product__guaranteed.InnerText.Replace("\n", "").Replace("  ", "").Trim().Replace("بیش از", "").Replace("نفر از خریداران این محصول را پیشنهاد داده‌اند", "").Trim()));
                    }

                }

                var productWrapper = article_info.SelectSingleNode("//div[@class='c-product__attributes js-product-attributes']//div[@class='c-product__config']//div[@class='c-product__config-wrapper']");
                string brandElementQuery1 = "//div[@class='c-product__directory']//ul//li//a[@class='btn-link-spoiler product-brand-title']";
                string brandElementQuery2 = "//div[@class='c-product__directory']//ul//li//span[@class='product-brand-title']";
                dto.Brand = productWrapper.SelectSingleNode(brandElementQuery1) != null ? productWrapper.SelectSingleNode(brandElementQuery1).InnerText : (productWrapper.SelectSingleNode(brandElementQuery2) != null ? productWrapper.SelectSingleNode(brandElementQuery2).InnerText : "");
                dto.Category = productWrapper.SelectSingleNode("//div[@class='c-product__directory']//ul//li//a[@class='btn-link-spoiler']").InnerText;
                List<string> colors = new List<string>();
                bool isColors = productWrapper.SelectNodes("//div[@class='c-product__variants']") != null;
                if (isColors)
                {
                    colors.AddRange(productWrapper.SelectNodes("//div[@class='c-product__variants']//ul//li").Select(x => x.InnerText).ToList());
                    dto.Colors = colors;
                }

                if (productWrapper.SelectNodes("//div[@class='c-product__params js-is-expandable']//ul//li") != null)
                {
                    var feature_list = productWrapper.SelectNodes("//div[@class='c-product__params js-is-expandable']//ul//li").Select(x => new { name = x.FirstChild.InnerText.Replace(":", "").Trim(), val = x.LastChild.InnerText.Replace("\n", "").Trim() }).ToList();
                }

                var c_box = article_info.SelectSingleNode("//div[@class='c-product__attributes js-product-attributes']//div[@class='c-product__summary js-product-summary']//div[@class='c-box']");
                string priceOffQuery = "//div[@class='c-product__seller-info js-seller-info']" +
                    "//div[@class='js-seller-info-changable c-product__seller-box']" +
                    "//div[@class='c-product__seller-row c-product__seller-row--price']" +
                    "//div[@class='c-product__seller-price-real']";
                string priceQuery = "//div[@class='c-product__seller-info js-seller-info']" +
                    "//div[@class='js-seller-info-changable c-product__seller-box']" +
                    "//div[@class='c-product__seller-row c-product__seller-row--price']" +
                    "//div[@class='c-product__seller-price-prev js-rrp-price u-hidden']";
                var priceElement = article_info.SelectSingleNode(priceQuery);
                var isExistPrice = priceElement != null;
                if (isExistPrice && priceElement.InnerText.Trim() != "")
                {
                    using (HtmlHelper html = new HtmlHelper())
                    {
                        dto.Price = Int64.Parse(html.NumberEN(article_info.SelectSingleNode(priceQuery).InnerText.Replace("\n", "").Replace(",", "").Trim()));
                    }
                }
                else
                {
                    var priceOff = article_info.SelectSingleNode(priceOffQuery);
                    if (priceOff != null)
                    {
                        using (HtmlHelper html = new HtmlHelper())
                        {
                            dto.Price = Int64.Parse(html.NumberEN(priceOff.InnerText
                                .Replace("\n", "").Replace("\r", "").Replace(",", "").Replace("تومان", "").Replace("  ", " ").Trim()));
                        }
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
                if (main.SelectNodes(tabsQuery) != null)
                {
                    var tabSections = main.SelectNodes(tabsQuery).ToArray();
                    List<ProductFeaturesDTO> features = new List<ProductFeaturesDTO>();
                    foreach (var feat in tabSections)
                    {
                        ProductFeaturesDTO p = new ProductFeaturesDTO();
                        p.Title = feat.ChildNodes[0].InnerText;
                        var _features = feat.ChildNodes[1].ChildNodes
                            .Select(x => new string[]
                            {
                        GetFeatureKey(x.ChildNodes[0].InnerHtml),
                        x.ChildNodes[1].InnerText.Replace("\n", "").Replace("          ", " ").Replace("     ", " ").Replace("     ", " ").Replace("  ", " ").Replace("  ", " ").Trim(),
                            }).ToArray();
                        int _lastFillNumber = 0;
                        int _TotalCount = _features.Count(x => x[0] != "");
                        string[][] _tempFeature = new string[_features.Count()][];
                        for (int i = 0; i < _features.Length; i++)
                        {
                            string currentKey = _features[i][0];
                            if (!string.IsNullOrEmpty(currentKey) || i == 0)
                            {
                                _lastFillNumber = i;
                                _tempFeature[i] = new string[] { currentKey, _features[i][1] };
                            }
                            else
                            {
                                List<string> _featuePlus = _tempFeature[_lastFillNumber].ToList();
                                _featuePlus.Add(_features[i][1]);
                                _tempFeature[_lastFillNumber] = _featuePlus.ToArray();
                            }
                        }
                        p.Features = _tempFeature.Where(x => x != null).ToList();
                        features.Add(p);
                    }
                    dto.Features = features;
                }
                #endregion

                dto = commentConcat(dto, url);
                //var jjj = JsonConvert.SerializeObject(dto);
                return (T)Convert.ChangeType(dto, typeof(DigikalaProductDTO));
            }
            catch (Exception ex)
            {
                LogDTO log = new LogDTO()
                {
                    _id = ObjectId.GenerateNewId().ToString(),
                    DateTime = DateTime.Now,
                    Description = ex.Message.ToString(),
                    ProjectId = (int)ProjectNames.Services,
                    Url = url,
                    MethodName = "Digikala - Digikala Helper - GetProduct",
                    Title = "GetProduct"
                };
                Logger.AddLog(log);
                return (T)Convert.ChangeType(null, typeof(DigikalaProductDTO));
            }
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
            using (var digi = new DigikalaMongoDBRepository())
            {
                digi.AddDigikalaBasePages(pageBases);
            }
            pageBases.Clear();
        }

        private DigikalaProductDTO commentConcat(DigikalaProductDTO dto, string url)
        {
            int? cmPageCount = (int?)null;
            int DKP = getDKPWithUrl(url);
            string firstCmUrl = GetReviewUrl(DKP);
            var firstCmPage = GetPage(firstCmUrl).Result;

            var doc = new HtmlDocument();
            doc.LoadHtml(firstCmPage);

            var rate = doc.DocumentNode.SelectNodes("//h2[@class='c-comments__headline']//span//span");
            if (rate != null)
            {
                using (HtmlHelper html = new HtmlHelper())
                {
                    dto.AvrageRate = int.Parse(html.NumberEN(rate[2].InnerText.Trim()));
                    dto.MaxRate = int.Parse(html.NumberEN(rate[3].InnerText.Replace("/", "").Trim()));
                    dto.TotalParticipantsCount = int.Parse(html.NumberEN(rate[4].InnerText.Replace("(", "").Replace(")", "").Replace("\n", "").Replace("نفر", "").Trim()));
                }
            }


            if (doc.DocumentNode.SelectSingleNode("//div[@class='c-comments__summary']//div[@class='c-comments__summary-box']") != null)
            {
                var _ratingItems = doc.DocumentNode
                    .SelectSingleNode("//div[@class='c-comments__summary']" +
                    "//div[@class='c-comments__summary-box']" +
                    "//ul[@class='c-comments__item-rating']").ChildNodes.ToArray().Where(x => x.Name.Contains("li"));

                List<string[]> ratingItems = new List<string[]>();

                foreach (var item in _ratingItems)
                {
                    var hasCell = item.SelectSingleNode("//div[@class='cell']") != null;
                    var cell = item.SelectSingleNode("//div[@class='cell']").InnerHtml.Replace("  ", " ").Trim();
                    if (hasCell)
                    {
                        var docCell = new HtmlDocument();
                        docCell.LoadHtml(item.InnerHtml.Replace("\n", "").Replace("  ", " ").Trim());

                        string[] _rate = new string[3];
                        _rate[0] = docCell.DocumentNode.SelectSingleNode("//div[@class='cell']").InnerText.Replace(":", "").Replace("\n", "").Trim();
                        _rate[1] = docCell.DocumentNode.SelectSingleNode("//div[@class='cell']//div[@class='c-rating c-rating--general js-rating']").Attributes["data-rate-digit"].Value.Replace(":", "").Replace("\n", "").Trim();
                        _rate[2] = docCell.DocumentNode.SelectSingleNode("//div[@class='cell']//div[@class='c-rating c-rating--general js-rating']//div[@class='c-rating__rate js-rating-value']").Attributes["data-rate-value"].Value.Replace("%", "").Replace("\n", "").Trim();
                        ratingItems.Add(_rate);
                    }
                }
                dto.RatingItems = ratingItems;
            }


            if (doc.GetElementbyId("comment-pagination") != null && doc.GetElementbyId("comment-pagination").SelectNodes("//ul[@class='c-pager__items']//li[@class='js-pagination-item']") != null)
            {
                cmPageCount = Int16.Parse(doc.GetElementbyId("comment-pagination").SelectNodes("//ul[@class='c-pager__items']//li[@class='js-pagination-item']").LastOrDefault().SelectNodes("//a[@class='c-pager__next']").Select(x => x.Attributes["data-page"].Value.ToString()).FirstOrDefault());
            }
            else if (doc.GetElementbyId("product-comment-list") != null && doc.GetElementbyId("product-comment-list").SelectNodes("//ul[@class='c-comments__list']") != null)
            {
                cmPageCount = 1;
            }
            if (cmPageCount.HasValue)
            {
                dto.Comments = GetComments(url, cmPageCount.Value);
            }
            return dto;
        }
        private int getDKPWithUrl(string url)
        {
            var indexDKP = url.LastIndexOf("dkp-");
            int lenghtDKP = url.Length - indexDKP;
            int indexEndChar = url.Substring(indexDKP).IndexOf("/");
            var uu = url.Length;
            string getDKP = url.Substring(indexDKP, indexEndChar < 1 ? lenghtDKP : indexEndChar).Replace("dkp-", "");

            List<string> splitUrl = new List<string>();
            splitUrl.AddRange(getDKP.Split("-"));
            if (splitUrl.Any())
            {
                foreach (var item in splitUrl)
                {
                    if (Regex.Match(item, @"^[0-9]*$").Success)
                    {
                        return int.Parse(item);

                    }
                }
            }
            return int.Parse(getDKP);
        }
        public string GetReviewUrl(int DKP, int Page = 1)
        {
            return string.Format("https://www.digikala.com/ajax/product/comments/{0}/?page={1}&mode=buyers", DKP, Page);
        }
        public string GetReviewListUrl(int DKP, int Page = 1)
        {
            return string.Format("https://www.digikala.com/ajax/product/comments/list/{0}/?page={1}&mode=buyers", DKP, Page);
        }
        public void AddProduct<T>(T dto)
        {
            DigikalaProductDTO digikalaProduct = (DigikalaProductDTO)Convert.ChangeType(dto, typeof(DigikalaProductDTO));
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                db.AddDigikalaProduct(ConvertProductDTOToEntity(digikalaProduct));
            }
        }
        public void AddProducts<T>(T dto)
        {
            AddProductsDigikala digikalaProducts = (AddProductsDigikala)Convert.ChangeType(dto, typeof(AddProductsDigikala));
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                db.AddDigikalaProducts(digikalaProducts.digikalaProducts.Select(x => ConvertProductDTOToEntity(x)).ToList());
            }
        }

        public async Task<T> GetAllBasePage<T>()
        {
            List<DigikalaPageBaseDTO> dtos = new List<DigikalaPageBaseDTO>();
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                dtos = db.GetAllBasePage();
            }
            GetAllBasePageDigikalaResult result = new GetAllBasePageDigikalaResult() { BasePages = dtos };
            return (T)Convert.ChangeType(result, typeof(GetAllBasePageDigikalaResult));
        }

        public void CrawledProduct(string id)
        {
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                db.CrwaledProduct(id);
            }
        }
        public void CrawledProducts(string[] ids)
        {
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                db.CrwaledProducts(ids);
            }
        }
        private DigikalaProduct ConvertProductDTOToEntity(DigikalaProductDTO dto)
        {
            DigikalaProduct m = new DigikalaProduct();
            m.AvrageRate = dto.AvrageRate;
            m.Brand = dto.Brand;
            m.Categories = dto.Categories;
            m.Category = dto.Category;
            m.Colors = dto.Colors;
            m.DKP = dto.DKP;
            m.Features = dto.Features == null ? null : dto.Features.Select(x => new ProductFeatures { Title = x.Title, Features = x.Features }).ToList();
            m.MaxRate = dto.MaxRate;
            m.Price = dto.Price;
            m.RatingItems = dto.RatingItems;
            m.Title = dto.Title;
            m.TitleEN = dto.TitleEN;
            m.TotalParticipantsCount = dto.TotalParticipantsCount;
            m.Url = dto.Url;
            m.Guaranteed = dto.Guaranteed;
            if (dto.Comments != null && dto.Comments.Count() > 0)
            {
                m.Comments = dto.Comments.Select(x => ConvertCommentDTOToEntity(x)).ToList();
            }
            return m;
        }
        private string GetFeatureKey(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            var className = doc.DocumentNode.FirstChild.Attributes["class"].Value;
            if (className == "block")
            {
                return doc.DocumentNode.FirstChild.InnerText.Replace("\n", "").Trim();
            }
            else
            {
                return doc.DocumentNode.SelectSingleNode("//a").InnerText.Replace("\n", "").Trim();
            }
        }
        private Comment ConvertCommentDTOToEntity(CommentDTO dto)
        {
            Comment m = new Comment()
            {
                Author = dto.Author,
                BoughtPrice = dto.BoughtPrice,
                Color = dto.Color,
                CommentDate = dto.CommentDate,
                CommentDisLike = dto.CommentDisLike,
                CommentId = dto.CommentId,
                CommentLike = dto.CommentLike,
                CreateDate = dto.CreateDate,
                Id = dto.Id,
                NegativeAspect = dto.NegativeAspect,
                OpinionType = dto.OpinionType,
                PageId = dto.PageId,
                PositiveAspect = dto.PositiveAspect,
                Purchased = dto.Purchased,
                Review = dto.Review,
                Seller = dto.Seller,
                SellerLink = dto.SellerLink,
                Title = dto.Title,
                Size = dto.Size
            };

            return m;
        }
        public async Task<T> GetFirstProductByCategory<T>(GetFirstProductByCategoryParam param)
        {
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                var result = db.GetFirstProductByCategory(param);
                return (T)Convert.ChangeType(result, typeof(DigikalaProductDTO));
            }
        }

        public bool AddReviewToDB(AddReviewToDBParam param)
        {
            try
            {
                using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
                {
                    Review review = new Review(param.review);
                    db.AddReview(review);
                    db.SetTaggedProduct(param.id, param.tagger);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogDTO log = new LogDTO()
                {
                    _id = ObjectId.GenerateNewId(DateTime.Now).ToString(),
                    DateTime = DateTime.Now,
                    Description = ex.Message,
                    ProjectId = (int)ProjectNames.Services,
                    Url = "DKP: " + param.review.ProductID,
                    MethodName = "AddReviewToDB",
                    Title = $"AddReviewToDB Error, Tagger: {param.tagger}"
                };
                Logger.AddLog(log);
                return false;
            }
        }
        public async Task<T> GetAllReviews<T>()
        {
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                var ss= db.GetAllReviews();
                return (T)Convert.ChangeType(ss, typeof(string[]));
            }
        }

        public string GetSatatusReview()
        {
            using (DigikalaMongoDBRepository db = new DigikalaMongoDBRepository())
            {
                var product = db.GetCountReview();
                var Sentences = db.GetCountSentences();
                return $"Products: {product} , Sentences: {Sentences}";
            }
        }
    }
}