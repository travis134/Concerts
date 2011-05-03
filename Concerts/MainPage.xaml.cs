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
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

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
        PhoneApplicationService phoneAppService = PhoneApplicationService.Current;

        public MainPage()
        {
            InitializeComponent();
            PanoramaItem eventsPage = new PanoramaItem() { Name = "eventsPage", Header = "Upcoming" };
            PanoramaItem artistsPage = new PanoramaItem() { Name = "artistsPage", Header = "Artists" };
            PanoramaItem venuesPage = new PanoramaItem() { Name = "venuesPage", Header = "Venues" };
            mainView.Items.Add(eventsPage);
            mainView.Items.Add(artistsPage);
            mainView.Items.Add(venuesPage);

            AdControl concertsAd = new AdControl() { ApplicationId = "33c7fa47-c859-47f1-8903-f745bf749ce0", AdUnitId = "10016302", Width = 300, Height = 50};
            LayoutRoot.Children.Add(concertsAd);
            Grid.SetRow(concertsAd, 1);

            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;
            ApplicationBar.Opacity = 1.0;

            events = new List<Event>();
            artists = new List<Artist>();
            venues = new List<Venue>();

            eventsStorageHelper = new StorageHelper<Event>("Events.xml", "Stale.txt");
            artistsStorageHelper = new StorageHelper<Artist>("Artists.xml");
            venuesStorageHelper = new StorageHelper<Venue>("Venues.xml");

            ApplicationBarIconButton search = new ApplicationBarIconButton(new Uri("/Icons/appbar.feature.search.rest.png", UriKind.Relative));
            search.Text = "search";
            search.Click += new EventHandler(search_Click);

            ApplicationBarIconButton refresh = new ApplicationBarIconButton(new Uri("/Icons/appbar.refresh.rest.png", UriKind.Relative));
            refresh.Text = "refresh";
            refresh.Click += new EventHandler(refresh_Click);

            ApplicationBarIconButton settings = new ApplicationBarIconButton(new Uri("/Icons/appbar.feature.settings.rest.png", UriKind.Relative));
            settings.Text = "settings";
            settings.Click += new EventHandler(settings_Click);

            ApplicationBar.Buttons.Add(refresh);
            ApplicationBar.Buttons.Add(settings);
            ApplicationBar.Buttons.Add(search);

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void search_Click(object sender, EventArgs e)
        {

        }

        void refresh_Click(object sender, EventArgs e)
        {
            refreshInfo();
        }

        void settings_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative));
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (eventsStorageHelper.IsStale(new TimeSpan(4, 0, 0)))
            {
                refreshInfo();
            }
            else
            {
                this.events = this.eventsStorageHelper.LoadAll();
                this.artists = this.artistsStorageHelper.LoadAll();
                this.venues = this.venuesStorageHelper.LoadAll();
                this.updateUi();
            }
        }

        private void refreshInfo()
        {
            object tempGPS;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("enableGPS", out tempGPS);
            if (tempGPS != null && (Boolean)tempGPS)
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
                object tempDefaultLocation;
                IsolatedStorageSettings.ApplicationSettings.TryGetValue("defaultLocation", out tempDefaultLocation);
                fetchInfo((String)tempDefaultLocation);
            }
        }

        private void fetchInfo(String location)
        {
            if (String.IsNullOrEmpty(location))
            {
                location = "Tempe,AZ";
            }
            String url = "http://api.bandsintown.com/events/search.xml?per_page=20&location=" + location + "&app_id=concerts_wp7";
            WebClient eventsClient = new WebClient();
            eventsClient.OpenReadAsync(new Uri(url));
            eventsClient.OpenReadCompleted += new OpenReadCompletedEventHandler(request_DownloadConcertInfo);
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            String location = null;
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    break;
                case GeoPositionStatus.Initializing:
                    break;
                case GeoPositionStatus.NoData:
                    break;
                case GeoPositionStatus.Ready:
                    location = watcher.Position.Location.Latitude.ToString() + "," + watcher.Position.Location.Longitude.ToString();
                    break;
            }
            fetchInfo(location);
        }

        public List<Event> parseEvents(XmlReader eventsReader)
        {
            List<Event> eventsResult = new List<Event>();
            
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
            return eventsResult;
        }

        public List<Artist> parseArtists(XmlReader artistsReader)
        {
            List<Artist> artistsResults = new List<Artist>();
            
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
            
            return artistsResults;
        }

        public Venue parseVenue(XmlReader venueReader)
        {
            Venue venueResult = new Venue();
           
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
            
            return venueResult;
        }

        private void updateUi()
        {
            DataTemplate eventsListItemTemplate = (DataTemplate)XamlReader.Load(
                "<DataTemplate xmlns='http://schemas.microsoft.com/client/2007'>"
                + "<StackPanel Orientation=\"Vertical\">"
                + "<TextBlock Text=\"{Binding ArtistNameUpper}\" TextWrapping=\"Wrap\" FontSize=\"48\" FontWeight=\"Bold\" Foreground=\"White\"/>"
                + "<TextBlock Text=\"{Binding EventDateAndLocation}\" TextWrapping=\"Wrap\" FontSize=\"22\" Foreground=\"#FF1BA1E2\"/>"
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
                + "<TextBlock Text=\"{Binding NameUpper}\" TextWrapping=\"Wrap\" FontSize=\"48\" FontWeight=\"Bold\" Foreground=\"White\"/>"
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
                + "<TextBlock Text=\"{Binding ShortNameUpper}\" TextWrapping=\"Wrap\" FontSize=\"48\" FontWeight=\"Bold\" Foreground=\"White\"/>"
                + "<TextBlock Text=\"{Binding CityAndRegion}\" TextWrapping=\"Wrap\" FontSize=\"22\" Foreground=\"#FFF09609\"/>"
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
                    this.events = this.eventsStorageHelper.LoadAll();
                    this.artists = this.artistsStorageHelper.LoadAll();
                    this.venues = this.venuesStorageHelper.LoadAll();
                }
                else
                {
                    this.eventsStorageHelper.SaveAll(this.events);
                    this.artistsStorageHelper.SaveAll(this.artists);
                    this.venuesStorageHelper.SaveAll(this.venues);
                }
            }
            else
            {
                this.events = this.eventsStorageHelper.LoadAll();
                this.artists = this.artistsStorageHelper.LoadAll();
                this.venues = this.venuesStorageHelper.LoadAll();
            }

            this.updateUi();
        }
       
        private void eventsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.URL = ((Event)((ListBox)sender).SelectedItem).EventUrl;
            task.Show();
        }

        private void artistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            MarketplaceSearchTask marketSearch = new MarketplaceSearchTask();
            marketSearch.ContentType = MarketplaceContentType.Music;
            marketSearch.SearchTerms = ((Artist)((ListBox)sender).SelectedItem).Name;
            marketSearch.Show();
        }

        private void venuesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (phoneAppService.State.ContainsKey("venue"))
            {
                phoneAppService.State["venue"] = ((Venue)((ListBox)sender).SelectedItem);
            }
            else
            {
                phoneAppService.State.Add(new KeyValuePair<string, object>("venue", ((Venue)((ListBox)sender).SelectedItem)));
            }
            this.NavigationService.Navigate(new Uri("/VenueView.xaml", UriKind.Relative));
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