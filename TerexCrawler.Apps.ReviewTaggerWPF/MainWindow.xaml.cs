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

namespace TerexCrawler.Apps.ReviewTaggerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Review review = new Review();
        DigikalaProductDTO digikalaProduct = new DigikalaProductDTO();
        List<Opinion> Opinions = new List<Opinion>();
        public List<sentence> sentences = new List<sentence>();
        int commentCount = 0;
        int commentCurrentIndex = -1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var ss = listAspects.Items;
            //Opinions.AddRange(listAspects.ItemsSource);
            //nextReview();
        }

        #region ListBox
        private void listPositive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = listPositive.Items[listPositive.SelectedIndex].ToString();
            listAspects.Items.Add(item);
            //listPositive.Items.RemoveAt(listPositive.SelectedIndex);
        }

        private void listNeutral_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = listNeutral.Items[listNeutral.SelectedIndex].ToString();
            listAspects.Items.Add(item);
        }

        private void listNegative_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string item = listNegative.Items[listNegative.SelectedIndex].ToString();
            listAspects.Items.Add(item);
        }
        #endregion

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            using (IWebsiteCrawler digikala = new DigikalaHelper())
            {
                digikalaProduct = digikala.GetFirstProductByCategory<DigikalaProductDTO>("گوشی موبایل","سامسونگ").Result;
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
                List<string> aspect = new List<string> { "باتری#مدت شارژ", "باتری#کیفیت", "صفحه نمایش" };
                listNegative.ItemsSource = aspect;
                listNeutral.ItemsSource = aspect;
                listPositive.ItemsSource = aspect;
            }
        }
    }
}
