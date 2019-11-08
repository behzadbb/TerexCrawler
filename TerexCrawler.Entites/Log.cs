using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TerexCrawler.Entites
{
    public class Log
    {
        [BsonId]
        public ObjectId _id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        public string MethodName { get; set; }

        public string Url { get; set; }
    }
}
