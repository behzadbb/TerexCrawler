using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models
{
    public struct LogDTO
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string MethodName { get; set; }
        public string Url { get; set; }
    }
}
