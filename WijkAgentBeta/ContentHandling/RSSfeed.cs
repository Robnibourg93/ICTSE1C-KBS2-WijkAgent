using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

namespace WijkAgentBeta.ContentHandling
{



    class RSSfeed
    {
        /*
            Author: Ronne Timmerman
            Modified: Ronne Timmerman
            Description: gets data form RSSfeed and processes it into data that can be handeld by the arc_reports class.

        */
        public string url { get; set; }
        string title;
        string description;
        string pubDate;
        double longitude;
        double latitude;
        string longitudeString;
        string latitudeString;
        string exception;

        XElement channel;
        public void getRSSfeed()
        {
            //Console.WriteLine("RSS stream started");
            try
            {

                url = "http://feeds.livep2000.nl/";

                XElement Rssfeed = XElement.Load(url);
                this.channel = Rssfeed.Element("channel");
            }
            catch (Exception e)
            {
                this.exception = Convert.ToString(e);
            }


            foreach (XElement item in this.channel.Elements())
            {
                if (item.Name == "item")
                {
                    XElement titleX = item.Element("title");
                    XElement descriptionX = item.Element("description");
                    XElement pubDateX = item.Element("pubDate");

                    XNamespace my = "http://www.w3.org/2003/01/geo/wgs84_pos#";
                    XName nodenameLong = my + "long";
                    XName nodenameLat = my + "lat";


                    XElement longitudeX = item.Element(nodenameLong);
                    XElement latitudeX = item.Element(nodenameLat);

                    try
                    {
                        longitudeString = longitudeX.Value;
                        latitudeString = latitudeX.Value;

                        //coverting longitude string into Double
                        string[] longitudesplit = longitudeString.Split('.');
                        double count = longitudesplit[1].Count();

                        double magnitude = Math.Pow(10, count);

                        longitude = Convert.ToDouble(longitudeString);
                        longitude = longitude / magnitude;
                        //end of converting

                        //converting latitude string into Double
                        string[] latitudesplit = latitudeString.Split('.');
                        count = latitudesplit[1].Count();
                        magnitude = Math.Pow(10, count);

                        latitude = Convert.ToDouble(latitudeString);
                        latitude = latitude / magnitude;
                        //end of converting                       
                    }
                    catch (Exception e)
                    {
                        string ex = Convert.ToString(e);
                    }

                    title = titleX.Value;
                    description = descriptionX.Value;
                    pubDate = pubDateX.Value;

                    arc_reports report = new arc_reports(title, description, pubDate, latitude, longitude);
                    report.saveToDataBase();
                }
            }
        }
    }
}
