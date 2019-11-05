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

namespace TerexCrawler.Test.ConsoleApp
{
    class Program
    {
        static MongoClient client = new MongoClient("mongodb://localhost");
        static MongoServer server => client.GetServer();
        static MongoDatabase db => server.GetDatabase("Digikala");

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
        private static void digikala_5_GetProduct()
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                string url1 = "https://www.digikala.com/product/dkp-313420";
                string url2 = "https://www.digikala.com/product/dkp-1675555";
                string url3 = "https://www.digikala.com/product/dkp-676525";
                string url4 = "https://www.digikala.com/product/dkp-6/";
                //var page = digikala.GetPage(url2);
                var s = digikala.GetProduct<DigikalaProductDTO>(url4);
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
            

            
            MongoCollection<BsonDocument> digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
            var getALl = digikalaCollection.FindAll()
                .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                .Take(10) // baraye Load Kamtar
                .ToList();
            Console.WriteLine($"list total {getALl.Count}");

            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                long x = 0;
                foreach (var item in getALl)
                {
                    string urlAdress = item[5].ToString();
//                    try
//                    { 
                       var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress);
                       digikala.AddProduct(tryToGetData);
                       Console.WriteLine(++x);
//                    }
//                    catch 
//                    {
//                      
//                    }
                }
//                string url1 = "https://www.digikala.com/product/dkp-313420";
//                string url2 = "https://www.digikala.com/product/dkp-1675555";
                //var page = digikala.GetPage(url2);
//                var s = digikala.GetProduct<DigikalaProductDTO>(url1);
                //var jjj = JsonConvert.SerializeObject(s);
                
            }
        }
    }
}
