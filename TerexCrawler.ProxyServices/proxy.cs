using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace TerexCrawler.ProxyServices
{
    public class proxy
    {
        public Queue ListFromTxtFilePathToQueue(string path)
        {
            Queue list = new Queue();
            string TestedProxy = File.ReadAllText(path);
            foreach (var item in TestedProxy.Split('\n'))
            {

                list.Enqueue(item.Replace("\r" , ""));

            }

            return list;
        }
        public void GetUntestedProxyListFromTxtFilePath(string path)
        {
            string unTestedProxyWithoutSplit = File.ReadAllText(path);
            string[] unTestedProxyList = unTestedProxyWithoutSplit.Split('\n');
            Console.WriteLine($"proxy count is {unTestedProxyList.Count()}");
            WebClient wc = new WebClient();
            List<string> testedProxyList = new List<string>();
            int x = 0;
            foreach (var item in unTestedProxyList)
            {
                var data = item.Split('\r')[0].Split('\t');
                
                string page = "";
              
                Console.WriteLine(x++);
                try
                {
                    string proxy = $"{data[0]}:{data[1]}";
                    wc.Proxy = new WebProxy(proxy);
                    page = wc.DownloadString("https://api.ipify.org?format=json");
                    testedProxyList.Add(proxy);
                }
                catch (Exception)
                {
                    page = "Disconnect";
                }
            }

            System.IO.File.WriteAllLines(path, testedProxyList);
            Console.WriteLine($"tested proxy count is {testedProxyList.Count()}");

        }
        public void GetUntestedProxyListFromTxtFilePath2(string path)
        {
            string unTestedProxyWithoutSplit = File.ReadAllText(path);
            string[] unTestedProxyList = unTestedProxyWithoutSplit.Split('\n');
            Console.WriteLine($"proxy count is {unTestedProxyList.Count()}");
            WebClient wc = new WebClient();
            

            List<string> testedProxyList = new List<string>();
            int x = 0;
            foreach (var item in unTestedProxyList)
            {
                var data = item.Replace("\r" , "");

                string page = "";

                Console.WriteLine(x++);
                try
                {
                    string proxy = $"{data}";
                    wc.Proxy = new WebProxy(proxy);
                    page = wc.DownloadString("https://api.ipify.org?format=json");
                    testedProxyList.Add(proxy);
                }
                catch (Exception)
                {
                    Console.WriteLine("err");

                    page = "Disconnect";
                }
            }

            System.IO.File.WriteAllLines(path, testedProxyList);
            Console.WriteLine($"tested proxy count is {testedProxyList.Count()}");

        }


    }
}
