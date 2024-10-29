using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace GutenbergApp
{
    /// <summary>
    /// Interaction logic for TextDisplayWindow.xaml
    /// </summary>
    public partial class TextDisplayWindow : Window
    {
        private string BookTextUrl;
        public TextDisplayWindow(string bookCoverUrl, string bookTextUrl)
        {
            InitializeComponent();
            BookCover.Source = new BitmapImage(new Uri(bookCoverUrl));
            BookTextUrl = bookTextUrl;
            LoadBookText(bookTextUrl);


        }

        private async void LoadBookText(string url)
        {
            using var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);

            // Handle redirection
            if (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Redirect)
            {
                string redirectUrl = response.Headers.Location.ToString();
                response = await client.GetAsync(redirectUrl);
            }

            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();
            BookText.Text = text;
        }
        private async void DownloadBook(object sender, RoutedEventArgs e)
        {
            using var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(BookTextUrl);
            
            // Handle redirection
            if (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Redirect)
            {
                string redirectUrl = response.Headers.Location.ToString();
                response = await client.GetAsync(redirectUrl);
            }

            response.EnsureSuccessStatusCode();
            var text = await response.Content.ReadAsStringAsync();

            // Save the text to a file
            var saveFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"DownloadedBook.txt");
            await System.IO.File.WriteAllTextAsync(saveFilePath, text);

            MessageBox.Show($"Book downloaded to {saveFilePath}");
        }

    }
}
