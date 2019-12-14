using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Api
{
    public class GetAspectsDTO
    {
        public int AspectType { get; set; }
    }
    public class GetAspectsResponseDTO
    {
        public List<string> Aspects { get; set; }
    }
}
