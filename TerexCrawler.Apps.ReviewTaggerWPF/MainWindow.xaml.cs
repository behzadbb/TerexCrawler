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

namespace TerexCrawler.Apps.ReviewTaggerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Review review = new Review();
        DigikalaProductDTO digikalaProduct = new DigikalaProductDTO();
        List<Opinion> opinions = new List<Opinion>();
        List<sentence> sentences = new List<sentence>();
        private int _sentenceId = 0;
        int sentenceId { get { return _sentenceId++; } }
        void sentenceIdReset() { _sentenceId = 0; }
        int commentCount = 0;
        int commentCurrentIndex = -1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            nextReview();
        }

        #region ListBox
        private void listPositive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = listPositive.Items[listPositive.SelectedIndex].ToString();
            listAspects.Items.Add(item);
            Opinion opinion = new Opinion();
            opinion.category = item;
            opinion.categoryClass = item;
            opinion.polarity = PolarityType.positive.ToString();
            opinion.polarityClass = (int)PolarityType.positive;

            opinions.Add(opinion);
        }

        private void listNeutral_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = listNeutral.Items[listNeutral.SelectedIndex].ToString();
            listAspects.Items.Add(item);

            Opinion opinion = new Opinion();
            opinion.category = item;
            opinion.categoryClass = item;
            opinion.polarity = PolarityType.neutral.ToString();
            opinion.polarityClass = (int)PolarityType.neutral;

            opinions.Add(opinion);
        }

        private void listNegative_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = listNegative.Items[listNegative.SelectedIndex].ToString();
            listAspects.Items.Add(item);

            Opinion opinion = new Opinion();
            opinion.category = item;
            opinion.categoryClass = item;
            opinion.polarity = PolarityType.negative.ToString();
            opinion.polarityClass = (int)PolarityType.negative;

            opinions.Add(opinion);
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
                    review.sentences.AddRange(sentences);
                    review.CreateDate = DateTime.Now;
                    review._id = ObjectId.GenerateNewId(DateTime.Now);
                    review.rid = digikalaProduct.DKP;
                    sentences.Clear();
                    sentenceIdReset();
                    digikala.AddReviewToDB(review);
                }
                // Api
                digikalaProduct = digikala.GetFirstProductByCategory<DigikalaProductDTO>("گوشی موبایل", "سامسونگ", "behzad").Result;

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
            List<string> aspect = new List<string> { "باتری#مدت شارژ", "باتری#کیفیت", "صفحه نمایش" };
            listNegative.ItemsSource = aspect;
            listNeutral.ItemsSource = aspect;
            listPositive.ItemsSource = aspect;
            opinions.Clear();
        }

        private void btnAddSentence_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSelectReview.Text))
            {
                var sentence = new sentence()
                {
                    id = sentenceId,
                    Text = txtSelectReview.Text.Trim()
                };
                if (opinions != null && opinions.Count() > 0)
                {
                    sentence.Opinions = opinions;
                }
                if (opinions == null || opinions.Count() == 0)
                {
                    sentence.OutOfScope = true;
                }
                sentences.Add(sentence);
                opinions.Clear();
                txtReview.Text=txtReview.Text.Replace(txtSelectReview.Text.Trim(),"").Trim();
            }

            List<string> aspect = new List<string> { "باتری#مدت شارژ", "باتری#کیفیت", "صفحه نمایش" };
            listNegative.ItemsSource = aspect;
            listNeutral.ItemsSource = aspect;
            listPositive.ItemsSource = aspect;
            opinions.Clear();
        }
    }
}