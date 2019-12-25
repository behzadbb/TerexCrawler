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
        public Aspects Aspects { get; set; }
    }
}
