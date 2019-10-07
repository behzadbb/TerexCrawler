using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TerexCrawler.Models.DTO.XmlSitemap
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public struct Urlset
    {
        [XmlElement("url")]
        public B5_Url[] urlset;
    }

    [XmlType("url")]
    public struct B5_Url
    {
        [XmlElement("loc")]
        public string loc { get; set; }
        [XmlElement("changefreq")]
        public string changefreq { get; set; }
        [XmlElement("priority")]
        public string priority { get; set; }
        [XmlElement("image")]
        public image image { get; set; }
    }

    [XmlType("image")]
    public struct image
    {
        [XmlElement("loc")]
        public string loc { get; set; }
        [XmlElement("caption")]
        public string caption { get; set; }
    }
}
