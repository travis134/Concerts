using System;

namespace Concerts
{
    public class Artist
    {
        public String Name { get; set; }
        public String Url { get; set; }
        public String Mbid { get; set; }
        public int Upcoming_Events_Count { get; set; }

        public Artist() { }
        public Artist(String name, String url, String mbid, int upcoming_events_count)
        {
            this.Name = name;
            this.Url = url;
            this.Mbid = mbid;
            this.Upcoming_Events_Count = upcoming_events_count;
        }
    }
}
