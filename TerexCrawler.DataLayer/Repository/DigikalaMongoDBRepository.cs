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
        MongoClient client = new MongoClient("mongodb://localhost");
        MongoServer server => client.GetServer();
        MongoDatabase db => server.GetDatabase("Digikala");

        //MongoCollection<Person> collection = db.GetCollection<Person>("people");

        public void AddDigikalaBasePage(DigikalaPageBaseDTO dto)
        {
            DigikalaBasePage model = new DigikalaBasePage();
            model.CreateDate = DateTime.Now;
            model.ChangeFreq = dto.ChangeFreq;
            model.ImageCaption = dto.ImageCaption;
            model.ImageLoc = dto.ImageLoc;
            model.Loc = dto.Loc;
            model.Priority = dto.Priority;

            MongoCollection<BsonDocument> digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");

            digikalaCollection.Insert(model);
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

            MongoCollection<BsonDocument> digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");

            digikalaCollection.InsertBatch(models);
        }

        public void AddDgikalaProduct(DigikalaProduct dto)
        {
            MongoCollection<BsonDocument> digikalaCollection = db.GetCollection<BsonDocument>("DigikalaProduct");

            digikalaCollection.Insert(dto);
        }
        
        public void AddDgikalaProducts(List<DigikalaProduct> dtos)
        {
            MongoCollection<BsonDocument> digikalaCollection = db.GetCollection<BsonDocument>("DigikalaProduct");

            digikalaCollection.Insert(dtos);
        }
    }
}
