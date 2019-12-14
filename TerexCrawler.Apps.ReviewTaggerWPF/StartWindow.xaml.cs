using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TerexCrawler.Models.DTO;
using TerexCrawler.Apps.ReviewTaggerWPF.Helpers;

namespace TerexCrawler.Apps.ReviewTaggerWPF
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            AuthResponse authResponse = new AuthResponse();
            if (!string.IsNullOrEmpty(txtUsername.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                UserDTO param = new UserDTO() { Username = txtUsername.Text.Trim(), Password = txtPassword.Text.Trim() };
                using (var Api = new WebAppApiCall())
                {
                    authResponse = Api.GetFromApi<AuthResponse>("Auth", param);
                }
                // var user = users.Where(x => x.Username.ToLower() == txtUsername.Text.ToLower().Trim() && x.Password.ToLower() == txtPassword.Text.ToLower().Trim()).FirstOrDefault();
                if (authResponse != null && authResponse.Success)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.user = authResponse.User;
                    mainWindow.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("please, try again", "Re Try", MessageBoxButton.OK);
                }
            }
        }
    }
}
