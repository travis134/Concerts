using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Concerts
{
    public class Concert
    {
        protected String pattern;

        protected String Title { get; set; }
        public String Band { get; set; }
        public DateTime Date { get; set; }
        public Uri Link { get; set; }
        public String DateString
        {
            get
            {
                
                int daysUntil = (this.Date - DateTime.Today).Days;
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
                    result += this.Date.Month.ToString() + "/" + this.Date.Day.ToString();
                }

                return result;
            }
        }
        public String Location { get; set; }

        public Concert() { }
        public Concert(String title, Uri link)
        {
            this.Title = title;
            this.Band = "No Name";
            this.Date = DateTime.Today;
            this.Link = link;
            this.Location = "No Location";

            this.pattern = "(?<band>.+) \\((?<location>.+) (?<month>[0-9]+)/(?<day>[0-9]+)\\)";
            Regex concertRegex = new Regex(this.pattern);
            Match m = concertRegex.Match(this.Title);
            if (m.Success)
            {
                int month, day;
                this.Band = m.Groups["band"].Value.ToString();
                this.Location = m.Groups["location"].Value.ToString();
                if (int.TryParse(m.Groups["month"].Value.ToString(), out month) && int.TryParse(m.Groups["day"].Value.ToString(), out day))
                {
                    this.Date = new DateTime(DateTime.Today.Year, month, day);   
                }
            }
        }
    }
}
