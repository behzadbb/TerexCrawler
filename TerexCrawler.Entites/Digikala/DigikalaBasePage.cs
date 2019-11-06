using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Entites.Digikala
{
    public class DigikalaBasePage
    {
        public ObjectId _id { get; set; }
        public int DKP { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CrawlDate { get; set; }
        public bool Crawled { get; set; }
        public string Loc { get; set; }
        public string ChangeFreq { get; set; }
        public string Priority { get; set; }
        public string ImageLoc { get; set; }
        public string ImageCaption { get; set; }
    }
}
