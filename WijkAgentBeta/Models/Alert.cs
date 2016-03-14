using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgentBeta.ContentHandling
{
    public class Alert
    {
        /*
            Author: Joshua van Gelder
            Modified by: Ronne Timmerman,Rob Nibourg

            Explain the main thing this class does.
        */
        //attributes
        public int id;
        public double latitude, longitude;
        public string report;
        public string services;
        public int IsRead;
        public DateTime date;

        //default constructor
        public Alert()
        {

        }
        //Made by Joshua van Gelder
        public Alert(string report, double latitude, double longitude, DateTime date)
        {
            this.report = report;
            this.latitude = latitude;
            this.longitude = longitude;
            this.date = date;
        }
        //Made by Rob Nibourg
        public Alert(string report, double latitude, double longitude, DateTime date, string services)
        {
            this.report = report;
            this.latitude = latitude;
            this.longitude = longitude;
            this.date = date;
            this.services = services;
        }
        //Made by Joshua van Gelder
        public Alert(int id, double latitude, double longitude)
        {
            this.id = id;
            this.latitude = latitude;
            this.longitude = longitude;
        }
        //Made by Joshua van Gelder
        public Alert(int id, string alert, string services)
        {
            this.id = id;
            this.report = alert;
            this.services = services;
        }
        //Made by Joshua van Gelder
        public Alert(int id, double latitude, double longitude, string report, string services, DateTime date)
        {
            this.id = id;
            this.latitude = latitude;
            this.longitude = longitude;
            this.report = report;
            this.services = services;
            this.date = date;
        }
    }
}
