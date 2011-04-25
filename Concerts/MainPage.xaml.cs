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

namespace Concerts
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<Event> events;
        List<Artist> artists;
        List<Venue> venues;
        GeoCoordinateWatcher watcher;

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            events = new List<Event>();
            artists = new List<Artist>();
            venues = new List<Venue>();

            
            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.MovementThreshold = 20;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
            }
            watcher.Start();
        }

        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        System.Diagnostics.Debug.WriteLine("you have this application access to location.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("location is not functioning on this device");
                    }
                    break;

                case GeoPositionStatus.Initializing:
                    System.Diagnostics.Debug.WriteLine("starting");
                    break;

                case GeoPositionStatus.NoData:
                    System.Diagnostics.Debug.WriteLine("no data");
                    break;

                case GeoPositionStatus.Ready:
                    System.Diagnostics.Debug.WriteLine("ready");
                    WebClient eventsClient = new WebClient();
                    eventsClient.OpenReadAsync(new Uri("http://api.bandsintown.com/events/search.xml?location=" + watcher.Position.Location.Latitude.ToString() + "," + watcher.Position.Location.Longitude.ToString() + "&app_id=concerts_wp7"));
                    eventsClient.OpenReadCompleted += new OpenReadCompletedEventHandler(request_DownloadConcertInfo);
                    break;
            }
        }

        public List<Event> parseEvents(XmlReader eventsReader)
        {
            List<Event> events = new List<Event>();
            while (eventsReader.Read())
            {
                if (eventsReader.NodeType == XmlNodeType.Element)
                {
                    switch (eventsReader.Name)
                    {
                        case "artists":
                            System.Diagnostics.Debug.WriteLine("At root: artists");
                            XmlReader artistsReader = eventsReader.ReadSubtree();
                            events.Last<Event>().Artists = parseArtists(artistsReader);
                            events.Last<Event>().ArtistName = events.Last<Event>().Artists.First<Artist>().Name;
                            break;
                        case "venue":
                            System.Diagnostics.Debug.WriteLine("At root: venue");
                            XmlReader venueReader = eventsReader.ReadSubtree();
                            events.Last<Event>().Venue = parseVenue(venueReader);
                            events.Last<Event>().VenueName = events.Last<Event>().Venue.Name;
                            break;
                        case "events":
                            System.Diagnostics.Debug.WriteLine("At root: events");
                            break;
                        case "event":
                            System.Diagnostics.Debug.WriteLine("Found new event");
                            events.Add(new Event());
                            break;
                        case "id":
                            events.Last<Event>().Id = eventsReader.ReadElementContentAsInt();
                            System.Diagnostics.Debug.WriteLine("Found id:" + events.Last<Event>().Id.ToString());
                            break;
                        case "url":
                            events.Last<Event>().Url = eventsReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found url:" + events.Last<Event>().Url.ToString());
                            break;
                        case "datetime":
                            events.Last<Event>().DateTime = eventsReader.ReadElementContentAsDateTime();
                            System.Diagnostics.Debug.WriteLine("Found datetime:" + events.Last<Event>().DateTime.ToString());
                            break;
                        case "ticket_url":
                            events.Last<Event>().Ticket_Url = eventsReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found ticket_url:" + events.Last<Event>().Ticket_Url.ToString());
                            break;
                        case "status":
                            events.Last<Event>().Status = eventsReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found status:" + events.Last<Event>().Status.ToString());
                            break;
                        case "ticket_status":
                            events.Last<Event>().Ticket_Status = eventsReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found ticket_status:" + events.Last<Event>().Ticket_Status.ToString());
                            break;
                        case "on_sale_datetime":
                            events.Last<Event>().On_Sale_Date = ParseNullableDateTime(eventsReader.ReadInnerXml().ToString());
                            System.Diagnostics.Debug.WriteLine("Found one_sale_datetime:" + events.Last<Event>().On_Sale_Date.ToString());
                            break;
                    }
                }
            }
            return events;
        }

        public List<Artist> parseArtists(XmlReader artistsReader)
        {
            List<Artist> artists = new List<Artist>();
            while (artistsReader.Read())
            {
                if (artistsReader.NodeType == XmlNodeType.Element)
                {
                    switch (artistsReader.Name)
                    {
                        case "artist":
                            System.Diagnostics.Debug.WriteLine("Found new artist");
                            artists.Add(new Artist());
                            break;
                        case "name":
                            artists.Last<Artist>().Name = artistsReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found artist name:" + artists.Last<Artist>().Name.ToString());
                            break;
                        case "url":
                            artists.Last<Artist>().Url = artistsReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found artist url:" + artists.Last<Artist>().Url.ToString());
                            break;
                        case "mbid":
                            artists.Last<Artist>().Mbid = artistsReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found artist mbid:" + artists.Last<Artist>().Mbid.ToString());
                            break;
                        case "upcoming_events_count":
                            artists.Last<Artist>().Upcoming_Events_Count = artistsReader.ReadElementContentAsInt();
                            System.Diagnostics.Debug.WriteLine("Found artist upcoming_events_count:" + artists.Last<Artist>().Mbid.ToString());
                            break;
                    }
                }
            }
            return artists;
        }

        public Venue parseVenue(XmlReader venueReader)
        {
            Venue venue = new Venue();
            while (venueReader.Read())
            {
                if (venueReader.NodeType == XmlNodeType.Element)
                {
                    switch (venueReader.Name)
                    {
                        case "id":
                            venue.Id = venueReader.ReadElementContentAsInt();
                            System.Diagnostics.Debug.WriteLine("Found avenue id:" + venue.Id.ToString());
                            break;
                        case "name":
                            venue.Name = venueReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found avenue name:" + venue.Name.ToString());
                            break;
                        case "url":
                            venue.Url = venueReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found avenue url:" + venue.Url.ToString());
                            break;
                        case "city":
                            venue.City = venueReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found avenue city:" + venue.City.ToString());
                            break;
                        case "region":
                            venue.Region = venueReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found avenue region:" + venue.Region.ToString());
                            break;
                        case "country":
                            venue.Country = venueReader.ReadElementContentAsString();
                            System.Diagnostics.Debug.WriteLine("Found avenue country:" + venue.Country.ToString());
                            break;
                        case "latitude":
                            venue.Latitude = venueReader.ReadElementContentAsFloat();
                            System.Diagnostics.Debug.WriteLine("Found avenue latitude:" + venue.Latitude.ToString());
                            break;
                        case "longitude":
                            venue.Longitude = venueReader.ReadElementContentAsFloat();
                            System.Diagnostics.Debug.WriteLine("Found avenue longitude:" + venue.Longitude.ToString());
                            break;
                    }
                }
            }
            return venue;
        }

        void request_DownloadConcertInfo(object sender,
            OpenReadCompletedEventArgs e)
        {
            XmlReader eventsReader = XmlReader.Create(e.Result);
            this.events = this.parseEvents(eventsReader);
            foreach(Event tempEvent in this.events)
            {
                ConcertInfoList.Items.Add(tempEvent);
            }
        }
       
        private void ConcertInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WebBrowserTask task = new WebBrowserTask();
            task.URL = events[ConcertInfoList.SelectedIndex].Ticket_Url;
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