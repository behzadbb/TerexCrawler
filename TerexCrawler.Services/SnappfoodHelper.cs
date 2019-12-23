//using AutoMapper;
using HtmlAgilityPack;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TerexCrawler.Common;
using TerexCrawler.DataLayer.Repository;
using TerexCrawler.Entites.Digikala;
using TerexCrawler.Entites.Snappfood;
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
    public class SnappfoodHelper : IWebsiteCrawler
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
        string[] user_agent = {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362"
        };
        private const string sitename = "Snappfood";
        public string WebsiteName => sitename;
        public string WebsiteUrl => "https://Snappfood.com";
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
                    MethodName = "Snappfood - GetPage",
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
                    MethodName = "Snappfood - GetPage",
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
                    Title = "Convert To Standard DTO, Snappfood",
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
                #region GetProduct
                Snappfood product = new Snappfood();
                string id = getSnappfoodIdByUrl(url);
                string tmp_url = getSnappfoodCommentLink(id, 0);
                using (IHttpClientHelper clientHelper = new RestSharpHelper())
                {
                    var resultClient = clientHelper.GetHttp(tmp_url, true, user_agent);
                    product = JsonConvert.DeserializeObject<Snappfood>(resultClient.Content);
                    double pageCount = 0;
                    pageCount = Math.Round((double)(product.data.count / product.data.pageSize));
                    if (product.data.comments != null && product.data.comments.Any() && pageCount > 0)
                    {
                        for (int i = 1; i <= pageCount; i++)
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(50);
                                string url1 = getSnappfoodCommentLink(id, i);
                                var resultClient1 = clientHelper.GetHttp(url1, true, user_agent);
                                if (!resultClient1.Success)
                                {
                                    System.Threading.Thread.Sleep(500);
                                    resultClient1 = clientHelper.GetHttp(url1, true, user_agent);
                                }
                                var comments1 = JsonConvert.DeserializeObject<Snappfood>(resultClient1.Content);
                                if (comments1 != null && comments1.data.comments != null & comments1.data.comments.Any())
                                {
                                    product.data.comments.AddRange(comments1.data.comments);
                                }
                            }
                            catch (Exception)
                            {
                            }

                        }
                    }
                    //var cm = product.data.comments.Select(x => x.commentText.Replace("\n", ", ")).ToArray();
                    //var cmss = string.Join("\n", cm);
                    product.Url = url;
                    product.CreateDateTime = DateTime.Now;
                    product.Id = ObjectId.GenerateNewId(DateTime.Now);
                    product.Reserve = false;
                    product.status = true;
                    product.isTagged = false;
                    product.Tagger = "_";
                    product.TagDate = DateTime.MinValue;
                }
                #endregion

                return (T)Convert.ChangeType(product, typeof(Snappfood));
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
                    MethodName = "Snappfood - Snappfood Helper - GetProduct",
                    Title = "GetProduct"
                };
                Logger.AddLog(log);
                return (T)Convert.ChangeType(null, typeof(Snappfood));
            }
        }

        public void AddProduct<T>(T dto)
        {
            Snappfood snappfoodProduct = (Snappfood)Convert.ChangeType(dto, typeof(Snappfood));
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                db.AddSnappfood(snappfoodProduct);
            }
        }
        public void AddProducts<T>(T dto)
        {
            AddProductsSnappfood productsSnappfood = (AddProductsSnappfood)Convert.ChangeType(dto, typeof(AddProductsSnappfood));
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                db.AddSnappfoods(productsSnappfood.Snappfoods);
            }
        }

        public List<CommentDTO> GetComments(string url, int count)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllBasePage<T>()
        {
            throw new NotImplementedException();
        }

        public void CrawledProduct(string id)
        {
            throw new NotImplementedException();
        }

        public void CrawledProducts(string[] ids)
        {
            throw new NotImplementedException();
        }

        public void AddBasePages(List<B5_Url> dtos)
        {
            throw new NotImplementedException();
        }

        private static string getSnappfoodCommentLink(string id, int pageNumber = 0)
        {
            return $"https://snappfood.ir/restaurant/comment/vendor/{id}/{pageNumber}";
        }
        private static string getSnappfoodIdByUrl(string url)
        {
            //string url = "https://snappfood.ir/restaurant/menu/31n4w0/%27%2Bitem.link%2B%27";
            int start = url.IndexOf("menu/") + 5;
            int lenght = url.Length - start;
            url = url.Substring(start, lenght);
            int end = url.IndexOf("/");
            return url.Substring(0, end);
        }

        public Task<T> GetFirstProductByCategory<T>(string category, string title, string tagger)
        {
            throw new NotImplementedException();
        }

        public bool AddReviewToDB(Review review, string id, string tagger)
        {
            throw new NotImplementedException();
        }

        public object GetAllReviews()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllReviews<T>()
        {
            throw new NotImplementedException();
        }

        public bool AddReviewToDB(AddReviewToDBParam param)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetFirstProductByCategory<T>(GetFirstProductByCategoryParam param)
        {
            throw new NotImplementedException();
        }

        public string GetSatatusReview()
        {
            throw new NotImplementedException();
        }

        public bool AddReviewToDB_NewMethod(AddReviewToDBParam param)
        {
            throw new NotImplementedException();
        }
    }

    
    public class AddProductsSnappfood
    {
        public List<Snappfood> Snappfoods { get; set; }
    }
}