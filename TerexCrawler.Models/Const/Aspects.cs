using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TerexCrawler.Models.Const
{
    public class Aspects
    {
        public List<Aspect> AspectList { get; set; }

        public Dictionary<string, string> CategoryToTitle { get { return this.CategoryList.ToDictionary(a => a.CategoryName, x => x.CategoryTitle); } }
        public Dictionary<string, string> TitleToCategory { get { return this.CategoryList.ToDictionary(a => a.CategoryTitle, x => x.CategoryName); } }
        public Dictionary<string, string> AspectToTitle { get { return this.AspectList.ToDictionary(a => a.Feature, x => x.Title); } }
        public Dictionary<string, string> TitleToAspect { get { return this.AspectList.ToDictionary(a => a.Title, x => x.Feature); } }
        public List<Category> CategoryList { get; set; }

        public Aspects()
        {
            this.CategoryList = new List<Category>()
            {
                new Category{id=1, CategoryName="mobile", CategoryTitle="موبایل"},
                new Category{id=2, CategoryName="appearance", CategoryTitle="ظاهر"},
                new Category{id=3, CategoryName="cpu", CategoryTitle="پردازشگر"},
                new Category{id=4, CategoryName="memory", CategoryTitle="ذخیره"},
                new Category{id=5, CategoryName="screen", CategoryTitle="نمایشگر"},
                new Category{id=6, CategoryName="camera", CategoryTitle="دوربین"},
                new Category{id=7, CategoryName="battery", CategoryTitle="باتری"},
                new Category{id=8, CategoryName="os", CategoryTitle="سیستم عامل"},
                new Category{id=9, CategoryName="software", CategoryTitle="نرم افزار"},
                new Category{id=10, CategoryName="service", CategoryTitle="خدمات"},
                new Category{id=11, CategoryName="communication", CategoryTitle="ارتباطی"},
                new Category{id=12, CategoryName="COMPANY", CategoryTitle="برند"},
            };

            this.AspectList = new List<Aspect>()
            {
                new Aspect { id = 1, Feature = "genaral", Title = "عمومی" },
                new Aspect { id = 2, Feature = "price", Title = "قیمت" },
                new Aspect { id = 3, Feature = "quality", Title = "کیفیت" },
                new Aspect { id = 4, Feature = "usability", Title = "سهولت استفاده" },
                new Aspect { id = 5, Feature = "connectivity", Title = "اتصال" },
                new Aspect { id = 6, Feature = "operation_performance", Title = "کارایی عملکرد" },
                new Aspect { id = 7, Feature = "design_features", Title = "ویژگی های طراحی" },
                new Aspect { id = 9, Feature = "miscellaneous", Title = "متفرقه" },
            };
        }
    }

    public class ResturantAspects
    {
        public List<Aspect> AspectList { get; set; }
        public Dictionary<string,string> LabelTo { get; set; }
        //public Dictionary<string, string> CategoryToTitle { get { return this.CategoryList.ToDictionary(a => a.CategoryName, x => x.CategoryTitle); } }
        //public Dictionary<string, string> TitleToCategory { get { return this.CategoryList.ToDictionary(a => a.CategoryTitle, x => x.CategoryName); } }
        //public Dictionary<string, string> AspectToTitle { get { return this.AspectList.ToDictionary(a => a.Feature, x => x.Title); } }
        //public Dictionary<string, string> TitleToAspect { get { return this.AspectList.ToDictionary(a => a.Title, x => x.Feature); } }
        public List<Label> CategoryList { get; set; }

        public ResturantAspects()
        {
            this.CategoryList = new List<Label>()
            {
                new Label{id=0, LabelName="RESTAURANT#GENERAL", LabelTitle="رستوران#عمومی" },
                new Label{id=1, LabelName="SERVICE#GENERAL", LabelTitle="خدمات#عمومی" },
                new Label{id=2, LabelName="FOOD#QUALITY", LabelTitle="غذا#کیفیت"},
                new Label{id=3, LabelName="FOOD#STYLE_OPTIONS", LabelTitle="غذا#تنوع"},
                new Label{id=4, LabelName="DRINKS#STYLE_OPTIONS", LabelTitle="نوشیدنی#تنوع"},
                new Label{id=5, LabelName="DRINKS#PRICES", LabelTitle="نوشیدنی#قیمت"},
                new Label{id=6, LabelName="RESTAURANT#PRICES", LabelTitle="رستوران#قیمت"},
                new Label{id=7, LabelName="RESTAURANT#MISCELLANEOUS", LabelTitle="رستوران#متفرقه"},
                new Label{id=8, LabelName="AMBIENCE#GENERAL", LabelTitle="محیط#عمومی"},
                new Label{id=9, LabelName="FOOD#PRICES", LabelTitle="غذا#قیمت"},
                new Label{id=10, LabelName="LOCATION#GENERAL", LabelTitle="لوکیشن#عمومی"},
                new Label{id=11, LabelName="DRINKS#QUALITY", LabelTitle="نوشیدنی#کیفیت"},
                new Label{id=12, LabelName="SERVICE#DELIVERY", LabelTitle="خدمات#ارسال"},
                new Label{id=13, LabelName="FOOD#WARM", LabelTitle="غذا#گرم"},
                new Label{id=14, LabelName="FOOD#VOLUME", LabelTitle="غذا#حجم"}
            };
        }
    }

    public class Aspect
    {
        public int id { get; set; }
        public string Feature { get; set; }
        public string Title { get; set; }

    }

    public class Category
    {
        public int id { get; set; }
        public string CategoryName { get; set; }
        public string CategoryTitle { get; set; }
    }
    public class Label
    {
        public int id { get; set; }
        public string LabelName { get; set; }
        public string LabelTitle { get; set; }
    }
}

