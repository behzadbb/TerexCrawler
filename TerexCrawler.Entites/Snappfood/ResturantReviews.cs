using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO.Snappfood;

namespace TerexCrawler.Entites.Snappfood
{
    public class ResturantReviews
    {
        public ObjectId _id { get; set; }
        public int NumId { get; set; }
        public string Rid { get; set; }
        public string Review { get; set; }
        public string Tagger { get; set; }
        public DateTime Date  { get; set; }
        public bool Seen { get; set; }
        public bool Reserve { get; set; }
        public bool Tagged { get; set; }
        public ResturantReviews(ResturantReviewsDTO dto)
        {
            this._id = ObjectId.Parse(dto._id);
            this.Date = dto.Date;
            this.NumId = dto.NumId;
            this.Reserve = dto.Reserve;
            this.Review = dto.Review;
            this.Rid = dto.Rid;
            this.Seen = dto.Seen;
            this.Tagged = dto.Tagged;
            this.Tagger = dto.Tagger;
        }
    }
}
