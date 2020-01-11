using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerexCrawler.Apps.Web.Models;
using TerexCrawler.Models.DTO;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Models.DTO.Snappfood;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.Services.Digikala;

namespace TerexCrawler.Apps.Web.Controllers
{
    public class ResturantController : Controller
    {
        public IActionResult Index(string id)
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
                User _user = users.Where(x => x.Username.ToLower() == id.ToLower().Trim()).FirstOrDefault();

                if (_user != null)
                {
                    TaggerResturantVM tagger = new TaggerResturantVM();
                    tagger.User = _user;
                    tagger.Tagger = _user.Username;

                    using (IWebsiteCrawler snappfood = new SnappfoodHelper())
                    {
                        GetFirstProductByCategoryParam param = new GetFirstProductByCategoryParam();
                        param.tagger = _user.Username;
                        param.title = _user.Title;
                        var s = snappfood.GetFirstProductByCategory<ResturantReviewsDTO>(param).Result;
                        string[] comments = s.Review.Split(". ");

                        //tagger.ProductCount = s.Review.Count();
                        //tagger.CommentJson = JsonConvert.SerializeObject(comments);
                        //tagger.CommentTitle = s.Review;
                        tagger.Review = s.Review;
                        //tagger.CountReview = s.Comments.Count();
                        tagger.CountReview = 1;
                        tagger.CountCurrent = 0;
                        tagger.ProductId = s.RestId;
                        tagger.idBson = s._id;
                        //tagger.ProductName = "";
                        //tagger.ProductDTO = s;
                    }
                    return View(tagger);
                }
            }
            return Redirect("http://google.com");
        }


        public IActionResult ResturantRejectReview(int id)
        {
            using (IWebsiteCrawler snappfood = new SnappfoodHelper())
            {
                snappfood.RejectReview(id);
                return Ok("Ok");
            }
        }
    }
}