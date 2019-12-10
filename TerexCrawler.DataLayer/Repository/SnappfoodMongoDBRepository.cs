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
            var query = Query<Snappfood>.Where(x => x.data.comments.Any() && !x.isTagged && !x.Reserve && x.data.comments.Count < 30);

            var update = Update<Snappfood>.Set(p => p.Tagger, tagger).Set(p => p.Reserve, true);

            snappfoodProducts.Update(query, update);
            var product = snappfoodProducts.FindOne(query);
            //var laptop = digikalaProducts.FindOne();
            return product;
        }

        public List<Snappfood> GetAllSnappfood()
        {
            var snappfoodsResult = snappfoodProducts.FindAll().ToList();
            return snappfoodsResult;
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
