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
            
            return asct.AspectList;
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
