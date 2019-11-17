using HtmlAgilityPack;
using System;
using TerexCrawler.HttpHelper;
using TerexCrawler.Models.Interfaces;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TerexCrawler.Test.ConsoleTerexB2B
{
    class Program
    {
        static string[] user_agent = {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.120 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:69.0) Gecko/20100101 Firefox/69.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362"
            };
        static void Main(string[] args)
        {
            Console.WriteLine("    Terexware   ");
            Console.WriteLine("Select Methods:");
            Console.WriteLine("1_ Get Companies Information");
            Console.WriteLine("2_ Get Company Details");
            Console.Write("Select: ");
            short selectMethod = Convert.ToInt16(Console.ReadLine());
            switch (selectMethod)
            {
                case 1:
                    getCompanies();
                    break;
                case 2:
                    getCompanyDetails("https://www.itpnews.com/companies/details/1/15056");
                    break;
                default:
                    break;
            }
        }
        static void getCompanies()
        {
            const string itpComapniesLink = "https://www.itpnews.com/companies";
            List<CompanyCat> companyCats = new List<CompanyCat>();
            using (IHttpClientHelper client = new HttpClientHelper())
            {
                var cli = client.GetHttp(itpComapniesLink, true, user_agent);
                if (cli.Success)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(cli.Content);
                    companyCats = doc.DocumentNode.SelectNodes("//body//div[@class='bodyss']//div//div//div//div//ul//li//a").Select(x => new CompanyCat { Link = x.Attributes["href"].Value, Name = x.InnerHtml }).ToList();
                    //string lists21 = string.Join("\n", lists.Select(x => $"Cat: {x.Name}  -  Link: {x.Link}").ToArray());
                    //string jsong = JsonConvert.SerializeObject(lists);
                    //int aaa = 0;
                }
            }
            using (IHttpClientHelper client = new HttpClientHelper())
            {
                if (companyCats != null && companyCats.Any())
                {
                    foreach (var cat in companyCats)
                    {
                        var cli = client.GetHttp(cat.Link, true, user_agent);
                        if (cli.Success)
                        {
                            var doc = new HtmlDocument();
                            doc.LoadHtml(cli.Content);
                            var aTags = doc.DocumentNode.SelectNodes("*//a[@class='btn btn-success']");
                            List<string> links = aTags.Select(x => x.Attributes["href"].Value).Where(x => x != string.Empty && x.Contains("/companies/details")).Distinct().ToList();
                            foreach (var item in links)
                            {
                                var companyDeatils = client.GetHttp(item, true, user_agent);
                            }

                            //string lists21 = string.Join("\n", lists.Select(x => $"Cat: {x.Name}  -  Link: {x.Link}").ToArray());
                            //string jsong = JsonConvert.SerializeObject(lists);
                            //int aaa = 0;
                        }
                    }
                }
            }

        }
        static Company getCompanyDetails(string url)
        {
            Company company = new Company();

            using (IHttpClientHelper client = new HttpClientHelper())
            {
                var cli = client.GetHttp(url, true, user_agent);
                if (cli.Success)
                {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(cli.Content);

                    var ss = doc.DocumentNode.SelectSingleNode("//body//div[@class='bodyss']" +
                        "//div[@class='bodydiv']" +
                        "//div//div//div//ul[@class='list-group']");
                }
            }

            return company;
        }
        public struct CompanyCat
        {
            public string Name { get; set; }
            public string Link { get; set; }
        }
        public struct Company
        {
            public string Name { get; set; }
            public string Manager { get; set; }
            public string Phones { get; set; }
            public string Address { get; set; }
            public string Emails { get; set; }
            public string Website { get; set; }
        }
    }
}
