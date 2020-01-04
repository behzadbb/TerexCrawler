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

        public string CleanReview(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            string _txt = NumberEN(text);
            _txt = _txt
                            .Replace("ي", "ی").Replace("ك", "ک").Replace(" ... ", " … ").Replace("باسلام","با سلام ").Replace("سلام.","سلام. ")
                            .Replace("ااااااا", "ا").Replace("اااااا", "ا").Replace("ااااا", "ا").Replace("اااا", "ا").Replace("ااا", "ا")
                            .Replace("دددد", "د").Replace("ددد", "د")
                            .Replace("ییی", "ی").Replace("ییی", "ی")
                            .Replace("هههه", "ه").Replace("ههه", "ه")
                            .Replace(",,,,,,", ",").Replace(",,,,,", ",").Replace(",,,,", ",").Replace(",,,", ",")
                            .Replace("میشود.", "میشود. ").Replace("کردم.", "کردم. ").Replace("خریدم.", "خریدم. ").Replace("شد.", "شد. ").Replace("شد؟", "شد؟ ")
                            .Replace("بود.", "بود. ").Replace("بود؟", "بود؟ ").Replace("میتونم.", "میتونم. ").Replace("میتونم؟", "میتونم؟ ")
                            .Replace("خوبیه.", "خوبیه. ").Replace("خوبیه؟", "خوبیه؟ ").Replace("خوبه.", "خوبه. ").Replace("خوبه؟", "خوبه؟ ")
                            .Replace("خوب.", "خوب. ").Replace("خوب؟", "خوب؟ ").Replace("خب.", "خب. ").Replace("خب؟", "خب؟ ")
                            .Replace("افتاده.", "افتاده. ").Replace("افتاده؟", "افتاده؟ ").Replace("افتاد.", "افتاد. ").Replace("افتاد؟", "افتاد؟ ")
                            .Replace("خورد.", "خورد. ").Replace("خورد؟", "خورد؟ ")
                            .Replace("نکنید.", "نکنید. ")
                            .Replace("باشه.", "باشه. ").Replace("باشه؟", "باشه؟ ")
                            .Replace("باشد.", "باشد. ").Replace("باشد؟", "باشد؟ ")
                            .Replace("باشیم.", "باشیم. ").Replace("باشیم؟", "باشیم؟ ")
                            .Replace("رسیده.", "رسیده. ").Replace("رسیده؟", "رسیده؟ ")
                            .Replace("رسید.", "رسید. ").Replace("رسید؟", "رسید؟ ")
                            .Replace("گفت.", "گفت. ").Replace("گفت؟", "گفت؟ ")
                            .Replace("گفتند.", "گفتند. ").Replace("گفتند؟", "گفتند؟ ")
                            .Replace("هست.", "هست. ").Replace("هست؟", "هست؟ ")
                            .Replace("هستش.", "هستش. ").Replace("هستش؟", "هستش؟ ")
                            .Replace("هستن.", "هستن. ").Replace("هستن؟", "هستن؟ ")
                            .Replace("هستند.", "هستند. ").Replace("هستند؟", "هستند؟ ")
                            .Replace("است.", "است. ").Replace("است؟", "است؟ ")
                            .Replace("میخوره.", "میخوره. ").Replace("میخوره؟", "میخوره؟ ")
                            .Replace("کاش.", "کاش. ").Replace("کاش؟", "کاش؟ ").Replace("گشت.", "گشت. ").Replace("گشت؟", "گشت؟ ")
                            .Replace("داشت.", "داشت. ").Replace("داشت؟", "داشت؟ ").Replace("ساخت.", "ساخت. ").Replace("ساخت؟", "ساخت؟ ")
                            .Replace("کند.", "کند. ").Replace("کنند.", "کنند. ").Replace("کنن.", "کنن. ").Replace("کن.", "کن. ").Replace("خرید.", "خرید. ")
                            .Replace("می شود.", "می شود. ").Replace("میشه.", "میشه. ").Replace("می شه.", "میشه. ").Replace("واقعا؟", "واقعا؟ ")
                            .Replace("داد.", "داد. ").Replace("داد؟", "داد. ").Replace("بشه.", "بشه. ").Replace("نمیشه.", "نمیشه. ")
                            .Replace("نیست.", "نیست. ").Replace("نیس.", "نیس. ").Replace("اس.", "اس. ").Replace("نشه.", "نشه. ").Replace("نشه؟", "نشه؟ ")
                            .Replace("عااااااااالیه", "عالیه").Replace("عاااااااالیه", "عالیه").Replace("عاااااااالیه", "عالیه").Replace("عااااااالیه", "عالیه").Replace("عااااالیه", "عالیه")
                            .Replace("عالیههههههههه", "عالیه").Replace("عالیهههههههه", "عالیه").Replace("عالیههههههه", "عالیه").Replace("عالیهههههه", "عالیه")
                            .Replace("عالیههههه", "عالیه").Replace("عالیهههه", "عالیه").Replace("عالیههه", "عالیه").Replace("عالیهه", "عالیه").Replace("عالیهه", "عالیه")
                            .Replace("عالیه.", "عالیه. ").Replace("داره.", "داره. ").Replace("داره؟", "داره؟ ").Replace("دارد.", "داره. ").Replace("دارد؟", "دارد؟ ")
                            .Replace("نیست.", "نیست. ").Replace("نیس.", "نیس. ").Replace("اس.", "اس. ").Replace("نشه.", "نشه. ").Replace("نشه؟", "نشه؟ ")
                            .Replace("   ", ". ").Replace("    ", ". ").Replace("     ", ". ")
                            .Replace("،", " ، ").Replace("  ", " ").Replace("     ", ". ")
                            .Replace("   ", ". ").Replace("    ", ". ").Replace("     ", ". ")
                            .Replace("!!!!", ". ").Replace("!!!", ". ").Replace("!!", "!").Replace("!!", "!")
                            .Replace("!!", "!").Replace("!!", "! ").Replace("! ! ", "! ").Replace("! ", ". ")
                            .Replace("????", ". ").Replace("???", ". ").Replace("??", ". ").Replace("?", ". ")
                            .Replace("؟؟؟؟", ". ").Replace("؟؟؟", ". ").Replace("؟؟", ". ").Replace("؟", ". ")
                            .Replace("\r\n\r\n\r\n", ". ").Replace("\r\n\r\n", ". ")
                            .Replace("!?\r\n", ". ").Replace("!?\n\r", ". ").Replace("!?\n", ". ").Replace("!<br>", ". ")
                            .Replace("!\n\r", ". ").Replace("!\r\n", ". ").Replace("!\n", ". ").Replace("!<br>", ". ")
                                                .Replace("?\n\r", ". ").Replace("?\r\n", ". ").Replace("?\n", ". ").Replace("?<br>", ". ")
                                                .Replace(" \n\r", ". ").Replace(" \r\n", ". ").Replace(" \n", ". ").Replace(" <br>", ". ")
                                                .Replace("\n\r", ". ").Replace("\r\n", ". ").Replace("\n", ". ").Replace("<br>", ". ")
                                                .Replace("<br />", ". ").Replace("<br/>", ". ").Replace(".   ", ". ")
                                                .Replace("  . . ", ". ").Replace(". . ", ". ").Replace(". . ", ". ")
                                                .Replace("&nbsp;", " ").Replace("•", "  ")
                                                .Replace("..... ", ". ").Replace(".... ", ". ").Replace(".. ", ". ").Replace(" . ", ". ")
                                                .Replace("دیجی کالا", "دیجیکالا").Replace("دیجیکلا ", "دیجیکالا").Replace("دجی کالا", "دیجیکالا").Replace("دیج کالا", "دیجیکالا")
                                                .Replace("  ", " ").Replace("  ", " ").Replace("  ", " ")
                                                .Replace(" ، ، ", " ، ").Replace("،،", " ، ").Replace("  ", " ").Replace("  ", " ")
                                                .Replace(" ، ، ", " ، ").Replace("،،", " ، ").Replace("  ", " ").Replace("  ", " ")
                                                .Trim();
            _txt = CleanDate(_txt);
            _txt = CleanPercent(_txt);
            _txt = CleanEmoji(_txt);
            _txt = CleanOS(_txt);
            _txt = CleanUnit(_txt);

            _txt = _txt.Replace("  ", " ").Replace("  ", " ");

            if (!string.IsNullOrEmpty(_txt) && _txt.Substring(0, 1) == ".")
            {
                _txt = _txt.Substring(1, _txt.Length - 1);
            }

            return _txt;
        }

        private string CleanPercent(string text)
        {
            return text.Replace("0درصد", "0 درصد").Replace("1درصد", "1 درصد").Replace("2درصد", "2 درصد")
                .Replace("3درصد", "3 درصد").Replace("4درصد", "4 درصد").Replace("5درصد", "5 درصد")
                .Replace("6درصد", "6 درصد").Replace("7درصد", "7 درصد").Replace("8درصد", "8 درصد").Replace("9درصد", "9 درصد");
        }
        private string CleanOS(string text)
        {
            return text
                .Replace("ورژن0", "ورژن 0")
                .Replace("ورژن1", "ورژن 1")
                .Replace("ورژن2", "ورژن 2")
                .Replace("ورژن3", "ورژن 3")
                .Replace("ورژن4", "ورژن 4")
                .Replace("ورژن5", "ورژن 5")
                .Replace("ورژن6", "ورژن 6")
                .Replace("ورژن7", "ورژن 7")
                .Replace("ورژن8", "ورژن 8")
                .Replace("ورژن9", "ورژن 9")
                .Replace("نسخه0", "نسخه 0")
                .Replace("نسخه1", "نسخه 1")
                .Replace("نسخه2", "نسخه 2")
                .Replace("نسخه3", "نسخه 3")
                .Replace("نسخه4", "نسخه 4")
                .Replace("نسخه5", "نسخه 5")
                .Replace("نسخه6", "نسخه 6")
                .Replace("نسخه7", "نسخه 7")
                .Replace("نسخه8", "نسخه 8")
                .Replace("نسخه9", "نسخه 9")
                .Replace("اندروید0", "اندروید 0")
                .Replace("اندروید1", "اندروید 1")
                .Replace("اندروید2", "اندروید 2")
                .Replace("اندروید3", "اندروید 3")
                .Replace("اندروید4", "اندروید 4")
                .Replace("اندروید5", "اندروید 5")
                .Replace("اندروید6", "اندروید 6")
                .Replace("اندروید7", "اندروید 7")
                .Replace("اندروید8", "اندروید 8")
                .Replace("اندروید9", "اندروید 9")
                .Replace("ویندوز0", "ویندوز 0")
                .Replace("ویندوز1", "ویندوز 1")
                .Replace("ویندوز2", "ویندوز 2")
                .Replace("ویندوز3", "ویندوز 3")
                .Replace("ویندوز4", "ویندوز 4")
                .Replace("ویندوز5", "ویندوز 5")
                .Replace("ویندوز6", "ویندوز 6")
                .Replace("ویندوز7", "ویندوز 7")
                .Replace("ویندوز8", "ویندوز 8")
                .Replace("ویندوز9", "ویندوز 9")
                .Replace("windows0", "windows 0")
                .Replace("windows1", "windows 1")
                .Replace("windows2", "windows 2")
                .Replace("windows3", "windows 3")
                .Replace("windows4", "windows 4")
                .Replace("windows5", "windows 5")
                .Replace("windows6", "windows 6")
                .Replace("windows7", "windows 7")
                .Replace("windows8", "windows 8")
                .Replace("windows9", "windows 9")
                .Replace("android0", "android 0")
                .Replace("android1", "android 1")
                .Replace("android2", "android 2")
                .Replace("android3", "android 3")
                .Replace("android4", "android 4")
                .Replace("android5", "android 5")
                .Replace("android6", "android 6")
                .Replace("android7", "android 7")
                .Replace("android8", "android 8")
                .Replace("android9", "android 9")
                .Replace("ios0", "ios 0")
                .Replace("ios1", "ios 1")
                .Replace("ios2", "ios 2")
                .Replace("ios3", "ios 3")
                .Replace("ios4", "ios 4")
                .Replace("ios5", "ios 5")
                .Replace("ios6", "ios 6")
                .Replace("ios7", "ios 7")
                .Replace("ios8", "ios 8")
                .Replace("ios9", "ios 9")
                .Replace("آی او اس0", "آی او اس 0")
                .Replace("آی او اس1", "آی او اس 1")
                .Replace("آی او اس2", "آی او اس 2")
                .Replace("آی او اس3", "آی او اس 3")
                .Replace("آی او اس4", "آی او اس 4")
                .Replace("آی او اس5", "آی او اس 5")
                .Replace("آی او اس6", "آی او اس 6")
                .Replace("آی او اس7", "آی او اس 7")
                .Replace("آی او اس8", "آی او اس 8")
                .Replace("آی او اس9", "آی او اس 9")
                .Replace("ver0", "ver 0")
                .Replace("ver1", "ver 1")
                .Replace("ver2", "ver 2")
                .Replace("ver3", "ver 3")
                .Replace("ver4", "ver 4")
                .Replace("ver5", "ver 5")
                .Replace("ver6", "ver 6")
                .Replace("ver7", "ver 7")
                .Replace("ver8", "ver 8")
                .Replace("ver9", "ver 9")
                .Replace("آیفون0", "آیفون 0")
                .Replace("آیفون1", "آیفون 1")
                .Replace("آیفون2", "آیفون 2")
                .Replace("آیفون3", "آیفون 3")
                .Replace("آیفون4", "آیفون 4")
                .Replace("آیفون5", "آیفون 5")
                .Replace("آیفون6", "آیفون 6")
                .Replace("آیفون7", "آیفون 7")
                .Replace("آیفون8", "آیفون 8")
                .Replace("آیفون9", "آیفون 9")
                .Replace("مدل0", "مدل 0")
                .Replace("مدل1", "مدل 1")
                .Replace("مدل2", "مدل 2")
                .Replace("مدل3", "مدل 3")
                .Replace("مدل4", "مدل 4")
                .Replace("مدل5", "مدل 5")
                .Replace("مدل6", "مدل 6")
                .Replace("مدل7", "مدل 7")
                .Replace("مدل8", "مدل 8")
                .Replace("مدل9", "مدل 9");
        }

        private string CleanDate(string text)
        {
            return text
                .Replace("0سال", "0 سال").Replace("1سال", "1 سال").Replace("2سال", "2 سال")
                .Replace("3سال", "3 سال").Replace("4سال", "4 سال").Replace("5سال", "5 سال")
                .Replace("6سال", "6 سال").Replace("7سال", "7 سال").Replace("8سال", "8 سال")
                .Replace("9سال", "9 سال")

                .Replace("0ماه", "0 ماه").Replace("1ماه", "1 ماه").Replace("2ماه", "2 ماه")
               .Replace("3ماه", "3 ماه").Replace("4ماه", "4 ماه").Replace("5ماه", "5 ماه")
               .Replace("6ماه", "6 ماه").Replace("7ماه", "7 ماه").Replace("8ماه", "8 ماه")
               .Replace("9ماه", "9 ماه")

               .Replace("0هفته", "0 هفته").Replace("1هفته", "1 هفته").Replace("2هفته", "2 هفته")
               .Replace("3هفته", "3 هفته").Replace("4هفته", "4 هفته").Replace("5هفته", "5 هفته")
               .Replace("6هفته", "6 هفته").Replace("7هفته", "7 هفته").Replace("8هفته", "8 هفته")
               .Replace("9هفته", "9 هفته")

               .Replace("0روز", "0 روز").Replace("1روز", "1 روز").Replace("2روز", "2 روز")
                .Replace("3روز", "3 روز").Replace("4روز", "4 روز").Replace("5روز", "5 روز")
                .Replace("6روز", "6 روز").Replace("7روز", "7 روز").Replace("8روز", "8 روز")
                .Replace("9روز", "9 روز");
        }

        private string CleanUnit(string text)
        {
            return text
                .Replace("0آمپر", "0 آمپر")
                .Replace("1آمپر", "1 آمپر")
                .Replace("2آمپر", "2 آمپر")
                .Replace("3آمپر", "3 آمپر")
                .Replace("4آمپر", "4 آمپر")
                .Replace("5آمپر", "5 آمپر")
                .Replace("6آمپر", "6 آمپر")
                .Replace("7آمپر", "7 آمپر")
                .Replace("8آمپر", "8 آمپر")
                .Replace("9آمپر", "9 آمپر")
                .Replace("0نوع", "0 نوع")
                .Replace("1نوع", "1 نوع")
                .Replace("2نوع", "2 نوع")
                .Replace("3نوع", "3 نوع")
                .Replace("4نوع", "4 نوع")
                .Replace("5نوع", "5 نوع")
                .Replace("6نوع", "6 نوع")
                .Replace("7نوع", "7 نوع")
                .Replace("8نوع", "8 نوع")
                .Replace("9نوع", "9 نوع")
                .Replace("0اینچ", "0 اینچ")
                .Replace("1اینچ", "1 اینچ")
                .Replace("2اینچ", "2 اینچ")
                .Replace("3اینچ", "3 اینچ")
                .Replace("4اینچ", "4 اینچ")
                .Replace("5اینچ", "5 اینچ")
                .Replace("6اینچ", "6 اینچ")
                .Replace("7اینچ", "7 اینچ")
                .Replace("8اینچ", "8 اینچ")
                .Replace("9اینچ", "9 اینچ")
                .Replace("0سانت", "0 سانت")
                .Replace("1سانت", "1 سانت")
                .Replace("2سانت", "2 سانت")
                .Replace("3سانت", "3 سانت")
                .Replace("4سانت", "4 سانت")
                .Replace("5سانت", "5 سانت")
                .Replace("6سانت", "6 سانت")
                .Replace("7سانت", "7 سانت")
                .Replace("8سانت", "8 سانت")
                .Replace("9سانت", "9 سانت")
                .Replace("0مدل", "0 مدل")
                .Replace("1مدل", "1 مدل")
                .Replace("2مدل", "2 مدل")
                .Replace("3مدل", "3 مدل")
                .Replace("4مدل", "4 مدل")
                .Replace("5مدل", "5 مدل")
                .Replace("6مدل", "6 مدل")
                .Replace("7مدل", "7 مدل")
                .Replace("8مدل", "8 مدل")
                .Replace("9مدل", "9 مدل")
                .Replace("0صفحه", "0 صفحه")
                .Replace("1صفحه", "1 صفحه")
                .Replace("2صفحه", "2 صفحه")
                .Replace("3صفحه", "3 صفحه")
                .Replace("4صفحه", "4 صفحه")
                .Replace("5صفحه", "5 صفحه")
                .Replace("6صفحه", "6 صفحه")
                .Replace("7صفحه", "7 صفحه")
                .Replace("8صفحه", "8 صفحه")
                .Replace("9صفحه", "9 صفحه")
                .Replace("0درجه", "0 درجه")
                .Replace("1درجه", "1 درجه")
                .Replace("2درجه", "2 درجه")
                .Replace("3درجه", "3 درجه")
                .Replace("4درجه", "4 درجه")
                .Replace("5درجه", "5 درجه")
                .Replace("6درجه", "6 درجه")
                .Replace("7درجه", "7 درجه")
                .Replace("8درجه", "8 درجه")
                .Replace("9درجه", "9 درجه")
                .Replace("0گرم", "0 گرم")
                .Replace("1گرم", "1 گرم")
                .Replace("2گرم", "2 گرم")
                .Replace("3گرم", "3 گرم")
                .Replace("4گرم", "4 گرم")
                .Replace("5گرم", "5 گرم")
                .Replace("6گرم", "6 گرم")
                .Replace("7گرم", "7 گرم")
                .Replace("8گرم", "8 گرم")
                .Replace("9گرم", "9 گرم")
                .Replace("0میلی آمپر", "0 میلی آمپر")
                .Replace("1میلی آمپر", "1 میلی آمپر")
                .Replace("2میلی آمپر", "2 میلی آمپر")
                .Replace("3میلی آمپر", "3 میلی آمپر")
                .Replace("4میلی آمپر", "4 میلی آمپر")
                .Replace("5میلی آمپر", "5 میلی آمپر")
                .Replace("6میلی آمپر", "6 میلی آمپر")
                .Replace("7میلی آمپر", "7 میلی آمپر")
                .Replace("8میلی آمپر", "8 میلی آمپر")
                .Replace("9میلی آمپر", "9 میلی آمپر")
                .Replace("0امپر", "0 امپر")
                .Replace("1امپر", "1 امپر")
                .Replace("2امپر", "2 امپر")
                .Replace("3امپر", "3 امپر")
                .Replace("4امپر", "4 امپر")
                .Replace("5امپر", "5 امپر")
                .Replace("6امپر", "6 امپر")
                .Replace("7امپر", "7 امپر")
                .Replace("8امپر", "8 امپر")
                .Replace("9امپر", "9 امپر")
                .Replace("0میلی امپر", "0 میلی امپر")
                .Replace("1میلی امپر", "1 میلی امپر")
                .Replace("2میلی امپر", "2 میلی امپر")
                .Replace("3میلی امپر", "3 میلی امپر")
                .Replace("4میلی امپر", "4 میلی امپر")
                .Replace("5میلی امپر", "5 میلی امپر")
                .Replace("6میلی امپر", "6 میلی امپر")
                .Replace("7میلی امپر", "7 میلی امپر")
                .Replace("8میلی امپر", "8 میلی امپر")
                .Replace("9میلی امپر", "9 میلی امپر")
                .Replace("0کیلو", "0 کیلو")
                .Replace("1کیلو", "1 کیلو")
                .Replace("2کیلو", "2 کیلو")
                .Replace("3کیلو", "3 کیلو")
                .Replace("4کیلو", "4 کیلو")
                .Replace("5کیلو", "5 کیلو")
                .Replace("6کیلو", "6 کیلو")
                .Replace("7کیلو", "7 کیلو")
                .Replace("8کیلو", "8 کیلو")
                .Replace("9کیلو", "9 کیلو")
                .Replace("0متر", "0 متر")
                .Replace("1متر", "1 متر")
                .Replace("2متر", "2 متر")
                .Replace("3متر", "3 متر")
                .Replace("4متر", "4 متر")
                .Replace("5متر", "5 متر")
                .Replace("6متر", "6 متر")
                .Replace("7متر", "7 متر")
                .Replace("8متر", "8 متر")
                .Replace("9متر", "9 متر")
                .Replace("0نفر", "0 نفر")
                .Replace("1نفر", "1 نفر")
                .Replace("2نفر", "2 نفر")
                .Replace("3نفر", "3 نفر")
                .Replace("4نفر", "4 نفر")
                .Replace("5نفر", "5 نفر")
                .Replace("6نفر", "6 نفر")
                .Replace("7نفر", "7 نفر")
                .Replace("8نفر", "8 نفر")
                .Replace("9نفر", "9 نفر")
                .Replace("0تا", "0 تا")
                .Replace("1تا", "1 تا")
                .Replace("2تا", "2 تا")
                .Replace("3تا", "3 تا")
                .Replace("4تا", "4 تا")
                .Replace("5تا", "5 تا")
                .Replace("6تا", "6 تا")
                .Replace("7تا", "7 تا")
                .Replace("8تا", "8 تا")
                .Replace("9تا", "9 تا")
                .Replace("0گیگ", "0 گیگ")
                .Replace("1گیگ", "1 گیگ")
                .Replace("2گیگ", "2 گیگ")
                .Replace("3گیگ", "3 گیگ")
                .Replace("4گیگ", "4 گیگ")
                .Replace("5گیگ", "5 گیگ")
                .Replace("6گیگ", "6 گیگ")
                .Replace("7گیگ", "7 گیگ")
                .Replace("8گیگ", "8 گیگ")
                .Replace("9گیگ", "9 گیگ")
                .Replace("0بایت", "0 بایت")
                .Replace("1بایت", "1 بایت")
                .Replace("2بایت", "2 بایت")
                .Replace("3بایت", "3 بایت")
                .Replace("4بایت", "4 بایت")
                .Replace("5بایت", "5 بایت")
                .Replace("6بایت", "6 بایت")
                .Replace("7بایت", "7 بایت")
                .Replace("8بایت", "8 بایت")
                .Replace("9بایت", "9 بایت")
                .Replace("0هرتز", "0 هرتز")
                .Replace("1هرتز", "1 هرتز")
                .Replace("2هرتز", "2 هرتز")
                .Replace("3هرتز", "3 هرتز")
                .Replace("4هرتز", "4 هرتز")
                .Replace("5هرتز", "5 هرتز")
                .Replace("6هرتز", "6 هرتز")
                .Replace("7هرتز", "7 هرتز")
                .Replace("8هرتز", "8 هرتز")
                .Replace("9هرتز", "9 هرتز")
                .Replace("0مگ", "0 مگ")
                .Replace("1مگ", "1 مگ")
                .Replace("2مگ", "2 مگ")
                .Replace("3مگ", "3 مگ")
                .Replace("4مگ", "4 مگ")
                .Replace("5مگ", "5 مگ")
                .Replace("6مگ", "6 مگ")
                .Replace("7مگ", "7 مگ")
                .Replace("8مگ", "8 مگ")
                .Replace("9مگ", "9 مگ")
                .Replace("0پیکسل", "0 پیکسل")
                .Replace("1پیکسل", "1 پیکسل")
                .Replace("2پیکسل", "2 پیکسل")
                .Replace("3پیکسل", "3 پیکسل")
                .Replace("4پیکسل", "4 پیکسل")
                .Replace("5پیکسل", "5 پیکسل")
                .Replace("6پیکسل", "6 پیکسل")
                .Replace("7پیکسل", "7 پیکسل")
                .Replace("8پیکسل", "8 پیکسل")
                .Replace("9پیکسل", "9 پیکسل")
                .Replace("0برگ", "0 برگ")
                .Replace("1برگ", "1 برگ")
                .Replace("2برگ", "2 برگ")
                .Replace("3برگ", "3 برگ")
                .Replace("4برگ", "4 برگ")
                .Replace("5برگ", "5 برگ")
                .Replace("6برگ", "6 برگ")
                .Replace("7برگ", "7 برگ")
                .Replace("8برگ", "8 برگ")
                .Replace("9برگ", "9 برگ")
                .Replace("0تسک", "0 تسک")
                .Replace("1تسک", "1 تسک")
                .Replace("2تسک", "2 تسک")
                .Replace("3تسک", "3 تسک")
                .Replace("4تسک", "4 تسک")
                .Replace("5تسک", "5 تسک")
                .Replace("6تسک", "6 تسک")
                .Replace("7تسک", "7 تسک")
                .Replace("8تسک", "8 تسک")
                .Replace("9تسک", "9 تسک")
                .Replace("0برنامه", "0 برنامه")
                .Replace("1برنامه", "1 برنامه")
                .Replace("2برنامه", "2 برنامه")
                .Replace("3برنامه", "3 برنامه")
                .Replace("4برنامه", "4 برنامه")
                .Replace("5برنامه", "5 برنامه")
                .Replace("6برنامه", "6 برنامه")
                .Replace("7برنامه", "7 برنامه")
                .Replace("8برنامه", "8 برنامه")
                .Replace("9برنامه", "9 برنامه")
                .Replace("0وظیفه", "0 وظیفه")
                .Replace("1وظیفه", "1 وظیفه")
                .Replace("2وظیفه", "2 وظیفه")
                .Replace("3وظیفه", "3 وظیفه")
                .Replace("4وظیفه", "4 وظیفه")
                .Replace("5وظیفه", "5 وظیفه")
                .Replace("6وظیفه", "6 وظیفه")
                .Replace("7وظیفه", "7 وظیفه")
                .Replace("8وظیفه", "8 وظیفه")
                .Replace("9وظیفه", "9 وظیفه")
                .Replace("0کارمند", "0 کارمند")
                .Replace("1کارمند", "1 کارمند")
                .Replace("2کارمند", "2 کارمند")
                .Replace("3کارمند", "3 کارمند")
                .Replace("4کارمند", "4 کارمند")
                .Replace("5کارمند", "5 کارمند")
                .Replace("6کارمند", "6 کارمند")
                .Replace("7کارمند", "7 کارمند")
                .Replace("8کارمند", "8 کارمند")
                .Replace("9کارمند", "9 کارمند")
                .Replace("0شرکت", "0 شرکت")
                .Replace("1شرکت", "1 شرکت")
                .Replace("2شرکت", "2 شرکت")
                .Replace("3شرکت", "3 شرکت")
                .Replace("4شرکت", "4 شرکت")
                .Replace("5شرکت", "5 شرکت")
                .Replace("6شرکت", "6 شرکت")
                .Replace("7شرکت", "7 شرکت")
                .Replace("8شرکت", "8 شرکت")
                .Replace("9شرکت", "9 شرکت")
                .Replace("0هزار", "0 هزار")
                .Replace("1هزار", "1 هزار")
                .Replace("2هزار", "2 هزار")
                .Replace("3هزار", "3 هزار")
                .Replace("4هزار", "4 هزار")
                .Replace("5هزار", "5 هزار")
                .Replace("6هزار", "6 هزار")
                .Replace("7هزار", "7 هزار")
                .Replace("8هزار", "8 هزار")
                .Replace("9میلیون", "9 میلیون")
                .Replace("0میلیون", "0 میلیون")
                .Replace("1میلیون", "1 میلیون")
                .Replace("2میلیون", "2 میلیون")
                .Replace("3میلیون", "3 میلیون")
                .Replace("4میلیون", "4 میلیون")
                .Replace("5میلیون", "5 میلیون")
                .Replace("6میلیون", "6 میلیون")
                .Replace("7میلیون", "7 میلیون")
                .Replace("8میلیون", "8 میلیون")
                .Replace("9میلیون", "9 میلیون")
                .Replace("0میلیارد", "0 میلیارد")
                .Replace("1میلیارد", "1 میلیارد")
                .Replace("2میلیارد", "2 میلیارد")
                .Replace("3میلیارد", "3 میلیارد")
                .Replace("4میلیارد", "4 میلیارد")
                .Replace("5میلیارد", "5 میلیارد")
                .Replace("6میلیارد", "6 میلیارد")
                .Replace("7میلیارد", "7 میلیارد")
                .Replace("8میلیارد", "8 میلیارد")
                .Replace("9میلیارد", "9 میلیارد")
                .Replace("0تومان", "0 تومان")
                .Replace("1تومان", "1 تومان")
                .Replace("2تومان", "2 تومان")
                .Replace("3تومان", "3 تومان")
                .Replace("4تومان", "4 تومان")
                .Replace("5تومان", "5 تومان")
                .Replace("6تومان", "6 تومان")
                .Replace("7تومان", "7 تومان")
                .Replace("8تومان", "8 تومان")
                .Replace("9تومان", "9 تومان")
                .Replace("0دلار", "0 دلار")
                .Replace("1دلار", "1 دلار")
                .Replace("2دلار", "2 دلار")
                .Replace("3دلار", "3 دلار")
                .Replace("4دلار", "4 دلار")
                .Replace("5دلار", "5 دلار")
                .Replace("6دلار", "6 دلار")
                .Replace("7دلار", "7 دلار")
                .Replace("8دلار", "8 دلار")
                .Replace("9دلار", "9 دلار")
                .Replace("0یورو", "0 یورو")
                .Replace("1یورو", "1 یورو")
                .Replace("2یورو", "2 یورو")
                .Replace("3یورو", "3 یورو")
                .Replace("4یورو", "4 یورو")
                .Replace("5یورو", "5 یورو")
                .Replace("6یورو", "6 یورو")
                .Replace("7یورو", "7 یورو")
                .Replace("8یورو", "8 یورو")
                .Replace("9یورو", "9 یورو")
                .Replace("0اپلیکیشین", "0 اپلیکیشین")
                .Replace("1اپلیکیشین", "1 اپلیکیشین")
                .Replace("2اپلیکیشین", "2 اپلیکیشین")
                .Replace("3اپلیکیشین", "3 اپلیکیشین")
                .Replace("4اپلیکیشین", "4 اپلیکیشین")
                .Replace("5اپلیکیشین", "5 اپلیکیشین")
                .Replace("6اپلیکیشین", "6 اپلیکیشین")
                .Replace("7اپلیکیشین", "7 اپلیکیشین")
                .Replace("8اپلیکیشین", "8 اپلیکیشین")
                .Replace("9اپلیکیشین", "9 اپلیکیشین")
                .Replace("0بار", "0 بار")
                .Replace("1بار", "1 بار")
                .Replace("2بار", "2 بار")
                .Replace("3بار", "3 بار")
                .Replace("4بار", "4 بار")
                .Replace("5بار", "5 بار")
                .Replace("6بار", "6 بار")
                .Replace("7بار", "7 بار")
                .Replace("8بار", "8 بار")
                .Replace("9بار", "9 بار")
                .Replace("0هسته", "0 هسته")
                .Replace("1هسته", "1 هسته")
                .Replace("2هسته", "2 هسته")
                .Replace("3هسته", "3 هسته")
                .Replace("4هسته", "4 هسته")
                .Replace("5هسته", "5 هسته")
                .Replace("6هسته", "6 هسته")
                .Replace("7هسته", "7 هسته")
                .Replace("8هسته", "8 هسته")
                .Replace("9هسته", "9 هسته")
                .Replace("0کور", "0 کور")
                .Replace("1کور", "1 کور")
                .Replace("2کور", "2 کور")
                .Replace("3کور", "3 کور")
                .Replace("4کور", "4 کور")
                .Replace("5کور", "5 کور")
                .Replace("6کور", "6 کور")
                .Replace("7کور", "7 کور")
                .Replace("8کور", "8 کور")
                .Replace("9کور", "9 کور")
                .Replace("0ترد", "0 ترد")
                .Replace("1ترد", "1 ترد")
                .Replace("2ترد", "2 ترد")
                .Replace("3ترد", "3 ترد")
                .Replace("4ترد", "4 ترد")
                .Replace("5ترد", "5 ترد")
                .Replace("6ترد", "6 ترد")
                .Replace("7ترد", "7 ترد")
                .Replace("8ترد", "8 ترد")
                .Replace("9ترد", "9 ترد")
                .Replace("0چند", "0 چند")
                .Replace("1چند", "1 چند")
                .Replace("2چند", "2 چند")
                .Replace("3چند", "3 چند")
                .Replace("4چند", "4 چند")
                .Replace("5چند", "5 چند")
                .Replace("6چند", "6 چند")
                .Replace("7چند", "7 چند")
                .Replace("8چند", "8 چند")
                .Replace("9چند", "9 چند")
                .Replace("0برابر", "0 برابر")
                .Replace("1برابر", "1 برابر")
                .Replace("2برابر", "2 برابر")
                .Replace("3برابر", "3 برابر")
                .Replace("4برابر", "4 برابر")
                .Replace("5برابر", "5 برابر")
                .Replace("6برابر", "6 برابر")
                .Replace("7برابر", "7 برابر")
                .Replace("8برابر", "8 برابر")
                .Replace("9برابر", "9 برابر")
                .Replace("0شغل", "0 شغل")
                .Replace("1شغل", "1 شغل")
                .Replace("2شغل", "2 شغل")
                .Replace("3شغل", "3 شغل")
                .Replace("4شغل", "4 شغل")
                .Replace("5شغل", "5 شغل")
                .Replace("6شغل", "6 شغل")
                .Replace("7شغل", "7 شغل")
                .Replace("8شغل", "8 شغل")
                .Replace("9شغل", "9 شغل")
                .Replace("0دقیقه", "0 دقیقه")
                .Replace("1دقیقه", "1 دقیقه")
                .Replace("2دقیقه", "2 دقیقه")
                .Replace("3دقیقه", "3 دقیقه")
                .Replace("4دقیقه", "4 دقیقه")
                .Replace("5دقیقه", "5 دقیقه")
                .Replace("6دقیقه", "6 دقیقه")
                .Replace("7دقیقه", "7 دقیقه")
                .Replace("8دقیقه", "8 دقیقه")
                .Replace("9دقیقه", "9 دقیقه")
                .Replace("0ساعت", "0 ساعت")
                .Replace("1ساعت", "1 ساعت")
                .Replace("2ساعت", "2 ساعت")
                .Replace("3ساعت", "3 ساعت")
                .Replace("4ساعت", "4 ساعت")
                .Replace("5ساعت", "5 ساعت")
                .Replace("6ساعت", "6 ساعت")
                .Replace("7ساعت", "7 ساعت")
                .Replace("8ساعت", "8 ساعت")
                .Replace("9ساعت", "9 ساعت")
                .Replace("0دوربین", "0 دوربین")
                .Replace("1دوربین", "1 دوربین")
                .Replace("2دوربین", "2 دوربین")
                .Replace("3دوربین", "3 دوربین")
                .Replace("4دوربین", "4 دوربین")
                .Replace("5دوربین", "5 دوربین")
                .Replace("6دوربین", "6 دوربین")
                .Replace("7دوربین", "7 دوربین")
                .Replace("8دوربین", "8 دوربین")
                .Replace("9دوربین", "9 دوربین")
                .Replace("0پردازنده", "0 پردازنده")
                .Replace("1پردازنده", "1 پردازنده")
                .Replace("2پردازنده", "2 پردازنده")
                .Replace("3پردازنده", "3 پردازنده")
                .Replace("4پردازنده", "4 پردازنده")
                .Replace("5پردازنده", "5 پردازنده")
                .Replace("6پردازنده", "6 پردازنده")
                .Replace("7پردازنده", "7 پردازنده")
                .Replace("8پردازنده", "8 پردازنده")
                .Replace("9پردازنده", "9 پردازنده")

                .Replace("0cm", "0 cm")
                .Replace("1cm", "1 cm")
                .Replace("2cm", "2 cm")
                .Replace("3cm", "3 cm")
                .Replace("4cm", "4 cm")
                .Replace("5cm", "5 cm")
                .Replace("6cm", "6 cm")
                .Replace("7cm", "7 cm")
                .Replace("8cm", "8 cm")
                .Replace("9cm", "9 cm")

                ;

        }

        private string CleanEmoji(string text)
        {
            return Regex.Replace(text, @"\p{Cs}", "");
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
