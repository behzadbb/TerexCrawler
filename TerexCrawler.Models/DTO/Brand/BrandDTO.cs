using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Brand
{
    public class BrandDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
