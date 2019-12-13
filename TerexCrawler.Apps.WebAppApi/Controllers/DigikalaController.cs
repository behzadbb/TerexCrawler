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
using TerexCrawler.Models.DTO.Brand;
using TerexCrawler.Models.DTO.Digikala;
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
    }
}