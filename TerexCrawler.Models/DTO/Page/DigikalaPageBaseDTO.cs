using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO.Page
{
    public struct DigikalaPageBaseDTO
    {
        public string Loc { get; set; }
        public string ChangeFreq { get; set; }
        public string Priority { get; set; }
        public string ImageLloc { get; set; }
        public string ImageCaption { get; set; }
    }
}
