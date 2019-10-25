using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Entites.Digikala
{
    public class Comment
    {
        public int Id { get; set; }
        public long? CommentId { get; set; }
        public int PageId { get; set; }
        public string Review { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime? CommentDate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Purchased { get; set; }
        public string[] NegativeAspect { get; set; }
        public string[] PositiveAspect { get; set; }
        public long? BoughtPrice { get; set; }
        public string Color { get; set; }
        public string Seller { get; set; }
        public string SellerLink { get; set; }
        public short? OpinionType { get; set; }
        public short? CommentLike { get; set; }
        public short? CommentDisLike { get; set; }
    }
}
