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
        //private MongoCollection<DigikalaBasePage> digikalaBasePages;
        private MongoCollection<Snappfood> snappfoodProducts;

        public SnappfoodMongoDBRepository()
        {
            client = new MongoClient("mongodb://localhost");
            server = client.GetServer();
            db = server.GetDatabase("SnappfoodDB");
            //digikalaBasePages = db.GetCollection<DigikalaBasePage>("DigikalaBasePages");
            snappfoodProducts = db.GetCollection<Snappfood>("Resturants");
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

        public Snappfood GetFirstSnappfood(string category, string title, string tagger)
        {
            var query = Query<Snappfood>.Where(x => x.Reviews.Any() && !x.isTagged && !x.Reserve && x.Reviews.Count < 30);

            var update = Update<Snappfood>.Set(p => p.Tagger, tagger).Set(p => p.Reserve, true);

            snappfoodProducts.Update(query, update);
            var product = snappfoodProducts.FindOne(query);
            //var laptop = digikalaProducts.FindOne();
            return product;
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
