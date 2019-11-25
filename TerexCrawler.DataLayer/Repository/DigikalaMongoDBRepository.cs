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

        public DigikalaMongoDBRepository()
        {
            client = new MongoClient("mongodb://localhost");
            server = client.GetServer();
            db = server.GetDatabase("Digikala");
            digikalaBasePages = db.GetCollection<DigikalaBasePage>("DigikalaBasePages");
            digikalaProducts = db.GetCollection<DigikalaProduct>("DigikalaProducts");
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

        public DigikalaProductDTO GetFirstProductByCategory(string category)
        {
            var laptop = digikalaProducts.FindAll()
                        .Where(p => p.Category.Contains(category) && p.Comments.Any() && p.Comments.Count() > 0)
                        .FirstOrDefault();
            DigikalaProductDTO digikalaProduct = new DigikalaProductDTO();
            digikalaProduct.AvrageRate = laptop.AvrageRate;
            digikalaProduct.Brand = laptop.Brand;
            digikalaProduct.Categories = laptop.Categories;
            digikalaProduct.Category = laptop.Category;
            digikalaProduct.Colors = laptop.Colors;
            digikalaProduct.Comments = laptop.Comments.Select(x => new Models.DTO.Comment.CommentDTO
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
            digikalaProduct.DKP = laptop.DKP;
            digikalaProduct.Features = laptop.Features == null ? null : laptop.Features.Select(x => new ProductFeaturesDTO { Title = x.Title, Features = x.Features }).ToList();
            digikalaProduct.Guaranteed = laptop.Guaranteed;
            digikalaProduct.MaxRate = laptop.MaxRate;
            digikalaProduct.Price = laptop.Price;
            digikalaProduct.RatingItems = laptop.RatingItems;
            digikalaProduct.Title = laptop.Title;
            digikalaProduct.TitleEN = laptop.TitleEN;
            digikalaProduct.TotalParticipantsCount = laptop.TotalParticipantsCount;
            digikalaProduct.Url = laptop.Url;

            return digikalaProduct;
        }
    }
}
