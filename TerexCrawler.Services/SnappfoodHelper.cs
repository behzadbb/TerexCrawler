﻿//using AutoMapper;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TerexCrawler.DataLayer.Repository;
using TerexCrawler.Entites.Snappfood;
using TerexCrawler.HttpHelper;
using TerexCrawler.Models;
using TerexCrawler.Models.DTO.Comment;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Models.DTO.Page;
using TerexCrawler.Models.DTO.Snappfood;
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
                SnappfoodDTO product = new SnappfoodDTO();
                string id = getSnappfoodIdByUrl(url);
                string tmp_url = getSnappfoodCommentLink(id, 0);
                using (IHttpClientHelper clientHelper = new RestSharpHelper())
                {
                    var resultClient = clientHelper.GetHttp(tmp_url, true, user_agent);
                    product = JsonConvert.DeserializeObject<SnappfoodDTO>(resultClient.Content);
                    double pageCount = 0;
                    pageCount = Math.Round((double)(product.data.count / product.data.pageSize));
                    if (product.data.comments != null && product.data.comments.Any() && pageCount > 0)
                    {
                        for (int i = 1; i <= pageCount; i++)
                        {
                            try
                            {
                                System.Threading.Thread.Sleep(25);
                                string url1 = getSnappfoodCommentLink(id, i);
                                var resultClient1 = clientHelper.GetHttp(url1, true, user_agent);
                                if (!resultClient1.Success)
                                {
                                    System.Threading.Thread.Sleep(500);
                                    resultClient1 = clientHelper.GetHttp(url1, true, user_agent);
                                }
                                var comments1 = JsonConvert.DeserializeObject<SnappfoodDTO>(resultClient1.Content);
                                comments1.data.comments.ForEach(x => x.replies = null);
                                comments1.data.comments.ForEach(x => x.foods = new List<Food>());
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
                    product._id = ObjectId.GenerateNewId(DateTime.Now).ToString();
                    product.Reserve = false;
                    product.status = true;
                    product.isTagged = false;
                    product.Tagger = "_";
                    product.TagDate = DateTime.Now.AddYears(-10);
                }
                #endregion

                return (T)Convert.ChangeType(product, typeof(SnappfoodDTO));
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


        public bool AddReviewToDB(Review review, string id, string tagger)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetAllReviews<T>()
        {
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                GetReviewsMinimumResponse response = new GetReviewsMinimumResponse();
                List<SnappfoodMinInfo> reviews = db.GetAllReviewsMinumumInfo();
                response.ReviewsMinimum.AddRange(reviews);
                return (T)Convert.ChangeType(response, typeof(GetReviewsMinimumResponse));
            }
        }

        public bool AddReviewToDB(AddReviewToDBParam param)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetFirstProductByCategory<T>(GetFirstProductByCategoryParam param)
        {
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                ResturantReviewsDTO reviews = db.GetFirstSnappfood(param.tagger);
                return (T)Convert.ChangeType(reviews, typeof(ResturantReviewsDTO));
            }
        }

        public bool AddReviewToDB_NewMethod(AddReviewToDBParam param)
        {
            try
            {
                using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
                {
                    Review review = new Review(param.review);
                    db.AddReviewNew(review);
                    if (param.AutoOff)
                    {
                        db.SetTaggedProduct(int.Parse(param.id), param.tagger);
                    }
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
                    MethodName = "AddReviewToDB_NewMethod",
                    Title = $"AddReviewToDB_NewMethod Error, Tagger: {param.tagger}"
                };
                Logger.AddLog(log);
                return false;
            }
        }

        public Task<T> GetAllReviewObjects<T>(string cat)
        {
            throw new NotImplementedException();
        }

        public List<sentence> GetTopSentences()
        {
            try
            {
                using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
                {
                    return db.GetTopSentences();
                }
            }
            catch (Exception ex)
            {
                LogDTO log = new LogDTO()
                {
                    DateTime = DateTime.Now,
                    Description = "Error GetTopSentences, Error= " + ex.ToString(),
                    Title = "Get Top Sentences, Snappfood",
                    MethodName = "GetTopSentences",
                    ProjectId = 1,
                    Url = "Export/TopResturant"
                };
                Logger.AddLog(log);
                return new List<sentence>();
            }
        }

        public void AddRawReviewsToDB(AddResturatsDBParam param)
        {
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                db.AddResturantReviews(param.resturantReviews.Select(x => new ResturantReviews(x)).ToList());
            }
        }


        public List<Review> GetAllReviews1()
        {
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                string xml = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" + "\n";
                xml += "<Reviews>\n";

                var reviews = db.GetAllReviewsLabel();
                foreach (var review in reviews)
                {
                    xml += $"    <Review rid=\"{review.rid}\">\n";
                    xml += "        <sentences>\n";
                    for (int i = 0; i < review.sentences.Count; i++)
                    {
                        if (review.sentences[i].Opinions != null)
                        {
                            xml += $@"            <sentence id=""{review.rid}:{i}"">" + "\n";
                            xml += $"                <text>{review.sentences[i].Text}</text>\n";
                            if (review.sentences[i].Opinions != null && review.sentences[i].Opinions.Any())
                            {
                                xml += "                <Opinions>\n";
                                foreach (var op in review.sentences[i].Opinions)
                                {
                                    xml += @$"                    <Opinion target=""{op.category}"" category=""{op.category}#{op.aspect}"" polarity=""{op.polarity}"" />" + "\n";
                                }
                                xml += "                </Opinions>\n";
                            }
                            xml += $"            </sentence>\n";
                        }
                    }
                    xml += "        </sentences>\n";
                    xml += "    </Review>\n";
                }
                xml += "</Reviews>";
                File.WriteAllText(@"C:\Users\Administrator\Desktop\1.xml", xml);
                return reviews;
            }
        }

        public List<Review> GetLabelReviews()
        {
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                var allReviews = db.GetAllReviewsLabel();
                return allReviews;
            }
        }

        public void RejectReview(int id)
        {
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                db.Reject(id);
            }
        }

        public string GetSatatusReview()
        {
            using (SnappfoodMongoDBRepository db = new SnappfoodMongoDBRepository())
            {
                var product = db.GetCountReview();
                var Sentences = db.GetCountSentences();
                return $"Products: {product} , Sentences: {Sentences}";
            }
        }
    }


    public class AddProductsSnappfood
    {
        public List<Snappfood> Snappfoods { get; set; }
    }
}