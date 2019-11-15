using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Entites.Snappfood
{
    public class Food
    {
        public string title { get; set; }
    }

    public class Comment
    {
        public int commentId { get; set; }
        public string date { get; set; }
        public string createdDate { get; set; }
        public string sender { get; set; }
        public string customerId { get; set; }
        public string commentText { get; set; }
        public int rate { get; set; }
        public string feeling { get; set; }
        public int? status { get; set; }
        public string expeditionType { get; set; }
        public List<Food> foods { get; set; }
        public List<object> replies { get; set; }
    }

    public class Data
    {
        public int count { get; set; }
        public int pageSize { get; set; }
        public List<Comment> comments { get; set; }
    }

    public class Snappfood
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public bool status { get; set; }
        public Data data { get; set; }
    }
}
