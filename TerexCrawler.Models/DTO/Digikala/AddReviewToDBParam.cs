using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Digikala
{
    public class AddReviewToDBParam
    {
        public string id { get; set; }
        public string tagger { get; set; }
        public Review review { get; set; }
    }
}
