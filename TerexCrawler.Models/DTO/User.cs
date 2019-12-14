using System;
using System.Collections.Generic;
using System.Text;
using TerexCrawler.Models.Enums;

namespace TerexCrawler.Models.DTO
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Category { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Role { get; set; }
        public int AspectType { get; set; }
        public User()
        {
            this.Brand = string.Empty;
            this.Title = string.Empty;
            this.Role = "user";
            this.AspectType = (int)AspectTypes.Mobile;
        }
    }
    public class UserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
