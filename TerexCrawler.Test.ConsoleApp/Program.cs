using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TerexCrawler.Common;
using TerexCrawler.Entites.Digikala;
using TerexCrawler.Entites.Snappfood;
using TerexCrawler.HttpHelper;
using TerexCrawler.Models;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Models.DTO.Page;
using TerexCrawler.Models.DTO.Snappfood;
using TerexCrawler.Models.DTO.XmlSitemap;
using TerexCrawler.Models.Enums;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.Services;
using TerexCrawler.Services.Digikala;

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
            p("9- get All Reviews");
            p("10- get All Reviews to xml");
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
                case 9:
                    digikala_getAllReviews_9();
                    break;
                case 10:
                    digikala_getAllReviewsXML_10();
                    break;
                case 11:
                    digikala_11_GetAllProduct();
                    break;
                case 12:
                    digikala_12_LoadLabelReview();
                    break;
                case 13:
                    digikala_13_GetStatus();
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
                case 103:
                    snappFood_103_GetAllReviews();
                    break;
                case 104:
                    snappFood_104_GetFirstReview();
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
        struct Reviews
        {
            public List<string> reviews { get; set; }
        }
        private static List<ProductTemp> productTemps = new List<ProductTemp>();

        private async static void digikala_getAllReviews_9()
        {
            List<string> reviews = new List<string>();
            List<string> cleanReviews = new List<string>();
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                bool allReviews = true;
                if (true)
                {
                    var digikalaReviews = digikala.GetAllReviews<string[]>();
                    reviews = digikalaReviews.Result.ToList();
                }
                else
                {
                    var digikalaReviews = digikala.GetAllReviews<string[]>();
                    reviews = digikalaReviews.Result.ToList();
                    int spliteSize = 50000;
                    float size = reviews.Count / spliteSize;
                    int itr = (int)Math.Round(size);
                    for (int i = 0; i <= itr; i++)
                    {
                        string json1 = JsonConvert.SerializeObject(new Reviews { reviews = reviews.Skip(i * spliteSize).Take(spliteSize).ToList() });
                        File.WriteAllText(@$"C:\Digikala\reviews\review-{i + 1}.json", json1, new UTF8Encoding(false));
                    }
                }
            }
            using (var html = new HtmlHelper())
            {
                cleanReviews.Clear();
                foreach (var item in reviews)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        string _txt = html.CleanReview(item);
                        if (!string.IsNullOrEmpty(_txt))
                        {
                            cleanReviews.Add(_txt);
                        }
                    }
                }
            }
            string json = JsonConvert.SerializeObject(new Reviews { reviews = cleanReviews.Distinct().ToList() });

            File.WriteAllText(@$"s:\Digikala\reviews\review-all.json", json, new UTF8Encoding(false));
        }
        private async static void digikala_getAllReviewsXML_10()
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                var mm = digikala.GetAllReviews1();
            }
        }
        private async static void digikala_11_GetAllProduct()
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                List<DigikalaProduct> mm = digikala.GetAllReviewObjects<List<DigikalaProduct>>("گوشی موبایل").Result;
                //var ss = mm.Where(x => x.Category.Contains("")).Count();
                var sss = mm.Where(x => x.Comments != null).Count();
                var ssss = mm.Where(x => x.Comments != null);

            }
        }
        private async static void digikala_12_LoadLabelReview()
        {
            string old = @"S:\digikala\label\AllReviews-OldLabel-1_10_2020.json";
            string last = @"S:\digikala\label\AllReviews-LastLabel-1_10_2020.json";

            string oldText = File.ReadAllText(old);
            string lastText = File.ReadAllText(last);

            var oldReviews = JsonConvert.DeserializeObject<List<ReviewDTO>>(oldText);
            var lastReviews = JsonConvert.DeserializeObject<List<ReviewDTO>>(lastText);

            List<ReviewDTO> mixReviews = new List<ReviewDTO>();
            List<ReviewDTO> _mixReviews = new List<ReviewDTO>();
            mixReviews.AddRange(oldReviews);
            mixReviews.AddRange(lastReviews);

            int countBefore = mixReviews.Count();

            var _result = mixReviews.GroupBy(x => x.ProductID).Select(z => new ReviewCount { rid = (long)z.Key, count = z.Count() }).ToList();
            List<long> listDupPid = _result.Where(x => x.count > 1).Select(x => x.rid).ToList();
            var duplicate = oldReviews.Where(x => listDupPid.Contains(x.ProductID)).ToList();
            var _oldReviews = oldReviews.Except(duplicate).ToList();

            mixReviews.Clear();
            mixReviews.AddRange(_oldReviews);
            mixReviews.AddRange(lastReviews);

            int op_bf = 0;
            int op_af = 0;
            var _result1 = mixReviews.GroupBy(x => x.ProductID).Select(z => new ReviewCount { rid = (long)z.Key, count = z.Count() }).ToList();
            foreach (var rev in mixReviews)
            {
                ReviewDTO _review = new ReviewDTO();
                _review.sentences = new List<sentence>();
                _review.ProductID = rev.ProductID;
                _review.CreateDate = rev.CreateDate;
                _review.rid = rev.rid;
                _review._id = rev._id;
                List<sentence> sentences = new List<sentence>();
                List<Opinion> _lastOpinion1 = new List<Opinion>();
                List<Opinion> _lastOpinion2 = new List<Opinion>();
                foreach (var _sent in rev.sentences)
                {
                    if (_sent.Opinions != null && _sent.Opinions.Count() > 0)
                    {
                        _lastOpinion2.Clear();
                        _lastOpinion2 = _sent.Opinions.ToList();
                    }
                    else
                    {
                        _lastOpinion2.Clear();
                    }
                    if (_lastOpinion1.Count() > 0 && _sent.Opinions != null && _sent.Opinions.Count() > _lastOpinion1.Count())
                    {
                        bool isDuplicate = true;
                        for (int i = 0; i < _lastOpinion1.Count; i++)
                        {
                            if (_lastOpinion1[i].category == _sent.Opinions[i].category &&
                                _lastOpinion1[i].aspect == _sent.Opinions[i].aspect &&
                                _lastOpinion1[i].polarity == _sent.Opinions[i].polarity)
                            {
                                isDuplicate = true;
                            }
                            else
                            {
                                isDuplicate = false;
                            }
                        }
                        if (isDuplicate)
                        {
                            var _sent_temp = _sent.Opinions.Skip(_lastOpinion1.Count()).ToList();
                            _sent.Opinions.Clear();
                            _sent.Opinions.AddRange(_sent_temp.ToList());
                        }
                    }

                    sentence sent = new sentence();
                    sent.Opinions = new List<Opinion>();
                    sent.id = _sent.id;
                    sent.Text = _sent.Text;
                    if (_sent.Opinions != null)
                    {
                        foreach (var op in _sent.Opinions)
                        {
                            if (!sent.Opinions.Where(x => x.category == op.category && x.aspect == op.aspect).Any())
                            {
                                sent.Opinions.Add(op);
                            }
                            else
                            {
                                op_bf += 1;
                                Console.WriteLine("_|_");
                            }
                        }

                        sent.OutOfScope = false;
                    }
                    else
                    {
                        sent.OutOfScope = true;
                    }
                    sentences.Add(sent);
                    sentences.ToList();
                    _lastOpinion1.Clear();
                    _lastOpinion1.AddRange(_lastOpinion2.ToList());
                    _lastOpinion2.Clear();
                }
                _review.sentences.AddRange(sentences);
                _mixReviews.Add(_review);
            }


            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                foreach (var item in _mixReviews)
                {
                    AddReviewToDBParam param = new AddReviewToDBParam();
                    param.AutoOff = false;
                    param.review = item;
                    var result = digikala.AddReviewToDB_NewMethod(param);
                }
            }
        }

        struct ReviewCount
        {
            public long rid { get; set; }
            public int count { get; set; }
        }

        class Aspect
        {
            public string Name { get; set; }
            public int count { get; set; }
            public string polarity { get; set; }
        }
        class Category
        {
            public string Name { get; set; }
            public int count { get; set; }
            public string polarity { get; set; }
        }
        class AspectCategory
        {
            public string aspect { get; set; }
            public string category { get; set; }
            public int count { get; set; }
            public string polarity { get; set; }
        }
        private async static void digikala_13_GetStatus()
        {
            List<Aspect> aspects = new List<Aspect>();
            List<Category> categories = new List<Category>();
            List<AspectCategory> aspectCategories = new List<AspectCategory>();
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {

                //var jhkkj = digikala.GetAllReviews1();
                List<sentence> sentences = new List<sentence>();
                List<Opinion> opinions = new List<Opinion>();
                var data = digikala.GetLabelReviews();
                foreach (var rev in data)
                {
                    if (rev.sentences != null && rev.sentences.Count() > 0)
                    {
                        sentences.AddRange(rev.sentences);
                    }
                }
                sentences.ToList();
                foreach (var sentence in sentences)
                {
                    if (sentence.Opinions != null && sentence.Opinions.Count() > 0)
                    {
                        foreach (var op in sentence.Opinions)
                        {
                            var q1 = aspects.Where(x => x.Name == op.aspect && x.polarity == op.polarity).Any();
                            if (q1)
                            {
                                var s=aspects.Where(x => x.Name == op.aspect && x.polarity == op.polarity).FirstOrDefault();
                                s.count += 1;
                            }
                            else
                            {
                                aspects.Add(new Aspect { Name = op.aspect, count = 1, polarity = op.polarity });
                            }
                            var q2 = categories.Where(x => x.Name == op.category && x.polarity == op.polarity).Any();
                            if (q2)
                            {
                                var s1=categories.Where(x => x.Name == op.category && x.polarity == op.polarity).FirstOrDefault();
                                s1.count += 1;
                            }
                            else
                            {
                                categories.Add(new Category { Name = op.category, count = 1, polarity = op.polarity });
                            }
                            var q3 = aspectCategories.Where(x => x.aspect == op.aspect && x.category == op.category && x.polarity == op.polarity).Any();
                            if (q3)
                            {
                                var s2= aspectCategories.Where(x => x.aspect == op.aspect && x.category == op.category && x.polarity == op.polarity).FirstOrDefault();
                                s2.count += 1;
                            }
                            else
                            {
                                aspectCategories.Add(new AspectCategory { category = op.category, aspect = op.aspect, count = 1, polarity = op.polarity });
                            }
                            opinions.Add(op);
                        }
                    }
                }
                opinions.ToList();
                string aspp = "";
                foreach (var item in aspects)
                {
                    aspp += $"{item.Name}	{item.polarity}	{item.count}\n";
                }
                string cats = "";
                foreach (var item in categories)
                {
                    cats += $"{item.Name}	{item.polarity}	{item.count}\n";
                }
                string aspectCats = "";
                foreach (var item in aspectCategories)
                {
                    aspectCats += $"{item.category}	{item.aspect}	{item.polarity}	{item.count}\n";
                }
                Console.WriteLine(digikala.GetSatatusReview());
            }
        }
        #endregion

        #region Snappfood - Resturant
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
                    SnappfoodDTO dto = new SnappfoodDTO();
                    using (SnappfoodHelper snappfoodHelper = new SnappfoodHelper())
                    {
                        dto = snappfoodHelper.GetProduct<SnappfoodDTO>(item).Result;
                        if (dto != null)
                        {
                            snappfoodHelper.AddProduct(new Snappfood(dto));
                            System.Threading.Thread.Sleep(200);
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

        private static void snappFood_103_GetAllReviews()
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
                            //cleanReviews.Add(_txt);
                        }
                    }
                }
            }

            //string json = JsonConvert.SerializeObject(new Reviews { reviews = cleanReviews.Distinct().ToList() });

            //File.WriteAllText(@$"s:\Digikala\reviews\review-all-snapp.json", json, new UTF8Encoding(false));
            //json = "";
            using (IWebsiteCrawler snapp = new SnappfoodHelper())
            {
                AddResturatsDBParam addResturats = new AddResturatsDBParam();
                addResturats.resturantReviews = resturantReviews;
                snapp.AddRawReviewsToDB(addResturats);
            }
        }

        private static void snappFood_104_GetFirstReview()
        {
            using (IWebsiteCrawler snapp = new SnappfoodHelper())
            {
                GetFirstProductByCategoryParam param = new GetFirstProductByCategoryParam();

                param.tagger = "behzad";
                var reviews = snapp.GetFirstProductByCategory<ResturantReviewsDTO>(param).Result;
            }
        }
        #endregion
    }
}