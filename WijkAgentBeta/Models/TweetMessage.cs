using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgentBeta.ContentHandling
{
    public class TweetMessage
    {

        /*
            TweetMessage Class.
            Author:  Simon Brink.
            Modified by: Simon Brink.
            Discription:
            Every tweet that we get from the database will be transformed to a TweetMessage. TweetMessage class is important to many functions. The class itself contains a couple of constructors and attributes.

        */
        public int idTweet { get; set; }
        public string message { get; set; }
        public string createdBy { get; set; }
        public string subject { get; set; }
        public double longitude { get; set; }
        public string language { get; set; }
        public double latitude { get; set; }
        public DateTime createdAt { get; set; }
        public string placeName { get; set; }
        public int IsRead;

        public TweetMessage(string createdBy, string message, DateTime createdAt)
        {
            this.createdBy = createdBy;
            this.message = message;
            this.createdAt = createdAt;
        }

        public TweetMessage(double longitude, double latitude, int idTweet)
        {
            this.longitude = longitude;
            this.latitude = latitude;
            this.idTweet = idTweet;
        }

        public TweetMessage(string message, string createdBy, DateTime createdAt, string placeName)
        {
            this.message = message;
            this.createdBy = createdBy;
            this.createdAt = createdAt;
            this.placeName = placeName;

        }

        public TweetMessage(int idTweet, double longitude, double latitude)
        {
            this.idTweet = idTweet;
            this.longitude = longitude;
            this.latitude = latitude;
        }

        public TweetMessage(string message, string subject, double longitude, double latitude)
        {
            this.message = message;
            this.subject = subject;
            this.longitude = longitude;
            this.latitude = latitude;

            this.createdBy = "Onbekend";
            this.createdAt = DateTime.Now;
        }


        public TweetMessage(string message, string createdBy, string subject, double longitude, double latitude, DateTime createdAt)
        {
            this.message = message;
            this.createdBy = createdBy;
            this.subject = subject;
            this.longitude = longitude;
            this.latitude = latitude;
            this.createdAt = createdAt;
        }

        public TweetMessage(string message, string createdBy, double longitude, double latitude, DateTime createdAt)
        {
            this.message = message;
            this.createdBy = createdBy;
            this.subject = subject;
            this.longitude = longitude;
            this.latitude = latitude;
            this.createdAt = createdAt;
        }

        public TweetMessage(string message, string createdBy, string subject, string language, double longitude, double latitude, DateTime createdAt, int id)
        {
            this.message = message;
            this.createdBy = createdBy;
            this.subject = subject;
            this.longitude = longitude;
            this.language = language;
            this.latitude = latitude;
            this.createdAt = createdAt;
            this.idTweet = id;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Verzonden door: " + createdBy);
            sb.AppendLine("Bericht: " + message);
            sb.AppendLine("Verzonden op: " + createdAt);
            return sb.ToString();
        }
    }
}
