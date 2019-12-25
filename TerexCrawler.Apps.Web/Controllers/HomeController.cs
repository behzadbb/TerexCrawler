﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TerexCrawler.Apps.Web.Models;
using TerexCrawler.Models;
using TerexCrawler.Models.DTO;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.Services.Digikala;
using Newtonsoft.Json;
using TerexCrawler.Models.Const;
using TerexCrawler.Apps.ReviewTaggerWPF;

namespace TerexCrawler.Apps.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Tagger(string id)
        {
            //Aspects aspects = new Aspects();
            List<User> users = new List<User>();
            users.Add(new User { Username = "devila", Password = "germany", Category = "گوشی موبایل" });
            users.Add(new User { Username = "NavidSharifi", Password = "navid", Category = "گوشی موبایل" });
            users.Add(new User { Username = "Behzad", Password = "behzad", Category = "گوشی موبایل", Role = "admin" });
            users.Add(new User { Username = "Hamshagerdi", Password = "mehrteam", Category = "گوشی موبایل", Brand = "اپل" });
            users.Add(new User { Username = "Setare", Password = "setare", Category = "گوشی موبایل" });
            users.Add(new User { Username = "ftm", Password = "ftm", Category = "گوشی موبایل" });
            users.Add(new User { Username = "user1", Password = "user1", Category = "گوشی موبایل" });
            users.Add(new User { Username = "jamali", Password = "jamali", Category = "گوشی موبایل" });
            users.Add(new User { Username = "dr.faili", Password = "faili_ut", Category = "گوشی موبایل" });

            if (!string.IsNullOrEmpty(id))
            {
                User _user = users.Where(x =>
                                        x.Username.ToLower() == id.ToLower().Trim()
                                        ).FirstOrDefault();

                if (_user != null)
                {
                    TaggerVM tagger = new TaggerVM();
                    tagger.User = _user;
                    tagger.Tagger = _user.Username;

                    using (IWebsiteCrawler digikala = new DigikalaHelper())
                    {
                        GetFirstProductByCategoryParam param = new GetFirstProductByCategoryParam();
                        param.Brand = _user.Brand;
                        param.category = _user.Category;
                        param.tagger = _user.Username;
                        param.title = _user.Title;
                        var s = digikala.GetFirstProductByCategory<DigikalaProductDTO>(param).Result;
                        tagger.ProductCount = s.Comments.Count();
                        tagger.CommentJson = JsonConvert.SerializeObject(s.Comments);
                        tagger.CommentTitle = s.Comments.FirstOrDefault().Title;
                        tagger.CountReview = s.Comments.Count();
                        tagger.CountCurrent = 1;
                        tagger.Review = s.Comments.FirstOrDefault().Review;
                        tagger.ProductId = s.DKP;
                        tagger.idBson = s._id;
                        //tagger.ProductDTO = s;
                    }
                    return View(tagger);
                }
            }
            return Redirect("http://google.com");
        }

        [HttpPost]
        public IActionResult AddLabel([FromBody]TaggerVMPost model)
        {
            if (model != null)
            {
                //var model = JsonConvert.DeserializeObject<TaggerVMPost>(id);
                AddReviewToDBParam param = new AddReviewToDBParam();
                param.id = model.idBson.ToString();
                param.tagger = model.Tagger;
                ReviewDTO review = new ReviewDTO();
                review._id = "000";
                review.CreateDate = DateTime.Now;
                review.ProductID = int.Parse(model.ProductId);
                review.rid = int.Parse(model.ProductId);

                sentence sentence = new sentence();
                sentence.Text = model.SelectReview;

                List<Opinion> Opinions = new List<Opinion>();
                if (!string.IsNullOrEmpty(model.PosItem))
                {
                    var pos = getAspects(model.PosItem);
                    Opinions.AddRange(pos.Select(x => new Opinion { category = x[0], categoryClass = x[1], polarity = "pos", polarityClass = 1 }).ToList());
                }
                if (!string.IsNullOrEmpty(model.NatItem))
                {
                    var pos = getAspects(model.NatItem);
                    Opinions.AddRange(pos.Select(x => new Opinion { category = x[0], categoryClass = x[1], polarity = "nat", polarityClass = 0 }).ToList());
                }
                if (!string.IsNullOrEmpty(model.NegItem))
                {
                    var pos = getAspects(model.NegItem);
                    Opinions.AddRange(pos.Select(x => new Opinion { category = x[0], categoryClass = x[1], polarity = "neg", polarityClass = -1 }).ToList());
                }
                if (string.IsNullOrEmpty(model.PosItem) && string.IsNullOrEmpty(model.NatItem) && string.IsNullOrEmpty(model.NegItem))
                {
                    sentence.OutOfScope = true;
                }
                sentence.Opinions = Opinions;
                review.sentences = new List<sentence>();
                review.sentences.Add(sentence);

                param.review = new ReviewDTO();
                param.review = review;
                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {
                    var result = digikala.AddReviewToDB_NewMethod(param);
                    if (!result)
                    {
                        return NoContent();
                    }
                }
                return Json(new { Message = "ثبت شد" });
            }
            else
            {
                return NoContent();
            }
        }

        private List<string[]> getAspects(string aspect)
        {
            List<string[]> result = new List<string[]>();
            Aspects aspects = new Aspects();
            if (aspect.Contains("*"))
            {
                string[] labels = aspect.Split('*');

                foreach (var item in labels)
                {
                    var s = item.Split('#');
                    string[] ss = { AspectsAir.TitleToCategory[s[0]], AspectsAir.TitleToAspect[s[1]] };
                    result.Add(ss);
                }
                return result;
            }
            var sss = aspect.Split('#');
            string[] ssss = { AspectsAir.TitleToCategory[sss[0]], AspectsAir.TitleToAspect[sss[1]] };
            result.Add(ssss);
            return result;
        }

        public IActionResult Privacy(string id)
        {
            List<User> users = new List<User>();
            users.Add(new User { Username = "devila", Password = "germany", Category = "گوشی موبایل" });
            users.Add(new User { Username = "NavidSharifi", Password = "navid", Category = "گوشی موبایل" });
            users.Add(new User { Username = "Behzad", Password = "behzad", Category = "گوشی موبایل", Role = "admin" });
            users.Add(new User { Username = "Hamshagerdi", Password = "mehrteam", Category = "گوشی موبایل", Brand = "اپل" });
            users.Add(new User { Username = "Setare", Password = "setare", Category = "گوشی موبایل" });
            users.Add(new User { Username = "ftm", Password = "ftm", Category = "گوشی موبایل" });
            users.Add(new User { Username = "user1", Password = "user1", Category = "گوشی موبایل" });
            users.Add(new User { Username = "jamali", Password = "jamali", Category = "گوشی موبایل" });
            users.Add(new User { Username = "dr.faili", Password = "faili_ut", Category = "گوشی موبایل" });

            if (!string.IsNullOrEmpty(id))
            {
                User _user = users.Where(x =>
                                        x.Username.ToLower() == id.ToLower().Trim()
                                        ).FirstOrDefault();

                if (_user != null)
                {
                    TaggerVM tagger = new TaggerVM();
                    tagger.User = _user;
                    tagger.Tagger = _user.Username;

                    using (IWebsiteCrawler digikala = new DigikalaHelper())
                    {
                        GetFirstProductByCategoryParam param = new GetFirstProductByCategoryParam();
                        param.Brand = _user.Brand;
                        param.category = _user.Category;
                        param.tagger = _user.Username;
                        //param.title = _user.Title;
                        var s = digikala.GetFirstProductByCategory<DigikalaProductDTO>(param).Result;
                        tagger.ProductCount = s.Comments.Count();
                        tagger.CommentJson = JsonConvert.SerializeObject(s.Comments);
                        tagger.CommentTitle = s.Comments.FirstOrDefault().Title;
                    }

                    return View(tagger);
                }
            }
            return Redirect("http://google.com");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult test()
        {
            return View();
        }
    }
}
