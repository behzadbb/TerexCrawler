using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO.XmlSitemap;

namespace TerexCrawler.Models.DTO.Page
{
    public class DigikalaPageBaseDTO
    {
        public string _id { get; set; }
        public int DKP { get; set; }
        public string Loc { get; set; }
        public string ChangeFreq { get; set; }
        public string Priority { get; set; }
        public string ImageLoc { get; set; }
        public string ImageCaption { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? CrawlDate { get; set; }
        public bool Crawled { get; set; }
        public DigikalaPageBaseDTO(B5_Url b5)
        {
            Loc = b5.loc;
            ChangeFreq = b5.changefreq;
            Priority = b5.priority;
            if (b5.image.caption != null && b5.image.loc != null)
            {
                ImageLoc = b5.image.loc;
                ImageCaption = b5.image.caption;
            }
        }
        public DigikalaPageBaseDTO()
        {

        }
    }
}