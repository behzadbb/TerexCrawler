using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Snappfood
{
    public class SnappfoodMinInfo
    {
        public string RestId { get; set; }
        public string Review { get; set; }
        public int CommentId { get; set; }
    }

    public class GetReviewsMinimumResponse
    {
        public List<SnappfoodMinInfo> ReviewsMinimum { get; set; }
        public GetReviewsMinimumResponse()
        {
            ReviewsMinimum = new List<SnappfoodMinInfo>();
        }
    }
}
