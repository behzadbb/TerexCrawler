using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Builders;
using TerexCrawler.Entites;
using TerexCrawler.Models.DTO.Page;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Entites.Digikala;
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.IO;
using System.Text.RegularExpressions;
using TerexCrawler.Models;

namespace TerexCrawler.DataLayer.Repository
{
    public class DigikalaMongoDBRepository : IDisposable
    {
        #region IDisposable Support
        private bool disposedValue = false;

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
            server.Disconnect();
            Dispose(true);

        }
        #endregion

        private MongoClient client;
        private MongoServer server;
        private MongoDatabase db;
        private MongoCollection<DigikalaBasePage> digikalaBasePages;
        private MongoCollection<DigikalaProduct> digikalaProducts;
        private MongoCollection<Review> digikalaReview;

        public DigikalaMongoDBRepository()
        {
            client = new MongoClient("mongodb://localhost");
            server = client.GetServer();
            db = server.GetDatabase("Digikala");
            digikalaBasePages = db.GetCollection<DigikalaBasePage>("DigikalaBasePages");
            digikalaProducts = db.GetCollection<DigikalaProduct>("DigikalaProducts");
            digikalaReview = db.GetCollection<Review>("DigikalaReviews");
        }

        public void AddDigikalaBasePage(DigikalaPageBaseDTO dto)
        {
            DigikalaBasePage model = new DigikalaBasePage();
            model.CreateDate = DateTime.Now;
            model.ChangeFreq = dto.ChangeFreq;
            model.ImageCaption = dto.ImageCaption;
            model.ImageLoc = dto.ImageLoc;
            model.Loc = dto.Loc;
            model.Priority = dto.Priority;

            digikalaBasePages.Insert(model);
        }

        public void AddDigikalaBasePages(List<DigikalaPageBaseDTO> dtos)
        {
            List<DigikalaBasePage> models = new List<DigikalaBasePage>();
            DateTime CreateDate = DateTime.Now;
            foreach (var dto in dtos)
            {
                DigikalaBasePage model = new DigikalaBasePage();
                model.DKP = dto.DKP;
                model.Crawled = false;
                model.CreateDate = CreateDate;
                model.ChangeFreq = dto.ChangeFreq;
                model.ImageCaption = dto.ImageCaption;
                model.ImageLoc = dto.ImageLoc;
                model.Loc = dto.Loc;
                model.Priority = dto.Priority;
                model.CrawlDate = null;
                models.Add(model);
            }

            digikalaBasePages.InsertBatch(models);
        }

        public void AddDigikalaProduct(DigikalaProduct dto)
        {
            digikalaProducts.Insert(dto);
        }

        public async void AddDigikalaProducts(List<DigikalaProduct> dtos)
        {
            digikalaProducts.InsertBatch(dtos);
        }

        public List<DigikalaPageBaseDTO> GetAllBasePage()
        {
            var digikalaBases = digikalaBasePages.FindAll()
                        .Where(p => !p.Crawled && p.Loc.Contains("dkp-")).Take(40000);
            List<DigikalaPageBaseDTO> sas = digikalaBases.Select(
                    x => new DigikalaPageBaseDTO
                    {
                        _id = x._id.ToString(),
                        DKP = x.DKP,
                        Crawled = x.Crawled,
                        CreateDate = x.CreateDate,
                        ChangeFreq = x.ChangeFreq,
                        ImageCaption = x.ImageCaption,
                        ImageLoc = x.ImageLoc,
                        Loc = x.Loc,
                        Priority = x.Priority,
                        CrawlDate = x.CrawlDate
                    }).ToList();
            return sas;
        }

        public void CrwaledProduct(string id)
        {
            ObjectId _id = ObjectId.Parse(id);
            var query = Query<DigikalaBasePage>.EQ(p => p._id, _id);
            var update = Update<DigikalaBasePage>.Set(p => p.CrawlDate, DateTime.Now).Set(p => p.Crawled, true);

            digikalaBasePages.Update(query, update);
        }
        public async void CrwaledProducts(string[] ids)
        {
            BsonArray array = new BsonArray();
            array.AddRange(ids.Select(x => ObjectId.Parse(x)));
            var query = Query<DigikalaBasePage>.In(x => x._id, array);
            var basePages = digikalaBasePages.Find(query);
            foreach (var item in basePages)
            {
                item.Crawled = true;
                item.CrawlDate = DateTime.Now;
                digikalaBasePages.Save(item);
            }
        }

        public void RemoveBasePage(string url)
        {
            try
            {
                var query = Query<DigikalaBasePage>.Where(x => x.Loc == url);
                digikalaBasePages.Remove(query);
            }
            catch (Exception)
            {
            }
        }

        public DigikalaProductDTO GetFirstProductByCategory(GetFirstProductByCategoryParam param)
        {
            var query = Query<DigikalaProduct>.Where(x => x.Category == param.category &&
                                                          (string.IsNullOrEmpty(param.title) || x.Title.Contains(param.title)) &&
                                                          (string.IsNullOrEmpty(param.Brand) || x.Brand != null && x.Brand.Contains(param.Brand)) &&
                                                          x.Comments.Any() &&
                                                          x.Reserved == false &&
                                                          !x.isTagged);

            var update = Update<DigikalaProduct>.Set(p => p.Reserved, true).Set(p => p.Tagger, param.tagger);

            digikalaProducts.Update(query, update);

            System.Threading.Thread.Sleep(50);

            var product = digikalaProducts.FindOne(query);
            DigikalaProductDTO digikalaProduct = new DigikalaProductDTO();
            digikalaProduct._id = product._id.ToString();
            digikalaProduct.AvrageRate = product.AvrageRate;
            digikalaProduct.Brand = product.Brand;
            digikalaProduct.Categories = product.Categories;
            digikalaProduct.Category = product.Category;
            digikalaProduct.Colors = product.Colors;
            digikalaProduct.Comments = product.Comments.Select(x => new Models.DTO.Comment.CommentDTO
            {
                Author = x.Author,
                BoughtPrice = x.BoughtPrice,
                Color = x.Color,
                CommentDate = x.CommentDate,
                CommentDisLike = x.CommentDisLike,
                CommentId = x.CommentId,
                CommentLike = x.CommentLike,
                CreateDate = x.CreateDate,
                Id = x.Id,
                NegativeAspect = x.NegativeAspect,
                OpinionType = x.OpinionType,
                PageId = x.PageId,
                PositiveAspect = x.PositiveAspect,
                Purchased = x.Purchased,
                Review = x.Review,
                Seller = x.Seller,
                SellerLink = x.SellerLink,
                Title = x.Title,
                Size = x.Size
            }).ToList();
            digikalaProduct.DKP = product.DKP;
            digikalaProduct.Features = product.Features == null ? null : product.Features.Select(x => new ProductFeaturesDTO { Title = x.Title, Features = x.Features }).ToList();
            digikalaProduct.Guaranteed = product.Guaranteed;
            digikalaProduct.MaxRate = product.MaxRate;
            digikalaProduct.Price = product.Price;
            digikalaProduct.RatingItems = product.RatingItems;
            digikalaProduct.Title = product.Title;
            digikalaProduct.TitleEN = product.TitleEN;
            digikalaProduct.TotalParticipantsCount = product.TotalParticipantsCount;
            digikalaProduct.Url = product.Url;
            return digikalaProduct;
        }

        public bool AddReview(Review review)
        {
            if (review._id == null || review._id.ToString().Contains("000"))
            {
                review._id = ObjectId.GenerateNewId(DateTime.Now);
            }
            digikalaReview.Insert(review);
            return true;
        }

        public bool AddReviewNew(Review review)
        {
            var query = Query<Review>.Where(x => x.ProductID == review.ProductID);

            var r = digikalaReview.FindOne(query);
            if (r == null)
            {
                if (review._id == null || review._id.ToString().Contains("000"))
                {
                    review._id = ObjectId.GenerateNewId(DateTime.Now);
                }
                digikalaReview.Insert(review);
                return true;
            }

            foreach (var item in review.sentences)
            {
                if (!r.sentences.Where(x => x.Text == item.Text).Any())
                {
                    r.sentences.Add(item);

                }
            }
            var update = Update<Review>.Set(p => p.sentences, r.sentences);

            digikalaReview.Update(query, update);
            return true;
        }

        public void SetTaggedProduct(string id, string tagger)
        {
            var query = Query<DigikalaProduct>.Where(x => x._id == ObjectId.Parse(id));

            var update = Update<DigikalaProduct>
                .Set(p => p.Reserved, false)
                .Set(p => p.Tagger, tagger)
                .Set(p => p.TaggedDate, DateTime.Now)
                .Set(p => p.isTagged, true);

            digikalaProducts.Update(query, update);
        }

        public object GetAllReviews()
        {
            List<Comment> comments = new List<Comment>();
            var query = Query<DigikalaProduct>.Where(x => x.Comments != null && x.Comments.Count > 0);
            List<DigikalaProduct> products = digikalaProducts.Find(query).ToList();//.Comments.FirstOrDefault().Review;
            var ssss = from c in products.Select(x => x.Comments)
                       from r in c.Select(x => x)
                       select r.Review;
            //string[] titles = products.Comments.Select(x => x.Title).ToArray();
            //var aspects = products.Comments.Where(x => x.NegativeAspect.Any()).Select(x => x.NegativeAspect);
            return ssss.ToArray();
        }
        public List<DigikalaProduct> GetAllReviews(string cat)
        {
            List<Comment> comments = new List<Comment>();
            var query = Query<DigikalaProduct>.Where(x => x.Comments != null && x.Category == cat);
            return digikalaProducts.Find(query).ToList();
        }

        public long GetCountReview()
        {
            return digikalaReview.Count();
        }

        public long GetCountSentences()
        {
            var reviews = digikalaReview.FindAll().Where(x => x.sentences.Any() && x.sentences.Count() > 0);
            return reviews.Sum(s => s.sentences.Count());
        }
        public List<Review> GetAllReviewsLabel()
        {
            return digikalaReview.FindAll().ToList();
        }

        public List<sentence> GetTopSentences()
        {
            List<sentence> sentences = new List<sentence>();
            var _temp = digikalaReview.FindAll().ToList();
            var _temp1 = _temp.Select(x => x.sentences).ToList();
            foreach (var item in _temp1)
            {
                sentences.AddRange(item);
            }
            return sentences.ToList();
        }
    }
}
