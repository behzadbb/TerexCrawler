using System;
using TerexCrawler.Common;
using TerexCrawler.Models.DTO.XmlSitemap;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using TerexCrawler.Services.Digikala;
using TerexCrawler.Models.Interfaces;
using System.Text.Unicode;
using MongoDB.Bson;
using MongoDB.Driver;
using TerexCrawler.Models.DTO.Page;
using Newtonsoft.Json;
using TerexCrawler.DataLayer.Context;
using TerexCrawler.Models.DTO.Digikala;
using MongoDB.Driver.Builders;
using TerexCrawler.Models;
using TerexCrawler.Models.Enums;
using TerexCrawler.Services;
using System.Text;
using TerexCrawler.HttpHelper;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using TerexCrawler.Entites.Snappfood;

namespace TerexCrawler.Test.ConsoleApp
{
    class Program
    {
        public Program()
        {

        }
        private static void p(string p)
        {
            Console.WriteLine(p);
        }

        static void Main(string[] args)
        {
            p("Start !");
            p("Select Method:");
            p("1- Test Digikala SitemapToObject");
            p("2- Test Digikala Clean SitemapFile");
            p("3- Test Digikala LoadSiteMap from Directory");
            p("4- Load All Sitemap Files");
            p("5- Get Product Page");
            p("6- Get Comments");
            p("7- Add Product To DB");
            p("100- Get Site Map");
            p("101- Get Site Map From Url");
            short methodNum = Convert.ToInt16(Console.ReadLine());

            switch (methodNum)
            {
                case 1:
                    digikala_SitemapToObject("");
                    break;
                case 2:
                    digikala_SitemapClean();
                    break;
                case 3:
                    digikala_SitemapFolder();
                    break;
                case 4:
                    digikala_LoadSitemap();
                    break;
                case 5:
                    digikala_5_GetProduct();
                    break;
                case 6:
                    digikala_6_GetProductComments();
                    break;
                case 7:
                    digikala_7_AddProductToMongo();
                    break;
                case 100:
                    snappFood_100_Sitemap();
                    break;
                case 101:
                    snappFood_101_SitemapFromSite();
                    break;
                case 102:
                    snappFood_102_SitemapFromSite();
                    break;
                default:
                    break;
            }

            Console.WriteLine("End");
            Console.ReadLine();
        }

        #region Digikala
        private static int getDKPWithUrl(string url)
        {
            var indexDKP = url.LastIndexOf("dkp-");
            var lenghtDKP = url.Length - indexDKP;
            var indexEndChar = url.Substring(indexDKP).IndexOf("/");

            string getDKP = url.Substring(indexDKP, indexEndChar).Replace("dkp-", "");

            List<string> splitUrl = new List<string>();
            splitUrl.AddRange(getDKP.Split("-"));
            if (splitUrl.Any())
            {
                foreach (var item in splitUrl)
                {
                    if (System.Text.RegularExpressions.Regex.Match(item, @"^[0-9]*$").Success)
                    {
                        return int.Parse(item);

                    }
                }
            }
            return int.Parse(getDKP);
        }
        private static void digikala_SitemapToObject(string path)
        {
            //string path = @"C:\Digikala\100125842.xml";
            using (var sitemap = new SitemapHelper())
            {
                Urlset list = sitemap.SitemapToObject(path);
                var _test = list.urlset.ToList();
                Console.WriteLine("Sitemap Count: " + _test.Count());
            }
        }
        private static void digikala_SitemapClean()
        {
            string path = @"C:\Digikala\100125842.xml";
            using (var sitemap = new SitemapHelper())
            {
                //sitemap.CleanFile(path);
            }
        }
        private static void digikala_SitemapFolder()
        {
            string directoryPath = @"C:\Digikala\xml";

            DirectoryInfo directorySelected = new DirectoryInfo(directoryPath);
            string file1 = string.Empty;
            FileInfo[] fileInfos = directorySelected.GetFiles();
            foreach (FileInfo file in fileInfos)
            {
                using (var sitemap = new SitemapHelper())
                {
                    sitemap.CleanFile(file);
                }
            }
        }
        private static void digikala_LoadSitemap()
        {
            string dirPath = @"C:\Digikala\xml\clean";
            DirectoryInfo directorySelected = new DirectoryInfo(dirPath);
            string file1 = string.Empty;
            FileInfo[] fileInfos = directorySelected.GetFiles("*.xml");
            List<B5_Url> dkps = new List<B5_Url>();
            for (int i = 0; i < fileInfos.Length; i++)
            {
                using (var sitemap = new SitemapHelper())
                {
                    dkps.AddRange(sitemap.SitemapToObject(fileInfos[i].ToString()).urlset.ToList());
                }
            }
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                digikala.AddBasePages(dkps);
            }
            int sss = 5;
        }
        private async static void digikala_5_GetProduct()
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                string url1 = "https://www.digikala.com/product/dkp-313420";
                string url2 = "https://www.digikala.com/product/dkp-1675555";
                string url3 = "https://www.digikala.com/product/dkp-676525";
                string url4 = "https://www.digikala.com/product/dkp-781586";
                //var page = digikala.GetPage(url2);
                var s = await digikala.GetProduct<DigikalaProductDTO>(url4);
                var jjj = JsonConvert.SerializeObject(s);
            }
        }
        private static void digikala_6_GetProductComments()
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                string url1 = "https://www.digikala.com/product/dkp-313420";
                string url2 = "https://www.digikala.com/product/dkp-6/";
                //var cm = digikala.GetComments(url2);
                //var s = digikala.GetProduct<DigikalaProductDTO>(page, url2);
                //var jjj = JsonConvert.SerializeObject(s);
            }
        }
        private async static void digikala_7_AddProductToMongo()
        {
            for (short i = 0; i < 120; i++)
            {
                List<DigikalaPageBaseDTO> getAll = new List<DigikalaPageBaseDTO>();
                List<BasePage> pages = new List<BasePage>();
                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {
                    getAll = (await digikala.GetAllBasePage<GetAllBasePageDigikalaResult>()).BasePages;
                    Console.WriteLine($"list total {getAll.Count()}");
                    pages = getAll.Select(x => new BasePage
                    {
                        Id = x._id,
                        Loc = x.Loc
                    }).ToList();
                    getAll.Clear();
                }

                long x = 0;
                short errorCount = 0;
                foreach (var item in pages)
                {
                    try
                    {
                        var _s = DateTime.Now;
                        DigikalaProductDTO product = null;
                        using (IWebsiteCrawler digikala = new DigikalaHelper())
                        {
                            product = await digikala.GetProduct<DigikalaProductDTO>(item.Loc);
                        }

                        if (product == null)
                        {
                            int dkp = getDKPWithUrl(item.Loc);
                            Console.WriteLine($"Try Again , DKP - {dkp} Wait: {1000} Secs");
                            System.Threading.Thread.Sleep(1000 * errorCount);
                            using (IWebsiteCrawler digikala = new DigikalaHelper())
                            {
                                product = await digikala.GetProduct<DigikalaProductDTO>(item.Loc);
                            }
                        }
                        var _t = Math.Round((DateTime.Now - _s).TotalSeconds, 2);
                        if (product != null)
                        {
                            ProductTemp prd = new ProductTemp();
                            prd.BasePage = item;
                            prd.DigikalaProduct = product;
                            digikala_SaveProductBatch(prd);
                            Console.WriteLine($"S{i},   {++x}  =  DKP-{product.DKP}    , Comment={(product.Comments != null ? product.Comments.Count() + "+  " : "0  ")} ,  in {_t} Secs ");
                            if (x % 5 == 0)
                            {
                                Console.WriteLine("--------------");
                                System.Threading.Thread.Sleep(100);
                            }
                            if (x % 100 == 0)
                            {
                                System.Threading.Thread.Sleep(500);
                                Console.Clear();
                            }
                            errorCount = 0;
                        }
                        else
                        {
                            if (errorCount < 3)
                            {
                                errorCount += 1;
                            }
                            int dkp = getDKPWithUrl(item.Loc);
                            Console.WriteLine($"{++x} = DKP-{dkp} , Wait: {1000 * errorCount} Secs ,  *** Error *** ,");
                            System.Threading.Thread.Sleep(1000 * errorCount);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (errorCount < 3)
                        {
                            errorCount += 1;
                        }
                        int dkp = getDKPWithUrl(item.Loc);
                        Console.WriteLine($"{++x} = DKP-{dkp} , Wait: {1000 * errorCount} Secs , *** Error ***   Problem");
                        using (ILoger Logger = new MongoDBLoggerHelper())
                        {
                            LogDTO log = new LogDTO()
                            {
                                _id = ObjectId.GenerateNewId().ToString(),
                                DateTime = DateTime.Now,
                                Description = ex.Message.ToString(),
                                ProjectId = (int)ProjectNames.Console,
                                Url = item.Loc,
                                MethodName = "Digikala - Console App",
                                Title = "Get Product Error - " + dkp
                            };
                            Logger.AddLog(log);
                        }
                        System.Threading.Thread.Sleep(1000 * errorCount);
                    }
                }
            }
            digikala_SaveProductBatch(null, true);
        }

        private static void digikala_SaveProductBatch(ProductTemp? productTemp = null, bool force = false)
        {
            if (productTemp.HasValue)
            {
                productTemps.Add(productTemp.Value);
            }

            if (productTemps.Count() >= 20 || force)
            {
                List<ProductTemp> temp = new List<ProductTemp>();
                temp.AddRange(productTemps);
                productTemps.Clear();
                using (IWebsiteCrawler digikala2 = new DigikalaHelper())
                {
                    AddProductsDigikala addProducts = new AddProductsDigikala { digikalaProducts = temp.Select(x => x.DigikalaProduct).ToList() };
                    digikala2.AddProducts(addProducts);
                    digikala2.CrawledProducts(temp.Select(x => x.BasePage.Id).ToArray());
                    Console.WriteLine($"{temp.Count()} Add To Database ");
                }
            }
        }
        struct BasePage
        {
            public string Loc { get; set; }
            public string Id { get; set; }
        }
        struct ProductTemp
        {
            public BasePage BasePage { get; set; }
            public DigikalaProductDTO DigikalaProduct { get; set; }
        }
        private static List<ProductTemp> productTemps = new List<ProductTemp>();
        #endregion

        #region
        private static void snappFood_100_Sitemap()
        {
            string path = @"C:\Snappfood\sitemap.xml";
            string[] readText = File.ReadAllLines(path);
            string[] readTexts = readText.Where(x => x.Contains("/restaurant/menu/")).Select(x => x.Replace("\t\t<loc>", "").Replace("</loc>", "")).ToArray();

            string path1 = @"C:\Snappfood\resturant.txt";

            if (!File.Exists(path1))
            {
                File.WriteAllLines(path1, readTexts, Encoding.UTF8);
            }
            Console.WriteLine(readTexts.Count());
        }
        private static void snappFood_101_SitemapFromSite()
        {
            string[] user_agent = {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362"
            };
            string sitemapUrl = @"https://snappfood.ir/sitemap.xml";
            using (IHttpClientHelper client = new HttpClientHelper())
            {
                var cli = client.GetHttp(sitemapUrl, true, user_agent);
                string[] lines = cli.Content.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                    );

                string[] readTexts = lines.Where(x => x.Contains("/restaurant/menu/")).Select(x => x.Replace("\t\t<loc>", "").Replace("</loc>", "")).ToArray();

                string path1 = @"C:\Snappfood\resturant.txt";

                if (!File.Exists(path1))
                {
                    File.WriteAllLines(path1, readTexts, Encoding.UTF8);
                }
                Console.WriteLine(readTexts.Count());
            }
        }
        private static void snappFood_102_SitemapFromSite()
        {
            string[] user_agent = {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362"
            };
            string sitemapUrl = @"https://snappfood.ir/sitemap.xml";
            List<string> readTexts = new List<string>();
            using (IHttpClientHelper client = new HttpClientHelper())
            {
                var cli = client.GetHttp(sitemapUrl, true, user_agent);
                string[] lines = cli.Content.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                    );

                readTexts.AddRange(lines.Where(x => x.Contains("/restaurant/menu/")).Select(x => x.Replace("\t\t<loc>", "").Replace("</loc>", "")).ToList());
            }
            if (readTexts.Any())
            {
                foreach (string item in readTexts)
                {
                    Snappfood snappfood = new Snappfood();
                    using (SnappfoodHelper snappfoodHelper = new SnappfoodHelper())
                    {
                        snappfood = snappfoodHelper.GetProduct<Snappfood>(item).Result;
                        if (snappfood != null)
                        {
                            snappfoodHelper.AddProduct(snappfood);
                        }
                    }
                }
            }
            int temp = 0;
        }
        private static string getSnappfoodCommentLink(string id, int pageNumber = 0)
        {
            return $"https://snappfood.ir/restaurant/comment/vendor/{id}/{pageNumber}";
        }
        private static string getSnappfoodIdByUrl(string url)
        {
            //string url = "https://snappfood.ir/restaurant/menu/31n4w0/%27%2Bitem.link%2B%27";
            int start = url.IndexOf("menu/") + 5;
            int lenght = url.Length - start;
            url = url.Substring(start, lenght);
            int end = url.IndexOf("/");
            return url.Substring(0, end);
        }
        #endregion
    }
}