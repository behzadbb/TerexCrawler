using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO.Snappfood;

namespace TerexCrawler.Entites.Snappfood
{
    public class ResturantReviews
    {
        [BsonId]
        public int _id { get; set; }
        public int CommentId { get; set; }
        public string RestId { get; set; }
        public string Review { get; set; }
        public string Tagger { get; set; }
        public DateTime Date  { get; set; }
        public bool Seen { get; set; }
        public bool Reserve { get; set; }
        public DateTime ReserveDate { get; set; }
        public bool Tagged { get; set; }
        public DateTime TagDate { get; set; }
        public bool Reject { get; set; }
        
        public ResturantReviews(ResturantReviewsDTO dto)
        {
            this._id = dto._id;
            this.Date = dto.Date;
            this.CommentId = dto.CommentId;
            this.Reserve = dto.Reserve;
            this.Review = dto.Review;
            this.RestId = dto.RestId;
            this.Seen = dto.Seen;
            this.Tagged = dto.Tagged;
            this.Tagger = dto.Tagger;
            this.ReserveDate = dto.ReserveDate;
            this.TagDate = dto.TagDate;
            this.Reject = dto.Reject;
        }
        public ResturantReviewsDTO ConvertToDTO()
        {
            ResturantReviewsDTO dto = new ResturantReviewsDTO();
            dto._id = _id;
            dto.Date = this.Date;
            dto.CommentId = this.CommentId;
            dto.Reserve = this.Reserve;
            dto.Review = this.Review;
            dto.RestId = this.RestId;
            dto.Seen = this.Seen;
            dto.Tagged = this.Tagged;
            dto.Tagger = this.Tagger;
            dto.Reject = this.Reject;

            return dto;
        }
    }
}
