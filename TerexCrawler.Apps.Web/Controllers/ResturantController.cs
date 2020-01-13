using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TerexCrawler.Apps.Web.Models;
using TerexCrawler.Common;
using TerexCrawler.Models;
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

                        tagger.Review = s.Review;
                        tagger.idBson = s._id;
                        tagger.CommentId = s._id;
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

        [HttpPost]
        public IActionResult AddLabel([FromBody]AddLabelResturantParam model)
        {
            Dictionary<string, int> pol = new Dictionary<string, int>() {
                { "positive", 1 },{ "neutral", 0 },{ "negative", -1}
            };
            if (model != null)
            {
                AddReviewToDBParam param = new AddReviewToDBParam();
                param.id = model.CommentId.ToString();
                param.tagger = model.Tagger;
                ReviewDTO review = new ReviewDTO();
                review._id = "000";
                review.CreateDate = DateTime.Now;
                review.ProductID = int.Parse(model.CommentId);
                review.rid = int.Parse(model.CommentId);

                sentence sentence = new sentence();
                sentence.Text = model.Text;

                List<Opinion> Opinions = new List<Opinion>();
                if (model.ResrurantLabels.Count() > 0)
                {
                    Opinions.AddRange(model.ResrurantLabels.Select(x => new Opinion
                    {
                        category = x.Label.Split('#')[0].ToUpper().Trim(),
                        aspect = x.Label.Split('#')[1].ToUpper().Trim(),
                        polarity = x.Polarity,
                        polarityClass = pol[x.Polarity]
                    }).ToList());
                    sentence.Opinions = Opinions;
                }
                if (Opinions.Count < 1)
                {
                    sentence.OutOfScope = true;
                }
                review.sentences = new List<sentence>();
                review.sentences.Add(sentence);

                param.review = new ReviewDTO();
                param.review = review;
                using (IWebsiteCrawler digikala = new SnappfoodHelper())
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


        public IActionResult init()
        {
            List<SnappfoodMinInfo> reviews = new List<SnappfoodMinInfo>();
            List<string> cleanReviews = new List<string>();
            List<ResturantReviewsDTO> resturantReviews = new List<ResturantReviewsDTO>();
            using (IWebsiteCrawler snapp = new SnappfoodHelper())
            {
                reviews = snapp.GetAllReviews<GetReviewsMinimumResponse>().Result.ReviewsMinimum;
            }
            using (var html = new HtmlHelper())
            {
                cleanReviews.Clear();
                for (int i = 0; i < reviews.Count; i++)
                {
                    if (!string.IsNullOrEmpty(reviews[i].Review))
                    {
                        string _txt = html.CleanReview(reviews[i].Review);
                        if (!string.IsNullOrEmpty(_txt))
                        {
                            ResturantReviewsDTO rr = new ResturantReviewsDTO()
                            {
                                _id = i,
                                CommentId = reviews[i].CommentId,
                                RestId = reviews[i].RestId,
                                Review = _txt,
                                Date = DateTime.Now,
                                Reserve = false,
                                Seen = false,
                                Tagged = false,
                                Tagger = "_",
                                TagDate = DateTime.Now.AddDays(-60),
                                ReserveDate = DateTime.Now.AddDays(-60),
                                Reject = false,
                            };
                            resturantReviews.Add(rr);
                        }
                    }
                    if (resturantReviews.Count>30000)
                    {
                        Insert(resturantReviews);
                        resturantReviews.Clear();
                    }
                }
            }
            Insert(resturantReviews);
            return Ok(resturantReviews.Count.ToString());
        }
        private void Insert(List<ResturantReviewsDTO> resturantReviews)
        {
            using (IWebsiteCrawler snapp = new SnappfoodHelper())
            {
                AddResturatsDBParam addResturats = new AddResturatsDBParam();
                addResturats.resturantReviews = resturantReviews;
                snapp.AddRawReviewsToDB(addResturats);
                System.Threading.Thread.Sleep(1500);
            }
        }
    }
}