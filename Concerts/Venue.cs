﻿using System;
using System.Xml.Serialization;

namespace Concerts
{
    public class Venue : IEquatable<Venue>, IComparable
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Url { get; set; }
        public String City { get; set; }
        public String Region { get; set; }
        public String Country { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        [XmlIgnoreAttribute]
        public String CityAndRegion
        {
            get
            {
                return this.City + ", " + this.Region;
            }
        }

        [XmlIgnoreAttribute]
        public String ShortName
        {
            get
            {
                String result = this.Name;
                if (result.Length > 30)
                {
                    result = result.Substring(0, 30) + "...";
                }
                return result;
            }
        }

        [XmlIgnoreAttribute]
        public String ShortNameUpper
        {
            get
            {
                return this.ShortName.ToUpper();
            }
        }

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

        int IComparable.CompareTo(object obj)
        {
            Venue v = (Venue)obj;
            if (this.Id > v.Id)
            {
                return 1;
            }
            else if (this.Id < v.Id)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public bool Equals(Venue v)
        {
            if (v != null && this.Id == v.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
