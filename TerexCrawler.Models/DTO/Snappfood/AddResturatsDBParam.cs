using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Snappfood
{
    public class AddResturatsDBParam
    {
        public List<ResturantReviewsDTO> resturantReviews { get; set; }
    }
    public class ResturantReviewsDTO
    {
        public int _id { get; set; }
        public int CommentId { get; set; }
        public string RestId { get; set; }
        public string Review { get; set; }
        public string Tagger { get; set; }
        public DateTime Date { get; set; }
        public bool Seen { get; set; }
        public bool Reserve { get; set; }
        public DateTime ReserveDate { get; set; }
        public bool Tagged { get; set; }
        public DateTime TagDate { get; set; }
        public bool Reject { get; set; }
    }
}
