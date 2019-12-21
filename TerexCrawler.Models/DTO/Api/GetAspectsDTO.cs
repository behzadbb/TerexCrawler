using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.Const;

namespace TerexCrawler.Models.DTO.Api
{
    public class GetAspectsDTO
    {
        public int AspectType { get; set; }
    }
    public class GetAspectsResponseDTO
    {
        public List<Aspect> Aspects { get; set; }
        public Dictionary<string, string> Categories { get; set; }
        public Dictionary<string, string> CategoriesTitle { get; set; }
    }
}
