using Microsoft.Phone.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Net;
using System;
using System.Xml;
using Microsoft.Phone.Tasks;
using System.Windows.Controls;
using System.Linq;
using System.Device.Location;
using System.Windows.Markup;
using Microsoft.Advertising.Mobile.UI;

namespace Concerts
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<Event> events;
        List<Artist> artists;
        List<Venue> venues;
        GeoCoordinateWatcher watcher;
        StorageHelper<Event> eventsStorageHelper;
        StorageHelper<Artist> artistsStorageHelper;
        StorageHelper<Venue> venuesStorageHelper;


        public MainPage()
        {
            InitializeComponent();
            PanoramaItem eventsPage = new PanoramaItem() { Name = "eventsPage", Header = "Upcoming" };
            PanoramaItem artistsPage = new PanoramaItem() { Name = "artistsPage", Header = "Artists" };
            PanoramaItem venuesPage = new PanoramaItem() { Name = "venuesPage", Header = "Venues" };
            mainView.Items.Add(eventsPage);
            mainView.Items.Add(artistsPage);
            mainView.Items.Add(venuesPage);

            events = new List<Event>();
            artists = new List<Artist>();
            venues = new List<Venue>();

            eventsStorageHelper = new StorageHelper<Event>("Events.xml", "Stale.txt");
            artistsStorageHelper = new StorageHelper<Artist>("Artists.xml");
            venuesStorageHelper = new StorageHelper<Venue>("Venues.xml");

            if (eventsStorageHelper.IsStale(new TimeSpan(4, 0, 0)))
            {
                if (watcher == null)
                {
                    watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                    watcher.MovementThreshold = 20;
                    watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                }
                watcher.Start();
            }
            else
            {
                this.events = this.eventsStorageHelper.Load();
                this.artists = this.artistsStorageHelper.Load();
                this.venues = this.venuesStorageHelper.Load();
                this.updateUi();
            }

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Boolean deviceReady = false;
            String url = "http://api.bandsintown.com/events/search.xml?per_page=20&location={0}&app_id=concerts_wp7";
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    break;
                case GeoPositionStatus.Initializing:
                    break;
                case GeoPositionStatus.NoData:
                    break;
                case GeoPositionStatus.Ready:
                    deviceReady = true;
                    url = String.Format(url, watcher.Position.Location.Latitude.ToString() + "," + watcher.Position.Location.Longitude.ToString());
                    break;
            }

            if (!deviceReady)
            {
                url = String.Format(url, "Tempe,AZ");
            }
            WebClient eventsClient = new WebClient();
            eventsClient.OpenReadAsync(new Uri(url));
            eventsClient.OpenReadCompleted += new OpenReadCompletedEventHandler(request_DownloadConcertInfo);
        }

        public List<Event> parseEvents(XmlReader eventsReader)
        {
            List<Event> eventsResult = new List<Event>();
            try
            {
                while (eventsReader.Read())
                {
                    if (eventsReader.NodeType == XmlNodeType.Element)
                    {
                        switch (eventsReader.Name)
                        {
                            case "artists":
                                XmlReader artistsReader = eventsReader.ReadSubtree();
                                eventsResult.Last<Event>().Artists = parseArtists(artistsReader);
                                break;
                            case "venue":
                                XmlReader venueReader = eventsReader.ReadSubtree();
                                eventsResult.Last<Event>().Venue = parseVenue(venueReader);
                                break;
                            case "events":
                                break;
                            case "event":
                                eventsResult.Add(new Event());
                                break;
                            case "id":
                                eventsResult.Last<Event>().Id = eventsReader.ReadElementContentAsInt();
                                break;
                            case "url":
                                eventsResult.Last<Event>().Url = eventsReader.ReadElementContentAsString();
                                break;
                            case "datetime":
                                eventsResult.Last<Event>().DateTime = eventsReader.ReadElementContentAsDateTime();
                                break;
                            case "ticket_url":
                                eventsResult.Last<Event>().Ticket_Url = eventsReader.ReadElementContentAsString() + "?affil_code=concertswp7";
                                break;
                            case "status":
                                eventsResult.Last<Event>().Status = eventsReader.ReadElementContentAsString();
                                break;
                            case "ticket_status":
                                eventsResult.Last<Event>().Ticket_Status = eventsReader.ReadElementContentAsString();
                                break;
                            case "on_sale_datetime":
                                eventsResult.Last<Event>().On_Sale_Date = ParseNullableDateTime(eventsReader.ReadInnerXml().ToString());
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return eventsResult;
        }

        public List<Artist> parseArtists(XmlReader artistsReader)
        {
            List<Artist> artistsResults = new List<Artist>();
            try
            {
                while (artistsReader.Read())
                {
                    if (artistsReader.NodeType == XmlNodeType.Element)
                    {
                        switch (artistsReader.Name)
                        {
                            case "artist":
                                artistsResults.Add(new Artist());
                                break;
                            case "name":
                                artistsResults.Last<Artist>().Name = artistsReader.ReadElementContentAsString();
                                break;
                            case "url":
                                artistsResults.Last<Artist>().Url = artistsReader.ReadElementContentAsString();
                                break;
                            case "mbid":
                                artistsResults.Last<Artist>().Mbid = artistsReader.ReadElementContentAsString();
                                break;
                            case "upcoming_events_count":
                                artistsResults.Last<Artist>().Upcoming_Events_Count = artistsReader.ReadElementContentAsInt();
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return artistsResults;
        }

        public Venue parseVenue(XmlReader venueReader)
        {
            Venue venueResult = new Venue();
            try
            {
                while (venueReader.Read())
                {
                    if (venueReader.NodeType == XmlNodeType.Element)
                    {
                        switch (venueReader.Name)
                        {
                            case "id":
                                venueResult.Id = venueReader.ReadElementContentAsInt();
                                break;
                            case "name":
                                venueResult.Name = venueReader.ReadElementContentAsString();
                                break;
                            case "url":
                                venueResult.Url = venueReader.ReadElementContentAsString();
                                break;
                            case "city":
                                venueResult.City = venueReader.ReadElementContentAsString();
                                break;
                            case "region":
                                venueResult.Region = venueReader.ReadElementContentAsString();
                                break;
                            case "country":
                                venueResult.Country = venueReader.ReadElementContentAsString();
                                break;
                            case "latitude":
                                venueResult.Latitude = venueReader.ReadElementContentAsFloat();
                                break;
                            case "longitude":
                                venueResult.Longitude = venueReader.ReadElementContentAsFloat();
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return venueResult;
        }

        private void updateUi()
        {
            DataTemplate eventsListItemTemplate = (DataTemplate)XamlReader.Load(
                "<DataTemplate xmlns='http://schemas.microsoft.com/client/2007'>"
                + "<StackPanel Orientation=\"Vertical\">"
                + "<TextBlock Text=\"{Binding ArtistName}\" TextWrapping=\"Wrap\" FontSize=\"48\"/>"
                + "<StackPanel Orientation=\"Horizontal\">"
                + "<TextBlock Text=\"{Binding EventDate}\" TextWrapping=\"Wrap\" FontSize=\"22\"/>"
                + "<TextBlock Text=\" at \" TextWrapping=\"Wrap\" FontSize=\"22\"/>"
                + "<TextBlock Text=\"{Binding Venue.Name}\" TextWrapping=\"Wrap\" FontSize=\"22\"/>"
                + "</StackPanel>"
                + "</StackPanel>"
                + "</DataTemplate>");

            ListBox eventsListBox = new ListBox() { Name = "eventsListBox", ItemTemplate = eventsListItemTemplate };
            eventsListBox.Items.Clear();
            foreach (Event tempEvent in this.events)
            {
                eventsListBox.Items.Add(tempEvent);
            }

            eventsListBox.SelectionChanged += new SelectionChangedEventHandler(eventsListBox_SelectionChanged);

            DataTemplate artistsListItemTemplate = (DataTemplate)XamlReader.Load(
                "<DataTemplate xmlns='http://schemas.microsoft.com/client/2007'>"
                + "<TextBlock Text=\"{Binding Name}\" TextWrapping=\"Wrap\" FontSize=\"48\"/>"
                + "</DataTemplate>");

            ListBox artistsListBox = new ListBox() { Name = "artistsListBox", ItemTemplate = artistsListItemTemplate };
            artistsListBox.Items.Clear();
            foreach (Artist tempArtist in this.artists)
            {
                artistsListBox.Items.Add(tempArtist);
            }

            artistsListBox.SelectionChanged += new SelectionChangedEventHandler(artistsListBox_SelectionChanged);

            DataTemplate venuesListItemTemplate = (DataTemplate)XamlReader.Load(
                "<DataTemplate xmlns='http://schemas.microsoft.com/client/2007'>"
                + "<StackPanel Orientation=\"Vertical\">"
                + "<TextBlock Text=\"{Binding Name}\" TextWrapping=\"Wrap\" FontSize=\"48\"/>"
                + "<StackPanel Orientation=\"Horizontal\">"
                + "<TextBlock Text=\"{Binding City}\" TextWrapping=\"Wrap\" FontSize=\"22\"/>"
                + "<TextBlock Text=\", \" TextWrapping=\"Wrap\" FontSize=\"22\"/>"
                + "<TextBlock Text=\"{Binding Region}\" TextWrapping=\"Wrap\" FontSize=\"22\"/>"
                + "</StackPanel>"
                + "</StackPanel>"
                + "</DataTemplate>");

            ListBox venuesListBox = new ListBox() { Name = "venuesListBox", ItemTemplate = venuesListItemTemplate };
            venuesListBox.Items.Clear();
            foreach (Venue tempVenue in this.venues)
            {
                venuesListBox.Items.Add(tempVenue);
            }
            

            venuesListBox.SelectionChanged += new SelectionChangedEventHandler(venuesListBox_SelectionChanged);

            PanoramaItem eventsPage = (PanoramaItem)this.FindName("eventsPage");
            PanoramaItem artistsPage = (PanoramaItem)this.FindName("artistsPage");
            PanoramaItem venuesPage = (PanoramaItem)this.FindName("venuesPage");

            eventsPage.Content = eventsListBox;
            artistsPage.Content = artistsListBox;
            venuesPage.Content = venuesListBox;

            AdControl concertsAd = new AdControl() { ApplicationId = "33c7fa47-c859-47f1-8903-f745bf749ce0", AdUnitId = "10016302", Width = 300, Height = 50, Margin = new Thickness(10) };
            LayoutRoot.Children.Add(concertsAd);
            Grid.SetRow(concertsAd, 1);
        }

        void request_DownloadConcertInfo(object sender,
            OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                XmlReader eventsReader = XmlReader.Create(e.Result);
                this.events = this.parseEvents(eventsReader);
                this.artists.Clear();
                this.venues.Clear();
                foreach (Event tempEvent in this.events)
                {
                    foreach (Artist tempArtist in tempEvent.Artists)
                    {
                        if (!this.artists.Contains<Artist>(tempArtist))
                        {
                            this.artists.Add(tempArtist);
                        }
                        
                    }

                    if (!this.venues.Contains<Venue>(tempEvent.Venue))
                    {
                        this.venues.Add(tempEvent.Venue);
                    }

                }

                System.Diagnostics.Debug.WriteLine("Already contains " + this.events.Count);

                if (this.events.Count <= 0)
                {
                    this.events = this.eventsStorageHelper.Load();
                    this.artists = this.artistsStorageHelper.Load();
                    this.venues = this.venuesStorageHelper.Load();
                }
                else
                {
                    this.eventsStorageHelper.Save(this.events);
                    this.artistsStorageHelper.Save(this.artists);
                    this.venuesStorageHelper.Save(this.venues);
                }
            }
            else
            {
                this.events = this.eventsStorageHelper.Load();
                this.artists = this.artistsStorageHelper.Load();
                this.venues = this.venuesStorageHelper.Load();
            }

            this.updateUi();
        }
       
        private void eventsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            ListBox eventsListBox = (ListBox) this.FindName("eventsListBox");
            task.URL = ((Event)eventsListBox.SelectedItem).EventUrl;
            task.Show();
        }

        private void artistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            ListBox artistsListBox = (ListBox)this.FindName("artistsListBox");
            task.URL = ((Artist)artistsListBox.SelectedItem).Url;
            task.Show();
        }

        private void venuesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            ListBox venueListBox = (ListBox)this.FindName("venuesListBox");
            task.URL = ((Venue)venueListBox.SelectedItem).Url;
            task.Show();
        }

        public static DateTime? ParseNullableDateTime(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }
            else
            {
                return DateTime.Parse(s);
            }
        }

    }
}