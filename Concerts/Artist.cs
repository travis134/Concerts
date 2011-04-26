using System;
using System.Xml.Serialization;

namespace Concerts
{
    public class Artist : IEquatable<Artist>, IComparable
    {
        public String Name { get; set; }
        public String Url { get; set; }
        public String Mbid { get; set; }
        public int Upcoming_Events_Count { get; set; }

        [XmlIgnoreAttribute]
        public String NameUpper
        {
            get
            {
                return this.Name.ToUpper();
            }
        }

        public Artist() { }
        public Artist(String name, String url, String mbid, int upcoming_events_count)
        {
            this.Name = name;
            this.Url = url;
            this.Mbid = mbid;
            this.Upcoming_Events_Count = upcoming_events_count;
        }

        int IComparable.CompareTo(object obj)
        {
            Artist a = (Artist)obj;
            return String.Compare(this.Mbid, a.Mbid);
        }

        public bool Equals(Artist a)
        {
            if (a != null && this.Mbid == a.Mbid)
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
