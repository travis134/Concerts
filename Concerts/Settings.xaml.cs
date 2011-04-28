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

namespace Event
{
    public partial class SettingsHelper : PhoneApplicationPage
    {
        List<String> cities;
        PhoneApplicationService phoneAppService = PhoneApplicationService.Current;

        public SettingsHelper()
        {
            InitializeComponent();

            ToggleSwitch toggleGps = new ToggleSwitch() { Header = "Enable GPS?", Foreground = new SolidColorBrush(Colors.White) };
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
            ListPicker defaultLocation = new ListPicker() { Header = "Default Location", Foreground = new SolidColorBrush(Colors.White), FullModeItemTemplate = locationsTemplate };
            defaultLocation.SelectionChanged += new SelectionChangedEventHandler(locationChanged);
            defaultLocation.ItemsSource = cities;
            ContentPanel.Children.Add(defaultLocation);


        }

        private void locationChanged(object sender, SelectionChangedEventArgs e)
        {
            phoneAppService.State.Add(new KeyValuePair<string, object>("defaultLocation", ((ListPicker)sender).SelectedItem.ToString()));
        }


    }
}