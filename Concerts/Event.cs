using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Concerts
{
    public class Event : IEquatable<Event>, IComparable
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

        [XmlIgnoreAttribute]
        public String ArtistName
        {
            get
            {
                return this.Artists.First<Artist>().Name;
            }
        }
        [XmlIgnoreAttribute]
        public String EventUrl
        {
            get
            {
                String result = this.Ticket_Url;
                if (this.Ticket_Url.Equals(String.Empty) || this.Ticket_Status.Equals("unavailable", StringComparison.OrdinalIgnoreCase))
                {
                    result = this.Url;
                }
                return result;
            }
        }
        [XmlIgnoreAttribute]
        public String EventDate
        {
            get
            {

                int daysUntil = (this.DateTime - DateTime.Today).Days;
                String result = "in " + daysUntil.ToString() + " more days, on ";
                if (daysUntil < 0)
                {
                    result = "A few days ago";
                }
                else if (daysUntil == 0)
                {
                    result = "Today";
                }
                else if (daysUntil == 1)
                {
                    result = "Tomorrow";
                }
                else
                {
                    result += this.DateTime.Month.ToString() + "/" + this.DateTime.Day.ToString();
                }

                return result;
            }
        }

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

        int IComparable.CompareTo(object obj)
        {
            Event e = (Event)obj;
            if (this.Id > e.Id)
            {
                return 1;
            }
            else if (this.Id < e.Id)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public bool Equals(Event e)
        {
            if (e != null && this.Id == e.Id)
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
