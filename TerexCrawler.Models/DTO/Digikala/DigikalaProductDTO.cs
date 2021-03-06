﻿using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO.Comment;

namespace TerexCrawler.Models.DTO.Digikala
{
    public class DigikalaProductDTO
    {
        public string _id { get; set; }
        public int DKP { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string TitleEN { get; set; }
        public int? AvrageRate { get; set; }
        public int? MaxRate { get; set; }
        public int? TotalParticipantsCount { get; set; }
        public List<string> Colors { get; set; }
        public string Category { get; set; }
        public List<string> Categories { get; set; }
        public long Price { get; set; }
        public string Brand { get; set; }
        public short? Guaranteed { get; set; }
        public List<ProductFeaturesDTO> Features { get; set; }
        public List<string[]> RatingItems { get; set; }
        public List<CommentDTO> Comments { get; set; }
        public bool isTagged { get; set; }
        public DateTime TaggedDate { get; set; }
        public string Tagger { get; set; }
    }
    public class ProductFeaturesDTO
    {
        public string Title { get; set; }
        public List<string[]> Features { get; set; }
    }
}
