using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;
using WijkAgentBeta.View;
using System.Device.Location;


using System.Threading.Tasks;

namespace WijkAgentBeta.ContentHandling
{
    public class HeatmapController
    {
        /*
            Author: Julian van 't veld and Melvin van de Velde
            Modified: Julian van 't veld and Melvin van de Velde
            Description: Creates heatmap. Sets the grid for the heatmap to cluster pins and show heatpoints on the map.

        */

        //Author Julian van 't Veld
        //Gets distance between pushpins
        public double GetDistance()
        {
            var firstCordinate = new GeoCoordinate(52.505183, 6.090095);
            var secondCordinate = new GeoCoordinate(52.337342, 5.617812);

            double distance = firstCordinate.GetDistanceTo(secondCordinate);
            return distance;
        }

        public void PrintDistance()
        {
            //Console.WriteLine(GetDistance() + "hallo Dit is de afstand");
        }


        //Here is the data being recieved and prepared for usage within this class.
        public MapUserControl heatmap;
        List<Pushpin> alertPins = new List<Pushpin>();
        List<Pushpin> tweetPins = new List<Pushpin>();
        List<Pushpin> dynamicPins = new List<Pushpin>();

        AlertController alerts = new AlertController();
        TweetController tweetController = new TweetController();
        public static int zoomLevel = 7;
        public static double gridSize = 0.3;
        public MapTileLayer tileLayer;
        private double tileOpacity = 0.50;


        public HeatmapController(MapUserControl heatmap)
        {
            this.heatmap = heatmap;
            heatmap.Map.ZoomLevel = zoomLevel;
            heatmap.Map.CredentialsProvider = new ApplicationIdCredentialsProvider("AiVpDjx_UzOiXEXS0ubyP-lhdirTxU-vlRJbcdqSNZNQbSbtCcGk4h7h3ZqJaBDW");
            PrintDistance();
            RefreshMap();
            addNewPolyline();


        }

        //Refreshed the map, repositions the pins and grid according to the zoom-level.
        public void RefreshMap()
        {
            heatmap.Map.Children.Clear();
            addNewPolyline();
            //Console.WriteLine("Refresh Map");
            CheckGridSquares();


        }

        //Author Melvin van de Velde
        //Draws gridlines for the heatmap
        void addNewPolyline()
        {
            double altitudeMax = 53.73;
            double altitudeMin = 50.63;
            double longtitudeMin = 3.32;
            double longtitudeMax = 7.30;


            while (altitudeMin < altitudeMax)
            {
                MapPolyline polyline = new MapPolyline();
                polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                polyline.StrokeThickness = 1;
                polyline.Opacity = 1;
                polyline.Locations = new LocationCollection() {
                new Location(altitudeMin ,3.32),
                new Location(altitudeMin,7.30) };

                altitudeMin = altitudeMin + gridSize;

            }

            while (longtitudeMin < longtitudeMax)
            {
                MapPolyline polyline = new MapPolyline();
                polyline.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                polyline.StrokeThickness = 1;
                polyline.Opacity = 1;
                polyline.Locations = new LocationCollection() {
                new Location(53.73 ,longtitudeMin),
                new Location(50.63,longtitudeMin) };

                longtitudeMin = longtitudeMin + gridSize;

            }
        }

        //Author Julian van 't Veld
        public void AddTileOverlay()
        {

            // Create a new map layer to add the tile overlay to.
            tileLayer = new MapTileLayer();

            // The source of the overlay.
            TileSource tileSource = new TileSource();
            tileSource.UriFormat = "{UriScheme}://ecn.t0.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g=129&mkt=en-us&shading=hill&stl=H";

            // Add the tile overlay to the map layer
            tileLayer.TileSource = tileSource;


            // Add the map layer to the map
            if (!heatmap.Map.Children.Contains(tileLayer))
            {
                heatmap.Map.Children.Add(tileLayer);
            }
            tileLayer.Opacity = tileOpacity;
        }



        //add user pins to the map
        public void addTwitterPins()
        {
            List<TweetMessage> tweetList = tweetController.getTweetLastDay();

            //creating a pin foreach user
            foreach (TweetMessage tweet in tweetList)
            {
                Location tweetLocation = new Location(tweet.latitude, tweet.longitude);
                Pushpin twitterpin = new Pushpin();

                twitterpin.Uid = tweet.idTweet.ToString();
                twitterpin.Location = tweetLocation;
                twitterpin.Background = new SolidColorBrush(Color.FromRgb(0, 220, 255));

                tweetPins.Add(twitterpin);
            }

            //add pins to the map
            foreach (Pushpin pin in tweetPins)
            {
                heatmap.Map.Children.Add(pin);
            }

        }

        //add alert pins to the map
        public void addAlertPins()
        {
            List<Alert> LAlerts = alerts.alertListPastDay();
            List<Pushpin> OldPins = alertPins;
            //creating a pin foreach alert
            foreach (Alert alert in LAlerts)
            {
                Location alertLocation = new Location(alert.latitude, alert.longitude);
                Pushpin alertpin = new Pushpin();

                alertpin.Uid = alert.id.ToString();
                alertpin.Location = alertLocation;
                alertpin.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                //Console.WriteLine(alert.report + " Services:" + alert.services + " Location " + alertLocation);
                if (!alertPins.Contains(alertpin))
                {
                    alertpin.Content = "New";
                    alertpin.FontSize = 8;
                }
                alertPins.Add(alertpin);
            }


        }

        public void removeAlertPins()
        {
            foreach (Pushpin pin in alertPins)
            {
                pin.IsEnabled = false;

            }
        }


        //Main method to locate pins and draw squares in the right color according to the amount of pins within a gridcell.
        public void CheckGridSquares()
        {
            List<GeoCoordinate> Coordinates = new List<GeoCoordinate>();



            var basepoint = new Location(53.73, 3.33);
            var square = new Location();
            double verschilLong;
            double verschilLat;
            double vakjesLong;
            double vakjesLat;

            int vakjesLongInt;
            int vakjesLatInt;
            List<TweetMessage> tweetlist = tweetController.getTweetLastDay();

            /* The coordinates from the Tweets are being changed to round numbers. This is so the heat map is drawn at the right place as we work with round numbers.
            After the coordinates are round they are added to the chekDoubleCoordinates.*/
            foreach (TweetMessage tweet in tweetlist)
            {

                verschilLong = (tweet.longitude - basepoint.Longitude) / gridSize;
                verschilLat = (basepoint.Latitude - tweet.latitude) / gridSize;

                vakjesLong = Math.Round(verschilLong, 1, MidpointRounding.AwayFromZero);
                vakjesLat = Math.Round(verschilLat, 1, MidpointRounding.AwayFromZero);

                vakjesLongInt = Convert.ToInt32(vakjesLong);
                vakjesLatInt = Convert.ToInt32(vakjesLat);

                double altitudeMin = 53.71 - (vakjesLatInt * gridSize);
                double longtitudeMin = 3.32 + (vakjesLongInt * gridSize);
                Coordinates.Add(new GeoCoordinate() { Latitude = altitudeMin, Longitude = longtitudeMin });

            }

            /* The coordinates from the alerts are being changed to round numbers. This is so the heat map is drawn at the right place as we work with round numbers.
            After the coordinates are round they are added to the chekDoubleCoordinates.*/

            List<Alert> LAlerts = alerts.alertListPastDay();

            foreach (Alert alert in LAlerts)
            {

                verschilLong = (alert.longitude - basepoint.Longitude) / gridSize;
                verschilLat = (basepoint.Latitude - alert.latitude) / gridSize;

                vakjesLong = Math.Round(verschilLong, 1, MidpointRounding.AwayFromZero);
                vakjesLat = Math.Round(verschilLat, 1, MidpointRounding.AwayFromZero);

                vakjesLongInt = Convert.ToInt32(vakjesLong);
                vakjesLatInt = Convert.ToInt32(vakjesLat);


                double altitudeMin = 53.71 - (vakjesLatInt * gridSize);
                double longtitudeMin = 3.32 + (vakjesLongInt * gridSize);
                Coordinates.Add(new GeoCoordinate() { Latitude = altitudeMin, Longitude = longtitudeMin });

            }

            /* This is where the coordinates are being checked from alerts and tweets.
            The amount of tweets and alerts in a certain area decides the color of the area.*/

            List<GeoCoordinate> Coordinates2 = new List<GeoCoordinate>();
            foreach (GeoCoordinate coordinate in Coordinates)
            {
                int aantalAlerts = 1;
                foreach (GeoCoordinate coordinate2 in Coordinates2)
                {

                    if (coordinate2.Latitude == coordinate.Latitude && coordinate2.Longitude == coordinate.Longitude)
                    {
                        aantalAlerts++;
                    }
                }

                //Fills the squares for the gridcells for the right amount of pins detected.
                MapPolygon polygon = new MapPolygon();
                if (aantalAlerts == 1)
                {
                    polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                }
                else if (aantalAlerts == 2)
                {
                    polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Orange);
                }
                else
                {
                    polygon.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                }

                polygon.StrokeThickness = 1;
                polygon.Opacity = 0.5;
                polygon.Locations = new LocationCollection() {



              new Location((coordinate.Latitude + gridSize) ,coordinate.Longitude),
              new Location(coordinate.Latitude,coordinate.Longitude),
              new Location(coordinate.Latitude,(coordinate.Longitude + gridSize)),
              new Location((coordinate.Latitude + gridSize),(coordinate.Longitude + gridSize))};
                if (aantalAlerts < 4)
                {
                    heatmap.Map.Children.Add(polygon);
                }
                Coordinates2.Add(new GeoCoordinate() { Latitude = coordinate.Latitude, Longitude = coordinate.Longitude });
            }
        }

    }
}















