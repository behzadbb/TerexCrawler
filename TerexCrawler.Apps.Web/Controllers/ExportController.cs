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
            foreach (var item in sentences.Where(x => !string.IsNullOrEmpty(x.Text)))
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
                    foreach (var item in sentences.Where(x => !string.IsNullOrEmpty(x.Text)))
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
        class Aspect1
        {
            public string Name { get; set; }
            public int count { get; set; }
        }
        class Category
        {
            public string Name { get; set; }
            public int count { get; set; }
            public string polarity { get; set; }
        }
        class Category1
        {
            public string Name { get; set; }
            public int count { get; set; }
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

            List<Aspect1> aspects1 = new List<Aspect1>();
            List<Category1> categories1 = new List<Category1>();

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
                foreach (var sentence in sentences.Where(x => !string.IsNullOrEmpty(x.Text)))
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
                            var q12 = aspects1.Where(x => x.Name == op.aspect).Any();
                            if (q12)
                            {
                                var s12 = aspects1.Where(x => x.Name == op.aspect).FirstOrDefault();
                                s12.count += 1;
                            }
                            else
                            {
                                aspects1.Add(new Aspect1 { Name = op.aspect, count = 1 });
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
                            var q22 = categories1.Where(x => x.Name == op.category).Any();
                            if (q22)
                            {
                                var s1 = categories1.Where(x => x.Name == op.category).FirstOrDefault();
                                s1.count += 1;
                            }
                            else
                            {
                                categories1.Add(new Category1 { Name = op.category, count = 1 });
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
                string aspp1 = "";
                foreach (var item in aspects1)
                {
                    aspp1 += $"{item.Name}	{item.count}\n";
                }
                string cats1 = "";
                foreach (var item in categories1)
                {
                    cats1 += $"{item.Name}	{item.count}\n";
                }
                string aspectCats = "";
                foreach (var item in aspectCategories)
                {
                    aspectCats += $"{item.category}	{item.aspect}	{item.polarity}	{item.count}\n";
                }

                return Ok(aspp1 + "\n\n\n_____\n\n\n" + cats1 + "\n\n\n_____\n\n\n" + aspp + "\n\n\n_____\n\n\n" + cats + "\n\n\n_____\n\n\n" + aspectCats);
            }
        }

        public IActionResult DetailsReportV2(string id = null)
        {
            if (id == "digi")
            {
                using (IWebsiteCrawler snapp = new DigikalaHelper())
                {
                    //var jhkkj = digikala.GetAllReviews1();
                    List<sentence> sentences = new List<sentence>();
                    List<sentence> sentencesTrain = new List<sentence>();
                    List<sentence> sentencesTest = new List<sentence>();
                    List<sentence> sentencesTest1 = new List<sentence>();

                    var data = snapp.GetLabelReviews();
                    foreach (var rev in data)
                    {
                        if (rev.sentences != null && rev.sentences.Count() > 0)
                        {
                            sentences.AddRange(rev.sentences);
                        }
                    }
                    sentences.ToList();
                    int countTest = (int)(sentences.Count() * 0.2);
                    int countTrain = sentences.Count() - countTest;
                    sentencesTrain = sentences.Take(countTrain).ToList();
                    sentencesTest = sentences.Skip(countTrain).Take(countTest).ToList();
                    sentencesTest1 = sentencesTest.Select(x => new sentence
                    {
                        id = x.id,
                        Opinions = new List<Opinion> { x.Opinions.First() },
                        OutOfScope = x.OutOfScope,
                        Text = x.Text
                    }).ToList();

                    string result = "Result:";
                    result += "\n\n****************************************\n❌ All Data:\n\n\n" + getDetails(sentences);
                    result += "\n\n****************************************\n❌ 1.Tarin:\n\n\n" + getDetails(sentencesTrain);
                    result += "\n\n****************************************\n❌ 2.Test:\n\n\n" + getDetails(sentencesTest);
                    result += "\n\n****************************************\n❌ 3.Test First:\n\n\n" + getDetails(sentencesTest1);

                    return Ok(result);
                }
            }
            using (IWebsiteCrawler snapp = new SnappfoodHelper())
            {
                //var jhkkj = digikala.GetAllReviews1();
                List<sentence> sentences = new List<sentence>();
                List<sentence> sentencesTrain = new List<sentence>();
                List<sentence> sentencesTest = new List<sentence>();
                List<sentence> sentencesTest1 = new List<sentence>();

                var data = snapp.GetLabelReviews();
                foreach (var rev in data)
                {
                    if (rev.sentences != null && rev.sentences.Count() > 0)
                    {
                        sentences.AddRange(rev.sentences);
                    }
                }
                sentences.ToList();
                int countTest = (int)(sentences.Count() * 0.2);
                int countTrain = sentences.Count() - countTest;
                sentencesTrain = sentences.Take(countTrain).ToList();
                sentencesTest = sentences.Skip(countTrain).Take(countTest).ToList();
                sentencesTest1 = sentencesTest.Select(x => new sentence
                {
                    id = x.id,
                    Opinions = new List<Opinion> { x.Opinions.First() },
                    OutOfScope = x.OutOfScope,
                    Text = x.Text
                }).ToList();

                string result = "Result:";
                result += "\n\n****************************************\n❌ All Data:\n\n\n" + getDetails(sentences);
                result += "\n\n****************************************\n❌ 1.Tarin:\n\n\n" + getDetails(sentencesTrain);
                result += "\n\n****************************************\n❌ 2.Test:\n\n\n" + getDetails(sentencesTest);
                result += "\n\n****************************************\n❌ 3.Test First:\n\n\n" + getDetails(sentencesTest1);

                return Ok(result);
            }
        }
        public IActionResult DetailsReportV3(string id = null)
        {
            List<Aspect> aspects = new List<Aspect>();
            List<Category> categories = new List<Category>();
            List<AspectCategory> aspectCategories = new List<AspectCategory>();
            using (IWebsiteCrawler website = new DigikalaHelper())
            {
                List<sentence> sentences = new List<sentence>();
                List<Opinion> opinions = new List<Opinion>();
                var data = website.GetLabelReviews();
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

                var Cats = aspectCategories.Select(x => x.category).Distinct().ToArray();
                var Aspects = aspectCategories.Select(x => x.aspect).Distinct().ToArray();
                string[,] table = new string[Cats.Length + 1, Aspects.Length + 1];
                table[0, 0] = "X";
                for (int j = 1; j < Aspects.Length + 1; j++)
                {
                    table[0, j] = Aspects[j - 1];
                }
                for (int i = 1; i < Cats.Length + 1; i++)
                {
                    table[i, 0] = Cats[i - 1];
                }
                for (int i = 1; i < Cats.Length + 1; i++)
                {
                    for (int j = 1; j < Aspects.Length + 1; j++)
                    {
                        var catAspc = aspectCategories.Where(x => x.aspect.Contains(Aspects[j - 1]) && x.category.Contains(Cats[i - 1])).ToList();
                        var polPlus = catAspc.Count() > 0 && catAspc.Where(x => x.polarity == "positive").Any() ? catAspc.Where(x => x.polarity == "positive").FirstOrDefault().count : 0;
                        int polNet = catAspc.Count() > 0 && catAspc.Where(x => x.polarity == "neutral").Any() ? catAspc.Where(x => x.polarity == "neutral").FirstOrDefault().count : 0;
                        int polNeg = catAspc.Count() > 0 && catAspc.Where(x => x.polarity == "negative").Any() ? catAspc.Where(x => x.polarity == "negative").FirstOrDefault().count : 0;
                        if (polPlus == 0 && polNet == 0 && polNeg == 0)
                            table[i, j] = "-";
                        else
                            table[i, j] = $"{polPlus},{polNet},{polNeg}";
                    }
                }
                string tabl = "";
                for (int i = 0; i < Cats.Length + 1; i++)
                {
                    for (int j = 0; j < Aspects.Length + 1; j++)
                    {
                        tabl += table[i, j] + "\t";
                    }
                    tabl += "\n";
                }
                return Ok(tabl);
            }
        }

        private string getDetails(List<sentence> sentences)
        {
            List<Aspect> aspects = new List<Aspect>();
            List<Category> categories = new List<Category>();

            List<Aspect1> aspects1 = new List<Aspect1>();
            List<Category1> categories1 = new List<Category1>();

            List<AspectCategory> aspectCategories = new List<AspectCategory>();
            List<Opinion> opinions = new List<Opinion>();
            foreach (var sentence in sentences.Where(x => !string.IsNullOrEmpty(x.Text)))
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
                        var q12 = aspects1.Where(x => x.Name == op.aspect).Any();
                        if (q12)
                        {
                            var s12 = aspects1.Where(x => x.Name == op.aspect).FirstOrDefault();
                            s12.count += 1;
                        }
                        else
                        {
                            aspects1.Add(new Aspect1 { Name = op.aspect, count = 1 });
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
                        var q22 = categories1.Where(x => x.Name == op.category).Any();
                        if (q22)
                        {
                            var s1 = categories1.Where(x => x.Name == op.category).FirstOrDefault();
                            s1.count += 1;
                        }
                        else
                        {
                            categories1.Add(new Category1 { Name = op.category, count = 1 });
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
            foreach (var item in aspects.OrderBy(x => x.Name))
            {
                aspp += $"{item.Name}	{item.polarity}	{item.count}\n";
            }
            string cats = "";
            foreach (var item in categories.OrderBy(x => x.Name))
            {
                cats += $"{item.Name}	{item.polarity}	{item.count}\n";
            }
            string aspp1 = "";
            foreach (var item in aspects1.OrderBy(x => x.Name))
            {
                aspp1 += $"{item.Name}	{item.count}\n";
            }
            string cats1 = "";
            foreach (var item in categories1.OrderBy(x => x.Name))
            {
                cats1 += $"{item.Name}	{item.count}\n";
            }
            string aspectCats = "";
            foreach (var item in aspectCategories.OrderBy(x => x.category))
            {
                aspectCats += $"{item.category}	{item.aspect}	{item.polarity}	{item.count}\n";
            }

            return (aspp1 + "\n\n_____\n\n" + cats1 + "\n\n_____\n\n" + aspp + "\n\n_____\n\n" + cats + "\n\n_____\n\n" + aspectCats);

        }
    }
}