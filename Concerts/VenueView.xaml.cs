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
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Shell;

namespace Concerts
{
    public partial class VenueView : PhoneApplicationPage
    {
        public Venue venue;
        PhoneApplicationService phoneAppService = PhoneApplicationService.Current;

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            object tempVenue;
            phoneAppService.State.TryGetValue("venue", out tempVenue);
            this.venue = (Venue)tempVenue;
            Map venueMap = new Map() { Center = new System.Device.Location.GeoCoordinate(venue.Latitude, venue.Longitude)};
            LayoutRoot.Children.Add(venueMap);
            Grid.SetRow(venueMap, 1);
            ((TextBlock)this.FindName("PageTitle")).Text = this.venue.Name;
        }

        public VenueView()
        {
            InitializeComponent();
            
        }

    }
}