using System;
using System.Collections.Generic;

namespace Concerts
{
    public class Event
    {
        public int Id { get; set; }
        public String Url { get; set; }
        public DateTime DateTime { get; set; }
        public String Ticket_Url { get; set; }
        public List<Artist> Artists { get; set; }
        public Venue Venue { get; set; }
        public String Status { get; set; }
        public String Ticket_Status { get; set; }
        public DateTime? On_Sale_Date { get; set; }
        public String ArtistName { get; set; }
        public String VenueName { get; set; }

        public Event() { }
        public Event(int id, String url, DateTime dateTime, String ticket_url, List<Artist> artists, Venue venue, String status, String ticket_status, DateTime? on_sale_datetime)
        {
            this.Id = id;
            this.Url = url;
            this.DateTime = dateTime;
            this.Ticket_Url = ticket_url;
            this.Artists = artists;
            this.Venue = venue;
            this.Status = status;
            this.Ticket_Status = ticket_status;
            this.On_Sale_Date = on_sale_datetime;
        }
    }
}
