using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;



namespace GutenbergApp
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            GenerateMainLinks();
        }
        public void GenerateMainLinks()
        {
            var baseUrl = "https://gutendex.com/books/?page=";
            //To Read how many pages we want, And for me i want to read it only 10 pages.
            var pageCount = 2;
            var links = new List<string>();
            for (int i = 1; i <= pageCount; i++)
            {
                links.Add($"{baseUrl}{i}");
            }

            foreach (var link in links)
            {
                DisplayDataFromLink(link);
            }
        }
        public void DisplayDataFromLink(string link)
        {
            var request = (HttpWebRequest)WebRequest.Create(link);
            request.Method = "GET";

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                var responseText = reader.ReadToEnd();
                DisplayInStackPanel(responseText);
            }
        }

        public void DisplayInStackPanel(string text)
        {
            var jsonData = JObject.Parse(text);

            foreach (var book in jsonData["results"])
            {
                var title = book["title"].ToString();
                var author = book["authors"].FirstOrDefault()?["name"].ToString();


                //Passing these 2 URL to another page
                var BookCoverUrl = book["formats"].Value<JToken>("image/jpeg")?.ToString();
                var BookTextUrl = book["formats"].Value<JToken>("text/plain; charset=us-ascii")?.ToString();
                

                var button = new Button
                {
                    Content = $"Title: {title}\nAuthor: {author}", 
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Background = new SolidColorBrush(Colors.Transparent),
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(1),
                    Foreground = new SolidColorBrush(Colors.White),
                    Margin = new Thickness(0, 0, 0, 5),
                    Padding = new Thickness(10, 0, 10, 0),
                    Height = 50,
                    Tag = new BookDetails { BookCoverUrl = BookCoverUrl, BookTextUrl = BookTextUrl }
                };
                button.Click += ViewTextButton;
                LayoutBooks.Children.Add(button);
            }


        }
        private void ViewTextButton(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is BookDetails urls)
            {
                var textDisplayWindow = new TextDisplayWindow(urls.BookCoverUrl, urls.BookTextUrl);
                textDisplayWindow.Show();
            }
        }
    }
}