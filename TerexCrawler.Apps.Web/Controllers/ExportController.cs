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

        struct ReviewCount
        {
            public long rid { get; set; }
            public int count { get; set; }
        }

        class Aspect
        {
            public string Name { get; set; }
            public int count { get; set; }
            public string polarity { get; set; }
        }
        class Category
        {
            public string Name { get; set; }
            public int count { get; set; }
            public string polarity { get; set; }
        }
        class AspectCategory
        {
            public string aspect { get; set; }
            public string category { get; set; }
            public int count { get; set; }
            public string polarity { get; set; }
        }
        public IActionResult DetailsReport()
        {
            List<Aspect> aspects = new List<Aspect>();
            List<Category> categories = new List<Category>();
            List<AspectCategory> aspectCategories = new List<AspectCategory>();
            using (IWebsiteCrawler snapp = new SnappfoodHelper())
            {
                //var jhkkj = digikala.GetAllReviews1();
                List<sentence> sentences = new List<sentence>();
                List<Opinion> opinions = new List<Opinion>();
                var data = snapp.GetLabelReviews();
                foreach (var rev in data)
                {
                    if (rev.sentences != null && rev.sentences.Count() > 0)
                    {
                        sentences.AddRange(rev.sentences);
                    }
                }
                sentences.ToList();
                foreach (var sentence in sentences)
                {
                    if (sentence.Opinions != null && sentence.Opinions.Count() > 0)
                    {
                        foreach (var op in sentence.Opinions)
                        {
                            var q1 = aspects.Where(x => x.Name == op.aspect && x.polarity == op.polarity).Any();
                            if (q1)
                            {
                                var s = aspects.Where(x => x.Name == op.aspect && x.polarity == op.polarity).FirstOrDefault();
                                s.count += 1;
                            }
                            else
                            {
                                aspects.Add(new Aspect { Name = op.aspect, count = 1, polarity = op.polarity });
                            }
                            var q2 = categories.Where(x => x.Name == op.category && x.polarity == op.polarity).Any();
                            if (q2)
                            {
                                var s1 = categories.Where(x => x.Name == op.category && x.polarity == op.polarity).FirstOrDefault();
                                s1.count += 1;
                            }
                            else
                            {
                                categories.Add(new Category { Name = op.category, count = 1, polarity = op.polarity });
                            }
                            var q3 = aspectCategories.Where(x => x.aspect == op.aspect && x.category == op.category && x.polarity == op.polarity).Any();
                            if (q3)
                            {
                                var s2 = aspectCategories.Where(x => x.aspect == op.aspect && x.category == op.category && x.polarity == op.polarity).FirstOrDefault();
                                s2.count += 1;
                            }
                            else
                            {
                                aspectCategories.Add(new AspectCategory { category = op.category, aspect = op.aspect, count = 1, polarity = op.polarity });
                            }
                            opinions.Add(op);
                        }
                    }
                }
                opinions.ToList();
                string aspp = "";
                foreach (var item in aspects)
                {
                    aspp += $"{item.Name}	{item.polarity}	{item.count}\n";
                }
                string cats = "";
                foreach (var item in categories)
                {
                    cats += $"{item.Name}	{item.polarity}	{item.count}\n";
                }
                string aspectCats = "";
                foreach (var item in aspectCategories)
                {
                    aspectCats += $"{item.category}	{item.aspect}	{item.polarity}	{item.count}\n";
                }

                return Ok(aspp + "\n\n\n\n\n\n" + cats + "\n\n\n\n\n\n" + aspectCats);
            }
        }
    }
}