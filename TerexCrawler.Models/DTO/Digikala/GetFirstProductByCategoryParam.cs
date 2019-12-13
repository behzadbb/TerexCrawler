using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Digikala
{
    public class GetFirstProductByCategoryParam
    {
        public string category { get; set; }
        public string title { get; set; }
        public string tagger { get; set; }
        public string Brand { get; set; }
    }
}
