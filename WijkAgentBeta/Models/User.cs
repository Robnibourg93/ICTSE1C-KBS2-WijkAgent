using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgentBeta.ContentHandling
{
    //User model
    public class User
    {
        /*
            Author: Rob Nibourg
            Modified:
            Description: User model
        */

        #region attributes
        //public attributes
        public int id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public int availability { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public DateTime lastUpdate;

        public const int noLocation = 6;
        #endregion

        #region Contructors
        public User()
        {

        }

        public User(string name, string code)
        {
            this.name = name;
            this.code = code;
        }

        public User(int id, string code, string name, int availability, double latitude, double longitude)
        {
            this.id = id;
            this.code = code;
            this.name = name;
            this.availability = availability;
            this.latitude = latitude;
            this.longitude = longitude;
        }
        #endregion

        #region checks
        //check if location is available
        //Made by Rob Nibourg
        public bool checkLocation()
        {

            //check if latitude is nothing, if so location is not available
            if (this.latitude == 0)
            {
                this.availability = noLocation;
            }

            //check if longitde is nothing, if so location is not available
            if (this.longitude == 0)
            {
                this.availability = noLocation;
            }

            //check abailability, if status is noLocation, then return false.
            if (this.availability == noLocation)
            {
                return false;
            }
            else {
                return true;
            }
        }
        #endregion
        //Made by Rob Nibourg
        public override string ToString()
        {
            return "Name: " + name + " Code: " + code;
        }
    }
}
