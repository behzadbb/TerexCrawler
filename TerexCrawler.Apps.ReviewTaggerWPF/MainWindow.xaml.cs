using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TerexCrawler.Entites;
using TerexCrawler.Models.Interfaces;
using TerexCrawler.Services.Digikala;
using TerexCrawler.Models.DTO.Digikala;
using TerexCrawler.Models;
using TerexCrawler.Models.Enums;
using MongoDB.Bson;
using TerexCrawler.Apps.ReviewTaggerWPF.Helpers;
using TerexCrawler.Models.Const;
using TerexCrawler.Models.DTO;
using TerexCrawler.Models.DTO.Api;
using System.ComponentModel;

namespace TerexCrawler.Apps.ReviewTaggerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReviewDTO review = new ReviewDTO();
        DigikalaProductDTO digikalaProduct = new DigikalaProductDTO();
        List<Opinion> opinions = new List<Opinion>();
        static List<sentence> sentences = new List<sentence>();
        private int _sentenceId = 0;
        public User user { get; set; }
        public List<string> aspects { get; set; }
        public List<Aspect> Aspects { get; set; }
        public Dictionary<string, string> CategoryToTitle { get; set; }
        public Dictionary<string, string> TitleToCategory { get; set; }
        public Dictionary<string, string> FeaturesToCategory { get; set; }
        public Dictionary<string, string> AspectToTitle { get; set; }
        public Dictionary<string, string> TitleToAspect { get; set; }
        int sentenceId { get { return _sentenceId++; } }
        void sentenceIdReset() { _sentenceId = 0; }
        int commentCount = 0;
        int commentCurrentIndex = -1;
        public MainWindow()
        {
            using (var Api = new WebAppApiCall())
            {
                GetAspectsDTO getAspects = new GetAspectsDTO() { AspectType = (int)AspectTypes.Mobile };
                var res = Api.GetFromApi<GetAspectsResponseDTO>("GetAspects", getAspects);
                Aspects = res.Aspects;
                
                AspectToTitle = new Dictionary<string, string>();
                
                AspectToTitle = res.Aspects.ToDictionary(a => a.Feature, x => x.Title);
                TitleToAspect = new Dictionary<string, string>();
                TitleToAspect = res.Aspects.ToDictionary(a => a.Title, a => a.Feature);
                FeaturesToCategory = new Dictionary<string, string>();
                FeaturesToCategory = res.Aspects.ToDictionary(a => a.Feature, a => a.Category);
                CategoryToTitle = res.Categories;
                TitleToCategory = res.CategoriesTitle;

                aspects = res.Aspects.Select(x => $"{CategoryToTitle[x.Category]}#{x.Title}").ToList();
            }
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            nextReview();
        }

        #region ListBox
        private void listPositive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPositive.SelectedIndex != null && listPositive.SelectedItem != null && listPositive.SelectedIndex != -1)
            {
                string selectItem = listPositive.Items[listPositive.SelectedIndex].ToString();
                string[] item = selectItem.Split('#');
                listAspects.Items.Add(selectItem);

                Opinion opinion = new Opinion();
                string feature = TitleToAspect[item[1]];
                opinion.category = TitleToCategory[item[0]];
                opinion.categoryClass = feature;
                opinion.polarity = PolarityType.positive.ToString();
                opinion.polarityClass = (int)PolarityType.positive;

                opinions.Add(opinion);
            }
        }

        private void listNeutral_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listNeutral.SelectedIndex != null && listNeutral.SelectedItem != null && listNeutral.SelectedIndex != -1)
            {
                string selectItem = listNeutral.Items[listNeutral.SelectedIndex].ToString();
                string[] item = selectItem.Split('#');
                listAspects.Items.Add(selectItem);
                
                Opinion opinion = new Opinion();
                string feature = TitleToAspect[item[1]];
                opinion.category = TitleToCategory[item[0]];
                opinion.categoryClass = feature;
                opinion.polarity = PolarityType.neutral.ToString();
                opinion.polarityClass = (int)PolarityType.neutral;

                opinions.Add(opinion);
            }
        }

        private void listNegative_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listNegative.SelectedIndex != null && listNegative.SelectedItem != null && listNegative.SelectedIndex != -1)
            {
                string selectItem = listNegative.Items[listNegative.SelectedIndex].ToString();
                string[] item = selectItem.Split('#');
                listAspects.Items.Add(selectItem);

                Opinion opinion = new Opinion();
                string feature = TitleToAspect[item[1]];
                opinion.category = TitleToCategory[item[0]];
                opinion.categoryClass = feature;
                opinion.polarity = PolarityType.negative.ToString();
                opinion.polarityClass = (int)PolarityType.negative;

                opinions.Add(opinion);
            }
        }
        #endregion

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (opinions != null && opinions.Count() > 0 && !string.IsNullOrEmpty(txtSelectReview.Text))
            {
                sentences.Add(new sentence
                {
                    id = sentenceId,
                    Text = txtSelectReview.Text.Trim(),
                    Opinions = opinions
                });
                opinions.Clear();
            }

            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                if (sentences != null && sentences.Count() > 0)
                {
                    if (review.sentences == null)
                    {
                        review.sentences = new List<sentence>();
                    }
                    review.sentences.AddRange(sentences);
                    sentences.Clear();
                }
                if (review != null && review.sentences != null && review.sentences.Count() > 0)
                {
                    review.CreateDate = DateTime.Now;
                    //review._id = ObjectId.GenerateNewId(DateTime.Now);
                    review.rid = digikalaProduct.DKP;
                    AddReviewToDBParam param = new AddReviewToDBParam
                    {
                        review = review,
                        id = digikalaProduct._id,
                        tagger = user.Username
                    };
                    AddReviewToDBResponse resultAddReview = new AddReviewToDBResponse() { Success = false };
                    //bool resultAddReview = digikala.AddReviewToDB(param); // ☺ Api
                    using (var Api = new WebAppApiCall())
                    {
                        resultAddReview = Api.GetFromApi<AddReviewToDBResponse>("AddReviewtodb", param);
                    }
                    if (resultAddReview.Success)
                    {
                        sentences.Clear();
                        sentenceIdReset();
                        review = new ReviewDTO();
                    }
                    else
                    {
                        MessageBox.Show("ثبت با مشکل روبرو شده است دوباره سعی کنید.", "Warning");
                        return;
                    }
                }
                GetFirstProductByCategoryParam getProductParam = new GetFirstProductByCategoryParam
                {
                    category = user.Category,
                    title = user.Title,
                    Brand = user.Brand,
                    tagger = user.Username
                };
                // ☺ Api
                using (var Api = new WebAppApiCall())
                {
                    digikalaProduct = Api.GetFromApi<DigikalaProductDTO>("GetFirstProductByCategory", getProductParam);
                }
                //digikalaProduct = digikala.GetFirstProductByCategory<DigikalaProductDTO>(getProductParam).Result;
                commentCurrentIndex = -1;
                commentCount = digikalaProduct.Comments.Count();
                nextReview();
                txtProductId.Text = digikalaProduct.DKP.ToString();
                txtTitle.Text = digikalaProduct.Title;
                txtEnTitle.Text = digikalaProduct.TitleEN;
                review.ProductID = digikalaProduct.DKP;
            }
        }

        private void nextReview()
        {
            commentCurrentIndex += 1;
            if (commentCurrentIndex < commentCount)
            {
                var comment = digikalaProduct.Comments.Skip(commentCurrentIndex).Take(1).FirstOrDefault();
                txtReview.Text = comment.Review;
                txtReviewTitle.Text = comment.Title;
                lblCount.Content = $"{commentCurrentIndex + 1} / {commentCount}";
            }

            fillAspects();
            opinions.Clear();
        }

        private void btnAddSentence_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSelectReview.Text))
            {
                sentence sentence = new sentence();
                if (opinions != null && opinions.Count() > 0)
                {
                    var _opinions = opinions.Select(x => new Opinion { category = x.category, categoryClass = x.categoryClass, polarity = x.polarity, polarityClass = x.polarityClass }).ToList();
                    sentence = new sentence()
                    {
                        id = sentenceId,
                        Text = txtSelectReview.Text.Trim(),
                        Opinions = _opinions
                    };

                }
                if (opinions == null || opinions.Count() == 0)
                {
                    sentence = new sentence()
                    {
                        id = sentenceId,
                        Text = txtSelectReview.Text.Trim(),
                        Opinions = new List<Opinion>(),
                        OutOfScope = true
                    };
                }
                sentences.Add(sentence);
                var sen = sentences.ToList();
                txtReview.Text = txtReview.Text.Replace(txtSelectReview.Text.Trim(), "").Trim();
                txtSelectReview.Text = "";
            }

            fillAspects();
            listAspects.Items.Clear();
            opinions.Clear();
        }
        //struct newAspect
        //{
        //    public int Id { get; set; }
        //    public string Title { get; set; }
        //    public string Group { get; set; }
        //};
        private void fillAspects()
        {
            //List<newAspect> newAspects = Aspects.Select(x => new newAspect { Id = x.id, Title = x.Title, Group = CategoryToTitle[x.Category] }).ToList();
            //ICollectionView view = CollectionViewSource.GetDefaultView(newAspects);
            //view.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
            //view.SortDescriptions.Add(new SortDescription("Id", ListSortDirection.Ascending));

            listNegative.UnselectAll();
            //listNegative.Items.Clear();
            listNegative.ItemsSource = aspects;

            listNeutral.UnselectAll();
            //listNeutral.Items.Clear();
            listNeutral.ItemsSource = aspects;

            listPositive.UnselectAll();
            //listPositive.Items.Clear();
            listPositive.ItemsSource = aspects;
        }

        private void txtSelectReview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            pasteReview();
        }

        private void lblSelectReview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            pasteReview();
        }
        private void pasteReview()
        {
            if (string.IsNullOrEmpty(txtSelectReview.Text.Trim()) && !string.IsNullOrEmpty(Clipboard.GetText().Trim()))
            {
                txtSelectReview.Text = Clipboard.GetText();
            }
        }

        private void btnDeleteAspect_Click(object sender, RoutedEventArgs e)
        {
            fillAspects();
            opinions.Clear();
            listAspects.Items.Clear();
        }
    }
}