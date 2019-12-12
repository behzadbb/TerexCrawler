﻿using System;
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
        static List<sentence> sentences = new List<sentence>();
        private int _sentenceId = 0;
        string tagger = "behzad";
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
                    review._id = ObjectId.GenerateNewId(DateTime.Now);
                    review.rid = digikalaProduct.DKP;
                    AddReviewToDBParam param = new AddReviewToDBParam
                    {
                        review = review,
                        id = digikalaProduct._id,
                        tagger = tagger
                    };
                    bool resultAddReview = false;
                    //bool resultAddReview = digikala.AddReviewToDB(param); // ☺ Api
                    using (var Api = new WebAppApiCall())
                    {
                        resultAddReview = Api.GetFromApi<bool>("AddReview", param);
                    }
                    if (resultAddReview)
                    {
                        sentences.Clear();
                        sentenceIdReset();
                        review = new Review();
                    }
                    else
                    {
                        MessageBox.Show("ثبت با مشکل روبرو شده است دوباره سعی کنید.", "Warning");
                        return;
                    }
                }
                GetFirstProductByCategoryParam getProductParam = new GetFirstProductByCategoryParam
                {
                    category = "گوشی موبایل",
                    title = "",
                    tagger = tagger
                };
                // ☺ Api
                using (var Api = new WebAppApiCall())
                {
                    digikalaProduct = Api.GetFromApi<DigikalaProductDTO>("GetFirstProductByCategory", getProductParam);
                }
                //digikalaProduct = digikala.GetFirstProductByCategory<DigikalaProductDTO>(getProductParam).Result;

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

            List<string> aspect = new List<string> { "باتری#مدت شارژ", "باتری#کیفیت", "صفحه نمایش" };
            listNegative.ItemsSource = aspect;
            listNeutral.ItemsSource = aspect;
            listPositive.ItemsSource = aspect;
            listAspects.Items.Clear();
            opinions.Clear();
        }
        private void AddSentence()
        {

        }
    }
}