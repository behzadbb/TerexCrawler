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
            Aspects aspects = new Aspects();
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
                        tagger.Review = s.Comments.FirstOrDefault().Review;
                    }

                    return View(tagger);
                }
            }
            return Redirect("http://google.com");
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