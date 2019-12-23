using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerexCrawler.Apps.ReviewTaggerWPF;
using TerexCrawler.Models;
using TerexCrawler.Models.Const;
using TerexCrawler.Models.DTO;
using TerexCrawler.Models.DTO.Digikala;

namespace TerexCrawler.Apps.Web.Models
{
    public class TaggerVM
    {
        public ReviewDTO ReviewDTO { get; set; }
        public List<string> AspectsVm { get; set; }
        public DigikalaProductDTO ProductDTO { get; set; }
        public int ProductCount { get; set; }
        public string CommentJson { get; set; }
        public User User { get; set; }
        public string ProductName { get; set; }
        public string CommentTitle { get; set; }
        public string Review { get; set; }
        public string SelectReview { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string Tagger { get; set; }
        public TaggerVM()
        {
            AspectsVm = AspectsAir.AspectList;
        }
    }
    public class TaggerTest
    {
        public string Tagger { get; set; }
    }
}
