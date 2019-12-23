using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TerexCrawler.Models.Const;

namespace TerexCrawler.Apps.ReviewTaggerWPF
{
    public static class AspectsAir
    {
        public static List<Aspect> FillAspect()
        {
            Aspects asct = new Aspects();
            AspectToTitle = asct.Mobiles.ToDictionary(a => a.Feature, x => x.Title);

            TitleToAspect = asct.Mobiles.ToDictionary(a => a.Title, a => a.Feature);
            FeaturesToCategory = new Dictionary<string, string>();
            FeaturesToCategory = asct.Mobiles.ToDictionary(a => a.Feature, a => a.Category);
            CategoryToTitle = asct.MobileCategories;
            TitleToCategory = asct.MobileCategoriesTitle;

            AspectList = asct.Mobiles.Select(x => $"{CategoryToTitle[x.Category]}#{x.Title}").ToList();


            return asct.Mobiles;
        }

        public static List<Aspect> Aspects = FillAspect();

        public static Dictionary<string, string> CategoryToTitle { get; set; }
        public static Dictionary<string, string> TitleToCategory { get; set; }
        public static Dictionary<string, string> FeaturesToCategory { get; set; }
        public static Dictionary<string, string> AspectToTitle { get; set; }
        public static Dictionary<string, string> TitleToAspect { get; set; }
        public static List<string> AspectList { get; set; }
    }
}
