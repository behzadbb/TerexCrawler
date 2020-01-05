using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.DTO.Snappfood;
using System.Linq;

namespace TerexCrawler.Entites.Snappfood
{
    public class SnappReview
    {
        public int CommentId { get; set; }
        public string Date { get; set; }
        public string CreatedDate { get; set; }
        public string Sender { get; set; }
        public string CustomerId { get; set; }
        public string CommentText { get; set; }
        public int Rate { get; set; }
        public string Feeling { get; set; }
        public int? Status { get; set; }
        public string ExpeditionType { get; set; }
        public List<string> Foods { get; set; }
        public SnappReview(Comment cm)
        {
            CommentId = cm.commentId;
            Date = cm.date;
            CreatedDate = cm.createdDate;
            Sender = cm.sender;
            CustomerId = cm.customerId;
            CommentText = cm.commentText;
            Rate = cm.rate;
            Feeling = cm.feeling;
            Status = cm.status;
            ExpeditionType = cm.expeditionType;
            Foods = new List<string>();
            if (cm.foods != null && cm.foods.Count > 0)
            {
                foreach (var item in cm.foods)
                {
                    Foods.Add(item.title);
                }
            }
        }
    }

    public class Snappfood
    {
        public ObjectId _id { get; set; }
        public string Url { get; set; }
        public bool status { get; set; }
        public bool isTagged { get; set; }
        public bool Reserve { get; set; }
        public string Tagger { get; set; }
        public DateTime TagDate { get; set; }
        public int count { get; set; }
        public int pageSize { get; set; }
        public List<SnappReview> Reviews { get; set; }
        public DateTime CreateDateTime { get; set; }

        public Snappfood(SnappfoodDTO dto)
        {
            this._id = ObjectId.GenerateNewId(DateTime.Now);
            this.Url = dto.Url;
            this.Tagger = dto.Tagger;
            this.TagDate = dto.TagDate;
            this.CreateDateTime = dto.CreateDateTime;
            this.isTagged = dto.isTagged;
            this.status = dto.status;
            this.Reserve = dto.Reserve;
            this.pageSize = dto.data.pageSize;
            this.count = dto.data.count;
            this.Reviews = new List<SnappReview>();
            if (dto.data.comments != null && dto.data.comments.Count() > 0)
            {
                foreach (var item in dto.data.comments.Where(r => !string.IsNullOrEmpty(r.commentText)))
                {
                    Reviews.Add(new SnappReview(item));
                }
            }
        }
    }
}
