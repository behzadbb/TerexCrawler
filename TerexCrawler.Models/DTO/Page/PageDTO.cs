using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO.Brand;
using TerexCrawler.Models.DTO.Comment;

namespace TerexCrawler.Models.DTO.Page
{
    public class PageDTO
    {
        public int Id { get; set; }
        public long PageId { get; set; }
        public DateTime? RegDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
        public BrandDTO Brand { get; set; }
        public List<CommentDTO> Comments { get; set; }
        public string[] Colors { get; set; }
        public Dictionary<string,string> TopFe { get; set; }
    }
}
