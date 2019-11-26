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
using System.Linq;
using TerexCrawler.Entites;

namespace TerexCrawler.Apps.ReviewTaggerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Review review = new Review();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            txtReview.Text = "سلام\n خوبی؟";
            List<string> aspect = new List<string> { "باتری#مدت شارژ", "باتری#کیفیت", "صفحه نمایش" };
            listNegative.ItemsSource=aspect;
            listNeutral.ItemsSource=aspect;
            listPositive.ItemsSource=aspect;
        }

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
    }
}
