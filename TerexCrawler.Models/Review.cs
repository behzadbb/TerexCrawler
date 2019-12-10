using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models
{
    [Serializable]
    public class Review
    {
        [BsonId]
        public Guid rid { get; set; }
        public long ProductID { get; set; }
        public List<sentence> sentences { get; set; }
    }
    public class sentence
    {
        public int id { get; set; }
        public string Text { get; set; }
        public List<Opinion> Opinions { get; set; }
    }
    public class Opinion
    {
        public string polarity { get; set; }
        public int polarityClass { get; set; }
        public string category { get; set; }
        public string categoryClass { get; set; }
    }
}