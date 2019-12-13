using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TerexCrawler.Models;

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
        public Log(LogDTO dto)
        {
            this.DateTime = dto.DateTime;
            this.Description = dto.Description;
            this.MethodName = dto.MethodName;
            this.ProjectId = dto.ProjectId;
            this.Title = dto.Title;
            this.Url = dto.Url;
            if (!string.IsNullOrEmpty(dto._id) && ObjectId.TryParse(dto._id, out ObjectId objectId))
            {
                this._id = objectId;
            }
            else
                this._id = ObjectId.GenerateNewId(DateTime.Now);
        }
    }
}
