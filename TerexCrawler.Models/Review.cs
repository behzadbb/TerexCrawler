using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models
{
    public class ReviewDTO
    {
        public string _id { get; set; }
        public DateTime CreateDate { get; set; }
        public int rid { get; set; }
        public long ProductID { get; set; }
        public List<sentence> sentences { get; set; }
    }
    [Serializable]
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }
        public DateTime CreateDate { get; set; }
        public int rid { get; set; }
        public long ProductID { get; set; }
        public List<sentence> sentences { get; set; }
        public Review(ReviewDTO dto)
        {
            this.CreateDate = dto.CreateDate;
            this.ProductID = dto.ProductID;
            this.rid = dto.rid;
            this.sentences = dto.sentences;
            this._id = ObjectId.GenerateNewId(DateTime.Now);
        }
    }
    public class sentence
    {
        public int id { get; set; }
        public string Text { get; set; }
        public List<Opinion> Opinions { get; set; }
        public bool OutOfScope { get; set; }
    }
    public class Opinion
    {
        public string polarity { get; set; }
        public int polarityClass { get; set; }
        public string category { get; set; }
        public string categoryClass { get; set; }
    }
}