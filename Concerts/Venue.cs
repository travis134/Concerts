using System;

namespace Concerts
{
    public class Venue
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String Country { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public Venue() { }
        public Venue(int id, String name, String url, String city, String region, String country, float latitude, float longitude)
        {
            this.Id = id;
            this.Name = name;
            this.Url = url;
            this.City = city;
            this.Region = region;
            this.Country = country;
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}
