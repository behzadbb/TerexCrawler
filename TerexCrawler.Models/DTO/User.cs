using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public User()
        {
            this.Brand = string.Empty;
            this.Title = string.Empty;
        }
    }
}
