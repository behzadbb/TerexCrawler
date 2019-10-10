using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerexCrawler.Common
{
    public class HtmlHelper : System.IDisposable
    {
        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        public void GetHtml_1(string Url, int sleep = 0)
        {
            //string Page = "";
            //HtmlResponse res = new HtmlResponse();
            //res.HtmlContent = Page;
            //res.statusCode = HttpStatusCode.NoContent;
            //res.Url = Url;
            //try
            //{
            //    if (sleep > 20)
            //    {
            //        Random rnd = new Random();
            //        int slp = rnd.Next(20, 100);
            //        Thread.Sleep(slp);
            //    }
            //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        Stream receiveStream = response.GetResponseStream();
            //        StreamReader readStream = null;

            //        if (response.CharacterSet == null)
            //        {
            //            readStream = new StreamReader(receiveStream);
            //        }
            //        else
            //        {
            //            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
            //        }

            //        Page = readStream.ReadToEnd();
            //        res.statusCode = HttpStatusCode.OK;
            //        response.Close();
            //        readStream.Close();
            //        return res;
            //    }
            //    else
            //    {
            //        res.statusCode = response.StatusCode;
            //        res.HtmlContent = response.StatusDescription;
            //        return res;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    res.HtmlContent = ex.ToString();
            //    res.statusCode = HttpStatusCode.NotFound;
            //    return res;
            //}
            ////return res;
        }
        public string SubStringWeb(string Text, string Start, string End, int Between = 0, int Set = 2)
        {
            int s = Text.IndexOf(Start);
            int e = Text.IndexOf(End, s + Set) + Between;
            if (e > s)
            {
                return Text.Substring(s, e - s);
            }
            return "";
        }
        public string GetHTML(string url)
        {
            string _temp = "";
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                _temp = client.DownloadString(url);
            }
            return _temp;
            //var web = new HtmlWeb();
            //HtmlDocument doc = web.Load(url);
            //return doc.Text;
        }
        public async Task<string> GetHTML_a(String url)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var web = new HtmlWeb();
                    HtmlDocument doc = web.Load(url);
                    return doc.Text;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task<string> HttpGetAsync(string URI)
        {
            try
            {
                HttpClient hc = new HttpClient();
                Task<Stream> result = hc.GetStreamAsync(URI);

                Stream vs = await result;
                StreamReader am = new StreamReader(vs);

                return await am.ReadToEndAsync();
            }
            catch (WebException)
            {
                return "-1";
            }
        }
        public string getById(string Html, string id)
        {
            var Doc = new HtmlDocument();
            Doc.LoadHtml(Html);
            var temp = Doc.GetElementbyId(id).InnerHtml;
            return temp;
        }
        public HtmlNode getByIdNode(string Html, string id)
        {
            var Doc = new HtmlDocument();
            Doc.LoadHtml(Html);
            return Doc.GetElementbyId(id);
        }
        public IEnumerable<HtmlNode> GetElementsByName(string Html, string name)
        {
            var Doc = new HtmlDocument();
            Doc.LoadHtml(Html);
            return Doc.DocumentNode.Descendants().Where(node => node.Name == name);
        }
        public IEnumerable<HtmlNode> getByClass(string Html, string classname, string elementType = "div")
        {
            var Doc = new HtmlDocument();
            Doc.LoadHtml(Html);
            return Doc.DocumentNode.Descendants(elementType).Where(d => d.Attributes["class"].Value.Contains(classname));
        }
        public string Clean(string input)
        {
            return input.Replace("\n", " ").Replace("    ", " ").Replace("  ", " ").Replace("  ", " ").Replace("  ", " ").Trim();
        }
        public string NumberEN(string input)
        {
            return input.Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4").Replace("۵", "5").Replace("۶", "6").Replace("v", "7").Replace("۸", "8").Replace("۹", "9").Replace("۷", "9");
        }
        public string MountToNum(string input)
        {
            string m = input
                .Replace("فروردین", "/1/")
                .Replace("اردیبهشت", "/2/")
                .Replace("خرداد", "/3/")
                .Replace("تیر", "/4/")
                .Replace("مرداد", "/5/")
                .Replace("شهریور", "/6/")
                .Replace("مهر", "/7/")
                .Replace("آبان", "/8/").Replace("ابان", "/8/")
                .Replace("آذر", "/9/").Replace("اذر", "/9/")
                .Replace("دی", "/10/")
                .Replace("بهمن", "/11/")
                .Replace("اسفند", "/12/")
                .Replace(" ", "");
            m = Clean(m);
            return m;
        }
        public DateTime JalaliToMiladi(string input)
        {
            int[] _dt = new int[3];
            string[] m = input.Split('/');
            _dt[0] = int.Parse(m[0]); _dt[1] = int.Parse(m[1]); _dt[2] = int.Parse(m[2]);
            PersianCalendar pc = new PersianCalendar();
            DateTime dt = new DateTime(_dt[2], _dt[1], _dt[0], pc);
            return dt;
        }
        public string CleanText(string text)
        {
            return text.Replace("\n", " ").Replace("\t", " ").Replace("\r", " ")
                .Replace("           ", " ").Replace("   ", " ").Replace("   ", " ")
                .Replace("  ", " ").Replace("  ", " ").Replace("  ", " ")
                .Replace("  ", " ").Replace("  ", " ").Trim();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~htmlhelper() {
        //   // do not change this code. put cleanup code in dispose(bool disposing) above.
        //   dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
