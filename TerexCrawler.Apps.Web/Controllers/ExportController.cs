using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerexCrawler.Models;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.Services.Digikala;
using Newtonsoft.Json;

namespace TerexCrawler.Apps.Web.Controllers
{
    public class ExportController : Controller
    {
        public IActionResult GetAllReviews(string id)
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                List<Review> reviews = digikala.GetLabelReviews();
                var json = JsonConvert.SerializeObject(reviews, Formatting.Indented);
                return File(Encoding.UTF8.GetBytes(json), "text/json", $"AllReviews-{id}-{DateTime.Now.ToShortDateString()}.json");
            }
        }
    }
}