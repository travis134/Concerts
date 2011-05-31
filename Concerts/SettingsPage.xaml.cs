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
using System.Windows.Markup;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Concerts
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        List<String> cities;
        PhoneApplicationService phoneAppService = PhoneApplicationService.Current;

        public SettingsPage()
        {
            InitializeComponent();

            ToggleSwitch toggleGps = new ToggleSwitch() { Name = "EnableGPSToggleSwitch", Header = "Enable GPS?"};
            ContentPanel.Children.Add(toggleGps);

            #region cities
            cities = new List<String>()
            {
                "Birmingham, Alabama",
                "Mobile, Alabama",
                "Montgomery, Alabama",
                "Anchorage, Alaska",
                "Fairbanks, Alaska",
                "Juneau, Alaska",
                "Phoenix, Arizona",
                "Tucson, Arizona",
                "Mesa, Arizona",
                "Little Rock, Arkansas",
                "Fort Smith, Arkansas",
                "Fayetteville, Arkansas",
                "San Diego, California",
                "San Jose, California",
                "Denver, Colorado",
                "Colorado Springs, Colorado",
                "Bridgeport, Connecticut",
                "Hartford, Connecticut",
                "New Haven, Connecticut",
                "Wilmington, Delaware",
                "Dover, Delaware",
                "Newark, Delaware",
                "Jacksonville, Florida",
                "Miami, Florida",
                "Tallahassee, Florida",
                "Atlanta, Georgia",
                "Augusta, Georgia",
                "Columbus, Georgia",
                "Honolulu, Hawaii",
                "Hilo, Hawaii",
                "Kailua,  Hawaii ",
                "Boise, Idaho",
                "Nampa, Idaho",
                "Idaho Falls, Idaho",
                "Chicago, Illinois",
                "Peoria, Illinois",
                "Rockford, Illinois",
                "Indianapolis, Indiana",
                "Fort Wayne, Indiana",
                "Evansville, Indiana",
                "Des Moines, Iowa",
                "Cedar Rapids, Iowa",
                "Davenport, Iowa",
                "Wichita, Kansas",
                "Overland Park, Kansas",
                "Kansas City, Kansas",
                "Louisville, Kentucky",
                "Lexington, Kentucky",
                "Owensboro, Kentucky",
                "New Orleans, Louisiana",
                "Shreveport, Louisiana",
                "Baton Rouge, Louisiana",
                "Portland, Maine",
                "Bangor, Maine",
                "Baltimore, Maryland",
                "Boston, Massachusetts",
                "Worcester, Massachusetts",
                "Springfield, Massachusetts",
                "Grand Rapids, Michigan",
                "Minneapolis, Minnesota",
                "St. Paul,  Minnesota",
                "Jackson, Mississippi",
                "Gulfport, Mississippi",
                "Biloxi, Mississippi",
                "Kansas City, Missouri",
                "Billings, Montana",
                "Omaha, Nebraska",
                "Lincoln, Nebraska",
                "Las Vegas, Nevada",
                "Reno, Nevada",
                "Henderson, Nevada",
                "Manchester, New Hampshire",
                "Concord, New Hampshire",
                "Newark, New Jersey",
                "Jersey City, New Jersey",
                "Albuquerque, New Mexico",
                "Las Cruces, New Mexico",
                "Rio Rancho, New Mexico",
                "Buffalo, New York",
                "Rochester, New York",
                "Charlotte, North Carolina",
                "Raleigh, North Carolina",
                "Greensboro, North Carolina",
                "Fargo, North Dakota",
                "Bismarck, North Dakota",
                "Grand Forks, North Dakota",
                "Columbus, Ohio",
                "Cleveland, Ohio",
                "Cincinnati, Ohio",
                "Oklahoma City, Oklahoma",
                "Tulsa, Oklahoma",
                "Norman, Oklahoma",
                "Portland, Oregon",
                "Salem, Oregon",
                "Eugene, Oregon",
                "Philadelphia, Pennsylvania",
                "Pittsburgh, Pennsylvania",
                "Allentown, Pennsylvania",
                "Providence, Rhode Island",
                "Cranston, Rhode Island",
                "Columbia, South Carolina",
                "Charleston, South Carolina",
                "Sioux Falls, South Dakota",
                "Rapid City, South Dakota",
                "Memphis, Tennessee",
                "Nashville, Tennessee",
                "Knoxville, Tennessee",
                "Dallas, Texas",
                "San Antonio, Texas",
                "Salt Lake City, Utah",
                "Provo, Utah",
                "Burlington, Vermont",
                "Virginia Beach, Virginia",
                "Norfolk, Virginia",
                "Chesapeake, Virginia",
                "Seattle, Washington",
                "Tacoma, Washington",
                "Spokane, Washington",
                "Charleston, West Virginia",
                "Milwaukee, Wisconsin",
                "Madison, Wisconsin",
                "Green Bay, Wisconsin",
                "Cheyenne, Wyoming",
                "Casper, Wyoming",
                "Laramie, Wyoming"
            };
            #endregion cities
            DataTemplate locationsTemplate = (DataTemplate)XamlReader.Load("<DataTemplate xmlns='http://schemas.microsoft.com/client/2007'><TextBlock Text=\"{Binding}\" FontSize=\"52\"/></DataTemplate>");
            ListPicker defaultLocation = new ListPicker() { Name = "DefaultLocationListPicker", Header = "Default Location", FullModeItemTemplate = locationsTemplate };
            defaultLocation.ItemsSource = cities;
            ContentPanel.Children.Add(defaultLocation);

            Button aboutButton = new Button() { Name = "AboutButton", Content = "About"};
            aboutButton.Click += Button_Click;
            ContentPanel.Children.Add(aboutButton);


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            ToggleSwitch tempToggleSwitch = (ToggleSwitch)this.FindName("EnableGPSToggleSwitch");
            if (IsolatedStorageSettings.ApplicationSettings.Contains("enableGPS"))
            {
                IsolatedStorageSettings.ApplicationSettings["enableGPS"] = tempToggleSwitch.IsChecked;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("enableGPS", tempToggleSwitch.IsChecked);
            }

            ListPicker tempListPicker = (ListPicker)this.FindName("DefaultLocationListPicker");
            if (IsolatedStorageSettings.ApplicationSettings.Contains("defaultLocation"))
            {
                IsolatedStorageSettings.ApplicationSettings["defaultLocation"] = tempListPicker.SelectedItem;
            }
            else
            { 
                IsolatedStorageSettings.ApplicationSettings.Add("defaultLocation", tempListPicker.SelectedItem);
            }

            base.OnNavigatedFrom(e);

        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            ListPicker tempListPicker = (ListPicker) this.FindName("DefaultLocationListPicker");
            object tempLocation;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("defaultLocation", out tempLocation))
            {
                tempListPicker.SelectedItem = (String)tempLocation;
            }

            ToggleSwitch tempToggleSwitch = (ToggleSwitch)this.FindName("EnableGPSToggleSwitch");
            object tempEnableGPS;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue("enableGPS", out tempEnableGPS))
            {
                tempToggleSwitch.IsChecked = (Boolean)tempEnableGPS;
            }
            base.OnNavigatedTo(e);
        }


    }
}