using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace TerexCrawler.Models.Const
{
    public class Aspects
    {
        public List<Aspect> Mobiles { get; set; }

        public Dictionary<string, string> MobileCategories { get; set; }
        public Dictionary<string, string> MobileCategoriesTitle { get; set; }

        //public static List<string> mobile = new List<string>() {
        //    "موبایل#نوآوری",
        //    "موبایل#کیفیت_ساخت",
        //    "مویایل#عمومی",
        //    "موبایل#قیمت",
        //    "موبایل#قابلیت",
        //    "موبایل#طراحی",
        //    "موبایل#حافظه",
        //    "موبایل#پردازنده",
        //    "موبایل#متفرقه",

        //    "سیستم_عامل#عملکرد",
        //    "سیستم_عامل#بروزرسانی",
        //    "سیستم_عامل#نسخه",

        //    "باتری#کیفیت",
        //    "باتری#شارژ",

        //    "پشتیبانی#کیفیت",

        //    "دوربین#سلفی",
        //    "دوربین#کیفیت",

        //    "برند#عمومی",
        //    "صفحه_نمایش#عمومی",
        //    "صفحه_نمایش#کیفیت",

        //    "خدمات#ارسال",
        //    "خدمات#عمومی",
        //    "خدمات#کمپین",
        //    "خدمات#گارانتی"
        //};

        //public static List<string> laptop = new List<string>() {
        //    "موبایل#نوآوری",
        //    "موبایل#کیفیت_ساخت",
        //    "مویایل#عمومی",
        //    "موبایل#قیمت",
        //    "موبایل#قابلیت",
        //    "موبایل#طراحی",
        //    "موبایل#حافظه",
        //    "موبایل#پردازنده",
        //    "موبایل#متفرقه",

        //    "سیستم_عامل#عملکرد",
        //    "سیستم_عامل#بروزرسانی",
        //    "سیستم_عامل#نسخه",

        //    "باتری#کیفیت",
        //    "باتری#شارژ",

        //    "پشتیبانی#کیفیت",

        //    "دوربین#سلفی",
        //    "دوربین#کیفیت",

        //    "برند#عمومی",
        //    "صفحه_نمایش#عمومی",
        //    "صفحه_نمایش#کیفیت",

        //    "خدمات#ارسال",
        //    "خدمات#عمومی",
        //    "خدمات#کمپین",
        //    "خدمات#گارانتی"};

        public Aspects()
        {
            this.Mobiles = new List<Aspect>()
            {
                new Aspect { id = 1, Category = "mobile", Feature = "genaral", Title = "عمومی" },
                new Aspect { id = 2, Category = "mobile", Feature = "price", Title = "قیمت" },
                new Aspect { id = 3, Category = "mobile", Feature = "brand", Title = "ارزش برند" },

                //new Aspect { id = 11, Category = "appearance", Feature = "body_material", Title = "جنس بدنه" },
                new Aspect { id = 12, Category = "appearance", Feature = "size", Title = "ابعاد" },
                //new Aspect { id = 13, Category = "appearance", Feature = "weight", Title = "وزن" },
                //new Aspect { id = 14, Category = "appearance", Feature = "waterproof_and_dustproof", Title = "ضدآب و گردوغبار بودن" },
                //new Aspect { id = 15, Category = "appearance", Feature = "dual_sim", Title = "دو سیم کارت" },
                new Aspect { id = 16, Category = "appearance", Feature = "design", Title = "طراحی ظاهری" },
                //new Aspect { id = 17, Category = "appearance", Feature = "endurance", Title = "استقامت" },

                new Aspect { id = 21, Category = "processing", Feature = "cpu", Title = "پردازنده" },
                //new Aspect { id = 22, Category = "processing", Feature = "gpu", Title = "پردازنده گرافیکی" },
                new Aspect { id = 23, Category = "processing", Feature = "ram", Title = "میزان حافظه رم" },
                //new Aspect { id = 23, Category = "processing", Feature = "heat_rate_in_processing", Title = "میزان حرارت در پردازش" },

                new Aspect { id = 31, Category = "storage", Feature = "storage_space", Title = "فضای ذخیره سازی" },
                //new Aspect { id = 32, Category = "storage", Feature = "ability_memory_card", Title = "دریافت کارت حافظه" },

                new Aspect { id = 41, Category = "screen", Feature = "size_and_shape", Title = "سایز و شکل" },
                new Aspect { id = 42, Category = "screen", Feature = "quality", Title = "کیفیت" },
                //new Aspect { id = 43, Category = "screen", Feature = "type_and_technology", Title = "نوع و تکنولوژی" },

                new Aspect { id = 51, Category = "camera", Feature = "quality", Title = "دقت و کیفیت دوربین" },
                new Aspect { id = 52, Category = "camera", Feature = "quality_front_camera", Title = "دوربین سلفی" },
                //new Aspect { id = 53, Category = "camera", Feature = "video_quality", Title = "کیفیت فیلم برداری" },
                //new Aspect { id = 54, Category = "camera", Feature = "flash_quality", Title = "کیفیت فلاش" },

                new Aspect { id = 61, Category = "battery", Feature = "capacity", Title = "ظرفیت" },
                //new Aspect { id = 62, Category = "battery", Feature = "charging_rate", Title = "میزان شارژ دهی" },
                new Aspect { id = 63, Category = "battery", Feature = "charging_speed", Title = "سرعت شارژ شدن" },

                new Aspect { id = 71, Category = "os", Feature = "operation_performance", Title = "کارایی" },
                new Aspect { id = 72, Category = "software", Feature = "speed_of_os", Title = "سرعت" },
                //new Aspect { id = 73, Category = "software", Feature = "speed_of_apps_and_games", Title = "سرعت اجرای نرم‌افزارها و بازی‌ها" },
                //new Aspect { id = 74, Category = "software", Feature = "software_on_the_phone", Title = "سرویس‌ها و نرم‌افزارهای موجود روی گوشی" },
                //new Aspect { id = 75, Category = "software", Feature = "user_interface", Title = "زیبایی رابط کاربری" },

                //new Aspect { id = 81, Category = "communication", Feature = "coverage", Title = "آنتن‌دهی" },
                //new Aspect { id = 82, Category = "communication", Feature = "data_exchange_speed", Title = "سرعت تبادل داده‌ها" },

                //new Aspect { id = 91, Category = "accessories", Feature = "accessories_in_the_box", Title = "لوازم جانبی همراه" },
                //new Aspect { id = 92, Category = "accessories", Feature = "accessories_on_the_market", Title = "تجهیزات جانبی در بازار" },

                new Aspect { id = 101, Category = "service", Feature = "warranty_and_service", Title = "گارانتی و خدمات" },
                //new Aspect { id = 102, Category = "service", Feature = "delivery", Title = "ارسال" },

            };
            MobileCategories = new Dictionary<string, string>();
            MobileCategories.Add("mobile", "موبایل");
            MobileCategories.Add("appearance", "ظاهر");
            MobileCategories.Add("processing", "توان پردازشی");
            MobileCategories.Add("storage", "فضای ذخیره سازی");
            MobileCategories.Add("screen", "صفحه نمایش");
            MobileCategories.Add("camera", "دوربین");
            MobileCategories.Add("battery", "باتری");
            MobileCategories.Add("software", "نرم‌افزار");
            MobileCategories.Add("communication", "توانایی‌های ارتباطی");
            MobileCategories.Add("accessories", "تجهیزات جانبی");
            MobileCategories.Add("service", "خدمات");

            MobileCategoriesTitle = new Dictionary<string, string>();
            MobileCategoriesTitle.Add("موبایل", "mobile");
            MobileCategoriesTitle.Add("ظاهر", "appearance");
            MobileCategoriesTitle.Add("توان پردازشی", "processing");
            MobileCategoriesTitle.Add("فضای ذخیره سازی", "storage");
            MobileCategoriesTitle.Add("صفحه نمایش", "screen");
            MobileCategoriesTitle.Add("دوربین", "camera");
            MobileCategoriesTitle.Add("باتری", "battery");
            MobileCategoriesTitle.Add("نرم‌افزار", "software");
            MobileCategoriesTitle.Add("توانایی‌های ارتباطی", "communication");
            MobileCategoriesTitle.Add("تجهیزات جانبی", "accessories");
            MobileCategoriesTitle.Add("خدمات", "service");

        }
    }
    public class Aspect
    {
        public int id { get; set; }
        public string Category { get; set; }
        public string Feature { get; set; }
        public string Title { get; set; }

    }

    [XmlRoot(ElementName = "sentence")]
    public class Sentence
    {
        [XmlElement(ElementName = "text")]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlElement(ElementName = "Opinions")]
        public Opinions Opinions { get; set; }
        [XmlAttribute(AttributeName = "OutOfScope")]
        public string OutOfScope { get; set; }
    }

    [XmlRoot(ElementName = "Opinion")]
    public class Opinion
    {
        [XmlAttribute(AttributeName = "category")]
        public string Category { get; set; }
        [XmlAttribute(AttributeName = "polarity")]
        public string Polarity { get; set; }
    }

    [XmlRoot(ElementName = "Opinions")]
    public class Opinions
    {
        [XmlElement(ElementName = "Opinion")]
        public List<Opinion> Opinion { get; set; }
    }

    [XmlRoot(ElementName = "sentences")]
    public class Sentences
    {
        [XmlElement(ElementName = "sentence")]
        public List<Sentence> Sentence { get; set; }
    }

    [XmlRoot(ElementName = "Review")]
    public class Review
    {
        [XmlElement(ElementName = "sentences")]
        public Sentences Sentences { get; set; }
        [XmlAttribute(AttributeName = "rid")]
        public string Rid { get; set; }
    }

    [XmlRoot(ElementName = "Reviews")]
    public class Reviews
    {
        [XmlElement(ElementName = "Review")]
        public List<Review> Review { get; set; }
    }

}

