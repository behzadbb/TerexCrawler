﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TerexCrawler.Models;
using TerexCrawler.Models.Const;
using TerexCrawler.Models.DTO;
using TerexCrawler.Models.DTO.Digikala;

namespace TerexCrawler.Apps.Web.Models
{
    public class TaggerVM
    {
        public string idBson { get; set; }
        public ReviewDTO ReviewDTO { get; set; }
        public Aspects Aspects { get; set; }
        public DigikalaProductDTO ProductDTO { get; set; }
        public int CountReview { get; set; }
        public int CountCurrent { get; set; }
        public string AspectLabel { get; set; }
        public int ProductCount { get; set; }
        public string CommentJson { get; set; }
        public User User { get; set; }
        public string ProductName { get; set; }
        public string CommentTitle { get; set; }
        public string Review { get; set; }
        public string SelectReview { get; set; }
        public long ProductId { get; set; }
        public int UserId { get; set; }
        public string Tagger { get; set; }
        public TaggerVM()
        {
            Aspects = new Aspects();
        }
    }
    
    public class TaggerVMPost
    {
        public string idBson { get; set; }
        public string PosItem { get; set; }
        public string NatItem { get; set; }
        public string NegItem { get; set; }
        public string SelectReview { get; set; }
        public string ProductId { get; set; }
        public string Tagger { get; set; }
    }

    public class TaggerTest
    {
        public string Tagger { get; set; }
    }
    public class AspectLabel
    {
        public string Category { get; set; }
        public string Aspect { get; set; }
        public string Polarity { get; set; }
    }
    public class ResrurantLabel
    {
        public string Label { get; set; }
        public string Polarity { get; set; }
    }
    public class AddLabelParam
    {
        public string ProductId { get; set; }
        public string Tagger { get; set; }
        public string idBson { get; set; }
        public string Text { get; set; }

        public AspectLabel[] AspectLabels { get; set; }
    }

    public class AddLabelResturantParam
    {
        public string CommentId { get; set; }
        public string Tagger { get; set; }
        public string Text { get; set; }

        public ResrurantLabel[] ResrurantLabels { get; set; }
    }


    public class TaggerResturantVM
    {
        public int idBson { get; set; }
        public ResturantAspects Aspects { get; set; }
        public string AspectLabel { get; set; }
        public User User { get; set; }
        public string Review { get; set; }
        public string SelectReview { get; set; }
        public string ProductId { get; set; }
        public int UserId { get; set; }
        public string Tagger { get; set; }
        public string RestId { get; set; }
        public int CommentId { get; set; }
        public TaggerResturantVM()
        {
            Aspects = new ResturantAspects();
        }
    }

}
