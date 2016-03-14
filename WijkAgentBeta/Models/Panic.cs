using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgentBeta.Models
{
    public class Panic
    {
        /*
            Author: Ronne Timmerman
            Modified:
            Description: Model for panic sitiuation, when panic button is clicked
        */

        public DateTime DateTime { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int userid { get; set; }

        public Panic(DateTime datetime, double longitude, double latitude, int id)
        {
            this.DateTime = datetime;
            this.longitude = longitude;
            this.latitude = latitude;
            this.userid = id;

            //Console.WriteLine("panic readed from database");

        }

        public Boolean checkIfRelevand()
        {
            Boolean returnvalue = true;

            if ((this.DateTime - DateTime.Now).TotalMinutes > 2)
            {
                returnvalue = false;
                //Console.WriteLine((this.DateTime - DateTime.Now).TotalMinutes);
            }

            return returnvalue;
        }
    }
}
