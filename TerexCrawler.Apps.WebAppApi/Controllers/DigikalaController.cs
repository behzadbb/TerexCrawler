using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TerexCrawler.Models.Const;
using TerexCrawler.Models.DTO;
using TerexCrawler.Models.DTO.Api;
using TerexCrawler.Models.DTO.Brand;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Models.Enums;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.Services.Digikala;

namespace TerexCrawler.Apps.WebAppApi.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class DigikalaController : ControllerBase
    {
        [HttpGet("{action}")]
        public string testConnection()
        {
            return "ok - " + DateTime.Now;
        }

        [HttpPost("{action}")]
        public AddReviewToDBResponse AddReviewtodb([FromBody]AddReviewToDBParam param)
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                var result = digikala.AddReviewToDB(param);
                return new AddReviewToDBResponse { Success = result };
            }
        }

        [HttpPost("{action}")]
        public DigikalaProductDTO GetFirstProductByCategory(GetFirstProductByCategoryParam param)
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                return digikala.GetFirstProductByCategory<DigikalaProductDTO>(param).Result;
            }
        }

        [HttpPost("{action}")]
        public AuthResponse Auth(UserDTO user)
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

            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {
                User _user = users.Where(x =>
                                        x.Username.ToLower() == user.Username.ToLower().Trim() &&
                                        x.Password.ToLower() == user.Password.ToLower().Trim()
                                        ).FirstOrDefault();
                if (user != null)
                {
                    AuthResponse res = new AuthResponse { Success = true, User = _user };
                    return res;
                }
            }
            return new AuthResponse { Success = false, User = null };
        }

        [HttpPost("{action}")]
        public GetAspectsResponseDTO GetAspects(GetAspectsDTO param)
        {
            if (param.AspectType == (int)AspectTypes.Mobile)
                return new GetAspectsResponseDTO { Aspects = Aspects.mobile };
            else if (param.AspectType == (int)AspectTypes.Laptop)
                return new GetAspectsResponseDTO { Aspects = Aspects.laptop };
            return new GetAspectsResponseDTO { Aspects = Aspects.mobile };
        }
    }
}