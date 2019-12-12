using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public string addBase()
        {
            return "Salam";
        }

        [HttpPost("{action}")]
        public bool AddReview(AddReviewToDBParam param)
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                return digikala.AddReviewToDB(param);
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