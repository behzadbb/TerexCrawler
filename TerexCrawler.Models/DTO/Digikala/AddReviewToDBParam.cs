using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Digikala
{
    public class AddReviewToDBParam
    {
        public string id { get; set; }
        public string tagger { get; set; }
        public ReviewDTO review { get; set; }
    }

    public class AddReviewToDBResponse
    {
        public bool Success { get; set; }
    }
}
