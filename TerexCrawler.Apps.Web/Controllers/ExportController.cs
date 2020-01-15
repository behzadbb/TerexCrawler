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

        public IActionResult TopSentences(string id)
        {
            id = id.ToLower();

            IWebsiteCrawler web = null;

            if (id == "mobile")
            {
                web = new DigikalaHelper();
            }
            else if (id == "resturant")
            {
                web = new SnappfoodHelper();
            }
            else
            {
                return Ok();
            }

            var sentences = web.GetTopSentences();
            string _sentences = "";
            foreach (var item in sentences)
            {
                foreach (var op in item.Opinions)
                {
                    _sentences += $@"{item.Text.Replace(",", " ").Replace("  ", " ")}	{op.category}_{op.aspect}	{op.polarity}" + "\r\n";
                }
            }
            return File(Encoding.UTF8.GetBytes(_sentences), "text/csv", "TopSentences.csv");
        }
        public IActionResult TopMobile()
        {
            IWebsiteCrawler web = new DigikalaHelper();
            var sentences = web.GetTopSentences();
            string _sentences = "";
            foreach (var item in sentences)
            {
                foreach (var op in item.Opinions)
                {
                    _sentences += $@"{item.Text.Replace(",", " ").Replace("  ", " ")}	{op.category}_{op.aspect}	{op.polarity}" + "\r\n";
                }
            }
            return File(Encoding.UTF8.GetBytes(_sentences), "text/csv", "TopSentences.csv");
        }
        public IActionResult TopResturant()
        {
            try
            {
                using (IWebsiteCrawler crawler = new SnappfoodHelper())
                {
                    var sentences = crawler.GetTopSentences();
                    if (sentences == null || sentences.Count < 1)
                    {
                        return Ok("sentences null");
                    }
                    string _sentences = "";
                    foreach (var item in sentences)
                    {
                        if (item.Opinions != null && item.Opinions.Count > 0)
                        {
                            foreach (var op in item.Opinions)
                            {
                                _sentences += $@"{item.id}	{item.Text.Replace(",", " ").Replace("  ", " ")}	{op.category}_{op.aspect}	{op.polarity}" + "\r\n";
                            }
                        }
                    }
                    return File(Encoding.UTF8.GetBytes(_sentences), "text/csv", "ResturantTopSentences.csv");
                }
            }
            catch (Exception ex)
            {
                return Ok("Error:\n" + ex.Message + "\n\n\nTime: " + DateTime.Now);
            }

        }
    }
}