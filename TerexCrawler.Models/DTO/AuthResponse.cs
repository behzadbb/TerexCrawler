using System;
using System.Collections.Generic;
using System.Text;

namespace TerexCrawler.Models.DTO
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public User User { get; set; }
    }
}
