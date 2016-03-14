using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using Microsoft.Maps.MapControl.WPF;
using System.Diagnostics;
using System.Device.Location;

using WijkAgentBeta.ContentHandling;
using System.Security;
using System.Windows.Input;

//Gps coordinateWatcher made my Julian;
//Map loading and processing made by Rob;

namespace WijkAgentBeta
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Author: Rob Nibourg
    /// Edited by:
    /// Description: 
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Attributes
        GeoCoordinateWatcher watcher;
        MapController mapController;
        HeatmapController heatMapController;
        UserController userController = new UserController();
        AuthenticationController auth = new AuthenticationController();

        public static Location location;
        public TweetController tweetController;
        public Pushpin pin = new Pushpin();
        public Pushpin heatmappin = new Pushpin();
        public TwitterStream stream;

        //different screens
        MainUserControl mainWindow = new MainUserControl();
        AuthenticationUserControl logInScreen = new AuthenticationUserControl();

        private ChatController chat = new ChatController();

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        #endregion

        #region Constructors

        public MainWindow()
        {
            mainWindow.btnAddTileLayer.Click += btnAddTileLayer_Click;
            mainWindow.btnRemoveTileLayer.Click += btnRemoveTileLayer_Click;
            logInScreen.LoginButton.Click += LoginButton_Click;
            mainWindow.LogOutButton.Click += LogOutButton_Click;
            GetLocationEvent();
            InitializeComponent();
            this.Width = 325;
            this.Height = 425;

            pageTransitionControl.ShowPage(logInScreen);

            location = new Location();


            Connection.DnsTest();
            if (Connection.internetConnection == false)
            {
                //Console.WriteLine("No internet connection available.");
                MessageBox.Show("Geen internetconnectie");
            }
            else
            {
                Connection.Connect();
                stream = new TwitterStream();

            }
        }
        #endregion

        //start background proccess controller, for heavy background proccess work
        private void startBackgroundproccessController()
        {
            BackgroundProcessesController BPC = new BackgroundProcessesController();
            BPC.StartBackgroundProcesses();
        }

        #region GPS handling
        public void GetLocationEvent()
        {
            this.watcher = new GeoCoordinateWatcher();
            //This method creates an event if a changed position is noticed by the GeoCoordinatewatcher.
            this.watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);

            //This is where the application is booted. If the watcher can not be booted there will be a pop-up.
            bool started = this.watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
            if (!started)
            {
                //Console.WriteLine("GeoCoordinateWatcher timed out on start.");
            }
        }

        //This method checkes whether your position has been changed. If yes, it will pass the coordinates to the watcher.
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (AuthenticationController.loggedInUser != null)
            {
                AuthenticationController.loggedInUser.latitude = e.Position.Location.Latitude;
                AuthenticationController.loggedInUser.longitude = e.Position.Location.Longitude;
                userController.updateUser(AuthenticationController.loggedInUser);
            }
            PrintPosition(e.Position.Location.Latitude, e.Position.Location.Longitude);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            SecureString pass = logInScreen.PassText.SecurePassword;
            string code = logInScreen.codeText.Text;
            if (auth.logIn(code, pass))
            {

                logInScreen.label.Opacity = 0;
                logInScreen.label1.Opacity = 0;
                logInScreen.codeText.Opacity = 0;
                logInScreen.PassText.Opacity = 0;
                logInScreen.LoginButton.Opacity = 0;

                if (logInScreen.LoginButton.Opacity == 0)
                {

                    this.Height = 500;
                    this.Width = 1112;

                    pageTransitionControl.ShowPage(mainWindow);
                    mapController = new MapController(mainWindow.map);
                    mainWindow.Heatmap.Map.KeyDown += new KeyEventHandler(Map_ZoomIn);
                    mainWindow.Heatmap.Map.KeyDown += new KeyEventHandler(Map_ZoomOut);
                    heatMapController = new HeatmapController(mainWindow.Heatmap);
                    mapController.RefreshMap();
                    mainWindow.map.Map.Center = pin.Location;

                }
            }
            else
            {
                logInScreen.ErrorLabel.Visibility = Visibility.Visible;
            }
        }


        public void Map_ZoomIn(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Add)
            {
                //Console.WriteLine("ZoomIn");
                HeatmapController.zoomLevel++;
                if (HeatmapController.zoomLevel < 4)
                {
                    HeatmapController.gridSize = HeatmapController.gridSize / 1.0;
                }
                else {
                    HeatmapController.gridSize = HeatmapController.gridSize / 1.4;
                }
                heatMapController.RefreshMap();
                //Console.WriteLine(HeatmapController.gridSize + " " + HeatmapController.zoomLevel);
            }

        }

        public void Map_ZoomOut(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Subtract)
            {

                //Console.WriteLine("ZoomOut");
                HeatmapController.zoomLevel--;
                if (HeatmapController.zoomLevel < 4)
                {
                    HeatmapController.gridSize = HeatmapController.gridSize * 1.0;
                }
                else {
                    HeatmapController.gridSize = HeatmapController.gridSize * 1.4;
                }
                heatMapController.RefreshMap();
                //Console.WriteLine(HeatmapController.gridSize + " " + HeatmapController.zoomLevel);
            }
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {

            AuthenticationController.isLogged = false;
            AuthenticationController.loggedInUser.availability = UserController.notLogged;
            userController.updateUser(AuthenticationController.loggedInUser);

            logInScreen.codeText.Text = "";
            logInScreen.PassText.Clear();


            logInScreen.label.Opacity = 100;
            logInScreen.label1.Opacity = 100;
            logInScreen.codeText.Opacity = 100;
            logInScreen.PassText.Opacity = 100;
            logInScreen.LoginButton.Opacity = 100;

            this.Width = 325;
            this.Height = 425;

            AuthenticationController.loggedInUser = null;

            pageTransitionControl.ShowPage(logInScreen);
        }

        public void btnAddTileLayer_Click(object sender, RoutedEventArgs e)
        {
            // Add the tile overlay on the map, if it doesn't already exist.
            if (heatMapController.tileLayer != null)
            {
                if (!mainWindow.Heatmap.Map.Children.Contains(heatMapController.tileLayer))
                {
                    mainWindow.Heatmap.Map.Children.Add(heatMapController.tileLayer);
                }
            }
            else
            {
                heatMapController.AddTileOverlay();
            }
        }

        public void btnRemoveTileLayer_Click(object sender, RoutedEventArgs e)
        {
            // Removes the tile overlay if it has been added to the map.
            if (mainWindow.Heatmap.Map.Children.Contains(heatMapController.tileLayer))
            {
                mainWindow.Heatmap.Map.Children.Remove(heatMapController.tileLayer);
            }
        }





        //This is where the gps coordinates get shown in the application.
        public void PrintPosition(double Latitude, double Longitude)
        {
            //This method sets a comma from the coordinates to a period which makes the double able to be used by the GeoCoordinateWatcher.
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            Thread.CurrentThread.CurrentCulture = customCulture;

            location.Longitude = Longitude;
            location.Latitude = Latitude;
            mainWindow.Heatmap.Map.Center = (location);
            pin.Location = location;
            heatmappin.Location = location;

            mainWindow.map.Map.Children.Remove(pin);
            mainWindow.Heatmap.Map.Children.Remove(pin);

            // Get the current agent's location, needed for the tweetController to get the tweets within the range of the agent.
            try
            {

                //tweetController = new TweetController(mainWindow.map);
                //tweetController.locationAgent = location;
                //tweetController.maxLatitude = tweetController.locationAgent.Latitude + 1;
                //tweetController.maxLongitude = tweetController.locationAgent.Longitude - 1;


                // Creates a query to execute for getTweets(); NOTE: in the future it needs to check all the possible subject(s) <-- To:DO list.
                //string query = tweetController.subjectQuery("Politie");

                // Get tweets.
                //tweetController.getTweets(query);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
            //Console.WriteLine("Latitude: {0}, Longitude {1}", Latitude, Longitude);
        }
        #endregion

    }
}
