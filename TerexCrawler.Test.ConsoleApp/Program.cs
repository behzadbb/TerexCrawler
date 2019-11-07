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
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using TerexCrawler.Models.DTO.Page;
using Newtonsoft.Json;
using TerexCrawler.DataLayer.Context;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.ProxyServices;

namespace TerexCrawler.Test.ConsoleApp
{
    class Program
    {

        public static proxy Proxy = new proxy();
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

        static async Task Main(string[] args)
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
                var s = digikala.GetProduct<DigikalaProductDTO>(url4, string.Empty);
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
            var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");


//
////            Proxy.GetUntestedProxyListFromTxtFilePath("C:\\proxy\\1.txt");
//            var proxyList = Proxy.ListFromTxtFilePathToQueue("C:\\proxy\\1.txt");
            int everyTheard = 500;
            long counter = 0;



            async void simpleTheard()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(everyTheard - everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty  , "https://localhost:44337/api/values");
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("err");
                        }
                    }


                }

            }



            async void simpleTheard2()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            IWebsiteCrawler digikala2 = new DigikalaHelper();
                            var tryToGetData = await digikala2.GetProduct<DigikalaProductDTO>(urlAdress, string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard3()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(2 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {

                        string urlAdress = item[5].ToString();
                        try
                        {
                            IWebsiteCrawler digikala2 = new DigikalaHelper();
                            var tryToGetData = await digikala2.GetProduct<DigikalaProductDTO>(urlAdress, string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard4()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(3 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress, string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Thread.Sleep(1000);
                            Console.WriteLine("err");
                        }
                    }


                }
            }




            async void simpleTheard5()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(4 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress, string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard6()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(5 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress, string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard7()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(6 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard8()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(7 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard9()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(8 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress, string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard10()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(9 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard11()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(10 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard12()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(11 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress, string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard13()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(12 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard14()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(13 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }



            async void simpleTheard15()
            {
                var getALl = digikalaCollection.FindAll()
                    .Where(c => c[4] == false && c[5].ToString().Contains("dkp-"))
                    .Skip(14 * everyTheard)
                    .Take(everyTheard) // baraye Load Kamtar
                    .ToList();



                using (IWebsiteCrawler digikala = new DigikalaHelper())
                {

                    foreach (var item in getALl)
                    {
                        string urlAdress = item[5].ToString();
                        try
                        {
                            var tryToGetData = await digikala.GetProduct<DigikalaProductDTO>(urlAdress , string.Empty);
                            digikala.AddProduct(tryToGetData);

                            //                    var digikalaCollection = db.GetCollection<BsonDocument>("DigikalaBasePages");
                            var query = Query.EQ("_id", item[0]);
                            //                    var user = digikalaCollection.FindOne(query);
                            var set = Update.Set("Crawled", 1);
                            digikalaCollection.Update(query, set);
                            Console.WriteLine(++counter);
                        }
                        catch
                        {
                            Console.WriteLine("err");
                        }
                    }


                }
            }





            Thread thr = new Thread(new ThreadStart(simpleTheard));
            Thread thr2 = new Thread(new ThreadStart(simpleTheard2));
            Thread thr3 = new Thread(new ThreadStart(simpleTheard3));
            Thread thr4 = new Thread(new ThreadStart(simpleTheard4));
            Thread thr5 = new Thread(new ThreadStart(simpleTheard5));
            Thread thr6 = new Thread(new ThreadStart(simpleTheard6));
            Thread thr7 = new Thread(new ThreadStart(simpleTheard7));
            Thread thr8 = new Thread(new ThreadStart(simpleTheard8));
            Thread thr9 = new Thread(new ThreadStart(simpleTheard9));
            Thread thr10 = new Thread(new ThreadStart(simpleTheard10));
            Thread thr11 = new Thread(new ThreadStart(simpleTheard11));
            Thread thr12 = new Thread(new ThreadStart(simpleTheard12));
            Thread thr13 = new Thread(new ThreadStart(simpleTheard13));
            Thread thr14 = new Thread(new ThreadStart(simpleTheard14));
            Thread thr15 = new Thread(new ThreadStart(simpleTheard15));
            thr.Start();
//            thr2.Start();
//            thr3.Start();
//            thr4.Start();
//            thr5.Start();
//            thr6.Start();
//            thr7.Start();
//            thr8.Start();
//            thr9.Start();
//            thr10.Start();
//            thr11.Start();
//            thr12.Start();
//            thr13.Start();
//            thr14.Start();
//            thr15.Start();



        }
    }
}
