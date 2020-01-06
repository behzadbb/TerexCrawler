using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerexCrawler.Apps.Web.Models
{
    public class RejectReviewParam
    {
        public int Id { get; set; }
        public string Tagger { get; set; }
    }
    public class RejectReviewResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
