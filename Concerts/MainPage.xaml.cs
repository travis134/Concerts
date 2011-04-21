using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Xml;
using System.ServiceModel.Syndication;
using System.IO;
using System.Windows.Media.Imaging;
using System.Threading;
using System.ComponentModel;
using Microsoft.Phone.Tasks;

namespace Concerts
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        List<Concert> concerts;
        Thread imageLoader;
        List<BitmapImage> images;
        int imageIndex;

        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }

            concerts = new List<Concert>();
            images = new List<BitmapImage>();

            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                for (int i = 0; i < 20; i++)
                {
                    RowDefinition Row = new RowDefinition();
                    Row.Height = new GridLength(0, GridUnitType.Auto);
                    ImageGrid.RowDefinitions.Add(Row);

                    RowDefinition Padding = new RowDefinition();
                    Padding.Height = new GridLength(20);
                    ImageGrid.RowDefinitions.Add(Padding);
                }
            });

            try
            {
                WebClient infoClient = new WebClient();
                infoClient.DownloadStringAsync(new Uri("http://www.tourfilter.com/newyork/rss/by_concert_date"));
                infoClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(request_DownloadConcertInfo);

                
            }
            catch (WebException we)
            {
                System.Diagnostics.Debug.WriteLine("Error: \n " + we.Message);
            }

        }

        void request_DownloadConcertInfo(object sender,
            DownloadStringCompletedEventArgs e)
        {
            XmlReader reader = XmlReader.Create(new StringReader(e.Result));
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            concerts.Clear();
            
            foreach (SyndicationItem si in feed.Items)
            {
                concerts.Add(new Concert(si.Title.Text.ToString(), si.Links[0].Uri));
            }
            ConcertInfoList.ItemsSource = concerts;

            imageLoader = new Thread(new ThreadStart(imageLoaderThread));
            imageLoader.Start();
        }

        void imageLoaderThread()
        {
            for (int i = 0; i<concerts.Count; i++)
            {
                WebClient imageClient = new WebClient();
                imageClient.DownloadStringAsync(new Uri("http://www.degraeve.com/flickr-rss/rss.php?tags=" + concerts[i].Band + " concert&tagmode=all&sort=interestingness-desc&num=1"));
                imageClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(request_DownloadConcertImages);
            }
        }

        void request_DownloadConcertImages(object sender,
            DownloadStringCompletedEventArgs e)
        {
            XmlReader reader = XmlReader.Create(new StringReader(e.Result));
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            foreach (SyndicationItem si in feed.Items)
            {
                if (images.Count >= 20)
                {
                    break;
                }
                //images.Add(new BitmapImage(new Uri(si.Id.ToString())));
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Image image = new Image() { Source = new BitmapImage(new Uri(si.Id.ToString())) };
                    image.Stretch = Stretch.UniformToFill;
                    Grid.SetRow(image, imageIndex * 2);
                    ImageGrid.Children.Add(image);
                });
                imageIndex++;
            }
        }

       
        private void ConcertInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.URL = concerts[ConcertInfoList.SelectedIndex].Link.ToString();
            task.Show();
        }


    }
}