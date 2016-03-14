using Microsoft.Maps.MapControl.WPF;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using WijkAgentBeta.ContentHandling;
using System.Threading;
using System;
using WijkAgentBeta.Models;
using System.Media;
using System.Windows.Input;
using WijkAgentBeta.View;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Windows.Controls;

namespace WijkAgentBeta.ContentHandling
{
    public class MapController
    {
        /*
            Author: Rob Nibourg
            Modified: Joshua van Gelder, Simon Brink and Ronne Timerman
        */
        #region attributes
        //Map user control
        MapUserControl map;
        //Pin lists
        List<Pushpin> userPins = new List<Pushpin>();
        List<Pushpin> alertPins = new List<Pushpin>();
        List<Pushpin> tweetPins = new List<Pushpin>();
        List<Location> oldTweetPins = new List<Location>();
        List<Location> oldAlertPins = new List<Location>();
        List<Pushpin> dynamicPins = new List<Pushpin>();
        //Data components
        UserController Users = new UserController();
        AlertController alerts = new AlertController();
        TweetController tweetController = new TweetController();
        PanicController panic = new PanicController();

        SoundPlayer audio = new SoundPlayer(WijkAgentBeta.Properties.Resources.Nuke_Alert_Sound); // here WindowsFormsApplication1 is the namespace and Connect is the audio file name
        //User pin
        Pushpin Userpin = new Pushpin();
        Location userLoc = new Location();

        Boolean PanicActive = false;

        System.Media.SoundPlayer player = new System.Media.SoundPlayer();

        public static int zoomLevel = 15;
        public static bool userTrackingOn = true;
        public static bool userPinsActive = true;
        public static bool twitterPinsActive = true;
        public static bool alertPinsActive = true;
        #endregion

        #region Contructors
        //Made by Rob Nibourg
        public MapController(MapUserControl map)
        {
            this.map = map;
            map.Map.ZoomLevel = zoomLevel;
            map.Map.CredentialsProvider = new ApplicationIdCredentialsProvider("AiVpDjx_UzOiXEXS0ubyP-lhdirTxU-vlRJbcdqSNZNQbSbtCcGk4h7h3ZqJaBDW");
            RefreshMap();

            Thread BPCTread = new Thread(startBackgroundproccessController);
            BPCTread.IsBackground = true;
            BPCTread.Start();

        }
        #endregion

        #region Map interaction
        //reload all pins
        //Made by Rob Nibourg
        public void RefreshMap()
        {
            //clear the map
            map.Map.Children.Clear();
            //center the map
            centerMap();
            //add all the pins            
            if (userPinsActive)
            {
                addUserPins();
            }
            if (alertPinsActive)
            {
                addNewAlertPins();
            }
            if (twitterPinsActive)
            {
                addTwitterPins();
            }

            //check if user has logged in and change pin accordingly
            if (AuthenticationController.loggedInUser != null)
            {
                userLoc.Latitude = AuthenticationController.loggedInUser.latitude;
                userLoc.Longitude = AuthenticationController.loggedInUser.longitude;
                Userpin.Location = userLoc;
            }
            map.Map.Children.Add(Userpin);
        }

        //Made by Rob Nibourg
        private void centerMap()
        {
            if (!panic.checkPanic() && userTrackingOn)
            {
                map.Map.Center = MainWindow.location;
            }
            if (panic.checkPanic())
            {
                Location panicLoc = new Location();
                panicLoc.Latitude = panic.panicModel.latitude;
                panicLoc.Longitude = panic.panicModel.longitude;
                map.Map.Center = panicLoc;
            }
        }
        #endregion

        #region Load pins Section
        //Made by Rob Nibourg
        public void addUserPins()
        {
            //create list with all available users in UserController
            List<User> LUsers = Users.getAllAvailableUsers();
            userPins.Clear();

            //create a pin foreach user
            foreach (User user in LUsers)
            {
                Location userLocation = new Location(user.latitude, user.longitude);

                if (AuthenticationController.isLogged)
                {
                    if (user.id != AuthenticationController.loggedInUser.id)
                    {
                        Pushpin userpin = new Pushpin();

                        userpin.Uid = user.id.ToString();
                        userpin.Location = userLocation;
                        userpin.MouseDown += Userpin_MouseDown;
                        userpin.Background = new SolidColorBrush(Color.FromRgb(0, 0, 143));

                        userPins.Add(userpin);
                    }
                }
             
            }

            //add pins to the map
            foreach (Pushpin pin in userPins)
            {

                map.Map.Children.Add(pin);
            }

        }

        //Made by Rob Nibourg
        public void addTwitterPins()
        {
            //create a list with the method from tweetcontroller
            List<TweetMessage> tweetList = tweetController.getTweets();

            //creating a pin foreach user
            foreach (TweetMessage tweet in tweetList)
            {
                Location tweetLocation = new Location(tweet.latitude, tweet.longitude);
                //check if the tweetpin already exists
                bool alreadyExists = oldTweetPins.Contains(tweetLocation);

                if (alreadyExists == false)
                {
                    Pushpin twitterpin = new Pushpin();

                    twitterpin.Uid = tweet.idTweet.ToString();
                    twitterpin.Location = tweetLocation;
                    twitterpin.Background = new SolidColorBrush(Color.FromRgb(0, 220, 255));

                    if (tweet.IsRead == 0)
                    {
                        twitterpin.Content = "New";
                        twitterpin.FontSize = 8;
                    }

                    oldTweetPins.Add(tweetLocation);
                    twitterpin.MouseDown += Twitterpin_MouseDown;
                    tweetPins.Add(twitterpin);
                    Console.WriteLine("Adding to map..");
                }
            }

            //add pins to the map
            try
            {
                foreach (Pushpin pin in tweetPins)
                {
                    map.Map.Children.Add(pin);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        //Made by Rob Nibourg
        private void Twitterpin_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Open infoBox Method
            Pushpin twitterpin = sender as Pushpin;
            TweetMessage tweet = tweetController.getTweetById(Convert.ToInt32(twitterpin.Uid));

            TwitterPinInfoBox twitterPinInfoBox = new TwitterPinInfoBox();

            twitterPinInfoBox.twitterMessageBox.Text = tweet.message;
            twitterPinInfoBox.createdByLabel.Content = "Geplaatst door: " + tweet.createdBy;
            twitterPinInfoBox.dateTimeLabel.Content = "Datum: " + tweet.createdAt.ToString();

            twitterPinInfoBox.Show();

        }
        //Made by Rob Nibourg
        private void Userpin_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //Open infoBox Method
            Pushpin userpin = sender as Pushpin;
            User user = Users.getUserById(Convert.ToInt32(userpin.Uid));

            UserPinInfoBox userPinInfoBox = new UserPinInfoBox();
            string availability = "";
            if (user.availability == 1) { availability = "Beschikbaar"; }
            if (user.availability == 2) { availability = "Gaat naar melding"; }
            if (user.availability == 3) { availability = "Op locatie"; }
            if (user.availability == 4) { availability = "Bezet"; }

            userPinInfoBox.LastUpdateLabel.Content = "Voor het Laast bijgewerkt op: " + user.lastUpdate.ToString();
            userPinInfoBox.NameLabel.Content = "Naam: " + user.name;
            userPinInfoBox.UserCodeLabel.Content = "Code: " + user.code;
            userPinInfoBox.AvailabilityLabel.Content = "Beschikbaarheid: " + availability;

            userPinInfoBox.Show();

        }
        //Made by Rob Nibourg
        private void Alertpin_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Open infoBox Method
            Pushpin alertpin = sender as Pushpin;
            Alert alert = alerts.getAlertById(Convert.ToInt32(alertpin.Uid));

            Console.WriteLine(alert.ToString());

            AlertPinInfoBox alertPinInfoBox = new AlertPinInfoBox();

            alertPinInfoBox.MessageTextBox.Text = alert.report;
            alertPinInfoBox.IdLabel.Content = alert.id.ToString();
            alertPinInfoBox.deleteButton.Click += deleteButton_Click;
            alertPinInfoBox.deleteButton.Tag = alert.id.ToString();
            alertPinInfoBox.ServiceLabel.Content = "Service: " + alert.services;
            alertPinInfoBox.DateLabel.Content = "Datum: " + alert.date.ToString();

            alertPinInfoBox.Show();

        }
        //Made by Joshua van Gelder
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            //Messagebox for showing warning message
            Button button = sender as Button;

            MessageBoxResult message = System.Windows.MessageBox.Show("Je staat op het punt om een melding te verwijderen, weet je dit zeker?", "Let op!", MessageBoxButton.YesNo);
            if (message == MessageBoxResult.Yes)
            {
                try
                {
                    removeAlertPinById(Convert.ToInt32(button.Tag.ToString()));
                    MessageBoxResult succes = System.Windows.MessageBox.Show("Succesfully deleted alert with ID: " + button.Tag.ToString(), "", MessageBoxButton.OK);
                }
                catch (MySqlException se)
                {
                    Debug.WriteLine(se);
                    MessageBoxResult error = System.Windows.MessageBox.Show("An error has occured, please contact your system administrator", "", MessageBoxButton.OK);
                }
            }
        }
        //Made by Joshua van Gelder
        public void addNewAlertPins()
        {
            //checks to see if the alerttable has been purged of yesterdays alerts
            if (AlertController.updatedAlertTable == false)
            {
                alerts.updateAlerts();
                alerts.deleteAlerts();
            }
            //initialize new list for holding alerts
            List<Alert> NewAlerts = new List<Alert>();
            //get the alert list from alertcontroller
            NewAlerts = alerts.newAlertList();

            foreach (Alert alert in NewAlerts)
            {
                //create new location for the alertpin
                Location alertLocation = new Location(alert.latitude, alert.longitude);

                //check if the alertpin already exists
                bool ifExtists = oldAlertPins.Contains(alertLocation);

                if (ifExtists == false)
                {
                    Pushpin alertPin = new Pushpin();
                    alertPin.Uid = alert.id.ToString();
                    alertPin.Location = alertLocation;
                    alertPin.MouseDown += Alertpin_MouseDown;
                    alertPin.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                    //if the alert has not been read, put a "new" tag on it
                    if (alert.IsRead == 0)
                    {
                        alertPin.Content = "New";
                        alertPin.FontSize = 8;
                    }
                    //add all alerts to their respective lists
                    oldAlertPins.Add(alertLocation);
                    alertPins.Add(alertPin);
                }
            }
            try
            {
                //add the pins to the map
                foreach (Pushpin pin in alertPins)
                {
                    map.Map.Children.Add(pin);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        //Made by Joshua van Gelder
        public void addAlertPins()
        {
            //checks to see if the alerttable has been purged of yesterdays alerts
            if (AlertController.updatedAlertTable == false)
            {
                alerts.updateAlerts();
                alerts.deleteAlerts();
            }
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
                if (alert.IsRead == 0)
                {
                    alertpin.Content = "New";
                    alertpin.FontSize = 8;
                }
                alertPins.Add(alertpin);
            }
            //adding alert pins to the map
            foreach (Pushpin pin in alertPins)
            {
                map.Map.Children.Add(pin);
            }
        }
        //Made by Joshua van Gelder
        public void removeAlertPins()
        {
            //disable every pin in alertPins
            foreach (Pushpin pin in alertPins)
            {
                pin.IsEnabled = false;
            }
        }
        #endregion

        #region Remove Specific pins section
        //remove alert pin by id
        //Made by Rob Nibourg
        public void removeAlertPinById(int id)
        {
            //loop through list and remove one
            foreach (Pushpin pin in alertPins)
            {
                if (pin.Uid == id.ToString())
                {
                    alertPins.Remove(pin);
                    map.Map.Children.Remove(pin);
                    alerts.moveAlertToHistoryById(id);
                    break;
                }
            }
        }

        //remove User pin by id
        //Made by Rob Nibourg
        public void removeUserPinById(int id)
        {
            //loop through list and remove one
            foreach (Pushpin pin in userPins)
            {
                if (pin.Uid == id.ToString())
                {
                    userPins.Remove(pin);
                    break;
                }
            }
        }

        //remove tweet pin by id
        //Made by Rob Nibourg
        public void removeTweetPinById(int id)
        {
            //loop through list and remove one
            foreach (Pushpin pin in tweetPins)
            {
                if (pin.Uid == id.ToString())
                {
                    tweetPins.Remove(pin);
                    break;
                }
            }
        }
        #endregion
        //made by Ronne Timmerman
        //start background proccess
        private void startBackgroundproccessController()
        {
            BackgroundProcessesController BPC = new BackgroundProcessesController();
            BPC.StartBackgroundProcesses(this);
        }

        // panic button region made by Ronne Timmerman
        //generates information dialog when Panic is recognized
        #region Panicbutton
        public void PanicStart(Panic panic)
        {
            Location panicLocation = new Location(panic.latitude, panic.longitude);

            map.Map.Center = panicLocation;
            map.Map.ZoomLevel = 15;
            if (PanicActive == false)
            {
                playaudio();
                MessageBox.Show("!!PANIC ACTIVATED!!" + "\n" + "uw collega heeft de paniek knop ingedrukt op de locatie zoals nu aangegeven. volg protecol en onderneem actie");
                audio.Stop();
            }

            PanicActive = true;
        }

        public void PanicStop()
        {
            PanicActive = false;
            player.Stop();
        }

        private void playaudio() // defining the function
        {

            audio.Play();
        }

        private void stopaudio()
        {
            audio.Stop();
        }
        #endregion
    }
}
