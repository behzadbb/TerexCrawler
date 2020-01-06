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
using TerexCrawler.Entites.Snappfood;
using TerexCrawler.Models.DTO.Snappfood;

namespace TerexCrawler.DataLayer.Repository
{
    public class SnappfoodMongoDBRepository : IDisposable
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
        private MongoDatabase dbRest;
        //private MongoCollection<DigikalaBasePage> digikalaBasePages;
        private MongoCollection<Snappfood> snappfoodProducts;
        private MongoCollection<ResturantReviews> resturantCollection;

        public SnappfoodMongoDBRepository()
        {
            client = new MongoClient("mongodb://localhost");
            server = client.GetServer();
            db = server.GetDatabase("SnappfoodDB");
            dbRest = server.GetDatabase("ResturantDB");
            //digikalaBasePages = db.GetCollection<DigikalaBasePage>("DigikalaBasePages");
            snappfoodProducts = db.GetCollection<Snappfood>("Resturants");
            resturantCollection = dbRest.GetCollection<ResturantReviews>("ResturantReviews");
        }

        //public void AddDigikalaBasePage(DigikalaPageBaseDTO dto)
        //{
        //    DigikalaBasePage model = new DigikalaBasePage();
        //    model.CreateDate = DateTime.Now;
        //    model.ChangeFreq = dto.ChangeFreq;
        //    model.ImageCaption = dto.ImageCaption;
        //    model.ImageLoc = dto.ImageLoc;
        //    model.Loc = dto.Loc;
        //    model.Priority = dto.Priority;

        //    digikalaBasePages.Insert(model);
        //}

        //public void AddDigikalaBasePages(List<DigikalaPageBaseDTO> dtos)
        //{
        //    List<DigikalaBasePage> models = new List<DigikalaBasePage>();
        //    DateTime CreateDate = DateTime.Now;
        //    foreach (var dto in dtos)
        //    {
        //        DigikalaBasePage model = new DigikalaBasePage();
        //        model.DKP = dto.DKP;
        //        model.Crawled = false;
        //        model.CreateDate = CreateDate;
        //        model.ChangeFreq = dto.ChangeFreq;
        //        model.ImageCaption = dto.ImageCaption;
        //        model.ImageLoc = dto.ImageLoc;
        //        model.Loc = dto.Loc;
        //        model.Priority = dto.Priority;
        //        model.CrawlDate = null;
        //        models.Add(model);
        //    }

        //    digikalaBasePages.InsertBatch(models);
        //}

        public void AddSnappfood(Snappfood dto)
        {
            snappfoodProducts.Insert(dto);
        }

        public async void AddSnappfoods(List<Snappfood> dtos)
        {
            snappfoodProducts.InsertBatch(dtos);
        }

        public ResturantReviewsDTO GetFirstSnappfood(string tagger)
        {
            var update = Update<ResturantReviews>.Set(p => p.Tagger, tagger).Set(p => p.Reserve, true).Set(p => p.ReserveDate, DateTime.Now);
            try
            {
                var query1 = Query<ResturantReviews>.Where(x => x.Reserve && !x.Tagged && x.Tagger == tagger && !x.Reject);
                var review1 = resturantCollection.FindOne(query1);
                if (review1 != null && review1.Review !=null && review1.Review !="")
                {

                    var query3 = Query<ResturantReviews>.Where(x => x._id == review1._id);
                    resturantCollection.Update(query3, update);
                    return review1.ConvertToDTO();
                }
            }
            catch (Exception)
            {
            }

            var query2 = Query<ResturantReviews>.Where(x => !x.Reserve && !x.Tagged && !x.Reject);
            var review2 = resturantCollection.FindOne(query2);

            var query4 = Query<ResturantReviews>.Where(x => x._id == review2._id);
            resturantCollection.Update(query4, update);

            return review2.ConvertToDTO();
        }

        public void SetReject(int id,string tagger)
        {
            try
            {
                var query1 = Query<ResturantReviews>.Where(x => x._id == id);
                var review1 = resturantCollection.FindOne(query1);
                if (review1 != null && review1.Review != null && review1.Review != "")
                {
                    var update = Update<ResturantReviews>.Set(p => p.Tagger, tagger).Set(p => p.Reject, true).Set(p => p.TagDate, DateTime.Now).Set(p => p.Tagged, false);
                    resturantCollection.Update(query1, update);
                }
            }
            catch (Exception)
            {
            }
        }

        public List<Snappfood> GetAllSnappfood()
        {
            try
            {
                var snappfoodsResult = snappfoodProducts.FindAll().ToList();
                return snappfoodsResult;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return new List<Snappfood>();
            }
        }

        public object GetAllReviews()
        {
            //List<Comment> comments = new List<Comment>();
            var query = Query<Snappfood>.Where(x => x.Reviews.Count > 0);
            var products = snappfoodProducts.FindAll().ToList();
            var ssss = products.Select(x => x.Reviews.ToList());
            var sssss = from c in products.Select(x => x.Reviews)
                        from r in c.Select(x => x)
                        select r.CommentText;
            //string[] titles = products.Comments.Select(x => x.Title).ToArray();
            //var aspects = products.Comments.Where(x => x.NegativeAspect.Any()).Select(x => x.NegativeAspect);
            return sssss.ToArray();
        }
        
        public List<SnappfoodMinInfo> GetAllReviewsMinumumInfo()
        {
            var query = Query<Snappfood>.Where(x => x.Reviews.Count > 0);
            var products = snappfoodProducts.FindAll().ToList();
            var reviewsinfo = new List<SnappfoodMinInfo>();
            for (int i = 0; i < products.Count; i++)
            {
                var restId = products[i]._id.ToString();
                reviewsinfo.AddRange(products[i].Reviews.Select(x => new SnappfoodMinInfo { RestId = restId, Review = x.CommentText, CommentId = x.CommentId }).ToList());
            }
            return reviewsinfo;
        }

        public async void AddResturantReview(ResturantReviews resturantReview)
        {
            resturantCollection.Insert(resturantReview);
        }
        public async void AddResturantReviews(List<ResturantReviews> resturantReviews)
        {
            resturantCollection.InsertBatch(resturantReviews);

        }
        public List<ResturantReviews> GetFirstResturantReview()
        {
            return resturantCollection.FindAll().ToList();
        }

        //public void CrwaledProduct(string id)
        //{
        //    ObjectId _id = ObjectId.Parse(id);
        //    var query = Query<DigikalaBasePage>.EQ(p => p._id, _id);
        //    var update = Update<DigikalaBasePage>.Set(p => p.CrawlDate, DateTime.Now).Set(p => p.Crawled, true);

        //    digikalaBasePages.Update(query, update);
        //}
        //public async void CrwaledProducts(string[] ids)
        //{
        //    BsonArray array = new BsonArray();
        //    array.AddRange(ids.Select(x => ObjectId.Parse(x)));
        //    var query = Query<DigikalaBasePage>.In(x => x._id, array);
        //    var basePages = digikalaBasePages.Find(query);
        //    foreach (var item in basePages)
        //    {
        //        item.Crawled = true;
        //        item.CrawlDate = DateTime.Now;
        //        digikalaBasePages.Save(item);
        //    }
        //}

        //public void RemoveBasePage(string url)
        //{
        //    var query = Query<DigikalaBasePage>.Matches(x => x.Loc, BsonRegularExpression.Create(new Regex(url)));
        //    digikalaBasePages.Remove(query);
        //}
    }
}
