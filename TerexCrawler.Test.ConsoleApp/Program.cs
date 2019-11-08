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
using System.Threading.Tasks;

namespace TerexCrawler.Test.ConsoleApp
{
    class Program
    {
        static MongoClient client = new MongoClient("mongodb://localhost");
        static MongoServer server => client.GetServer();
        static MongoDatabase db => server.GetDatabase("Digikala");
        private static List<DigikalaProductDTO> digikalaProducts = new List<DigikalaProductDTO>();

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
                default:
                    break;
            }

            Console.WriteLine("End");
            Console.ReadLine();
        }

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
                string url4 = "https://www.digikala.com/product/dkp-10778/";
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
            List<DigikalaPageBaseDTO> getAll = new List<DigikalaPageBaseDTO>();
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                getAll = digikala.GetAllBasePage<List<DigikalaPageBaseDTO>>();
                Console.WriteLine($"list total {getAll.Count}");
            }

            long x = 0;
            short errorCount = 0;
            foreach (var item in getAll)
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
                        Console.WriteLine($"Try Again , DKP - {dkp} Wait: {6000} Secs");
                        System.Threading.Thread.Sleep(6000 * errorCount);
                        using (IWebsiteCrawler digikala = new DigikalaHelper())
                        {
                            product = await digikala.GetProduct<DigikalaProductDTO>(item.Loc);
                            product.BaseID = item._id.ToString();
                        }
                    }
                    var _t = Math.Round((DateTime.Now - _s).TotalSeconds, 2);
                    if (product != null)
                    {
                        AddProduct(product);
                        Console.WriteLine($"{++x} = DKP-{product.DKP} , Comment={(product.Comments != null ? product.Comments.Count() + "+  " : "0  ")} ,  in {_t} Secs ");
                        if (x % 5 == 0)
                        {
                            Console.WriteLine("--------------");
                        }
                        if (x % 100 == 0)
                        {
                            System.Threading.Thread.Sleep(5000);
                            Console.Clear();
                        }
                        errorCount = 0;
                    }
                    else
                    {
                        errorCount += 1;
                        int dkp = getDKPWithUrl(item.Loc);
                        Console.WriteLine($"{++x} = DKP-{dkp} , Wait: {6000 * errorCount} Secs ,  *** Error *** ,");
                        System.Threading.Thread.Sleep(6000 * errorCount);
                    }
                }
                catch (Exception ex)
                {
                    errorCount += 1;
                    int dkp = getDKPWithUrl(item.Loc);
                    Console.WriteLine($"{++x} = DKP-{dkp} , Wait: {6000 * errorCount} Secs , *** Error ***   Problem");
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
                    System.Threading.Thread.Sleep(6000 * errorCount);
                }
            }
        }

        private async static void AddProduct(DigikalaProductDTO digikalaProduct)
        {
            digikalaProducts.Add(digikalaProduct);
            if (digikalaProducts.Count() >= 3)
            {
                using (IWebsiteCrawler digikala2 = new DigikalaHelper())
                {
                    digikala2.AddProducts(digikalaProducts);
                    digikala2.CrawledProducts(digikalaProducts.Select(x=>x.BaseID).ToList());
                    digikalaProducts.Clear();
                }
            }
        }

    }
}
