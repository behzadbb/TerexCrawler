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
        public string _id { get; set; }
        public int NumId { get; set; }
        public string Rid { get; set; }
        public string Review { get; set; }
        public string Tagger { get; set; }
        public DateTime Date { get; set; }
        public bool Seen { get; set; }
        public bool Reserve { get; set; }
        public bool Tagged { get; set; }
    }
}
