using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO
{
    public struct HttpResultResponseDTO
    {
        public string Content { get; set; }
        public bool Success { get; set; }
        public int ErrorCode { get; set; }
        public int HttpStatusCode { get; set; }
        public string ExeptionErrorMessage { get; set; }
    }
}
