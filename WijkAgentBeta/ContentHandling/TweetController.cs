using Microsoft.Maps.MapControl.WPF;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using Tweetinvi.Core.Interfaces;
using WijkAgentBeta.Database;

namespace WijkAgentBeta.ContentHandling
{
    public class TweetController
    {
        /*
            Author: Simon Brink
            Modified: Simon Brink & Joshua van Gelder

            Class: TweetController.cs
            Description: TweetController is the main controller for tweets in the database. This class checks continously if there are any importants tweets in a short duration.
            If this is true the TweetController creates a notice for the officer on the map. To do this there must be a valid connection with the database.
            Internet is required here.
        */

        // Variables tweetController
        #region Attributes
        dbConnection dbConnection = new dbConnection();
        MySqlDataReader reader;
        MySqlCommand command;

        string query;
        public string subject;
        public int idTweet;
        public double latitude = 0.0;
        public double longitude = 0.0;

        public Location locationLoggedUser { get; set; }
        public Location searchRange { get; set; }
        #endregion

        #region constructors
        public TweetController()
        {

        }
        #endregion

        #region tweetLists
        public List<TweetMessage> getAllTweetLocations()
        {
            //List to hold all the tweet locations
            List<TweetMessage> tweetLocationList = new List<TweetMessage>();
            //Query for selecting the locations from the tweetlist
            query = "SELECT idTweet,longitude,latitude FROM tweet WHERE longitude != 0 && latitude != 0;";
            //Bind query into a command
            command = new MySqlCommand(query, dbConnection.GetConnection());
            try
            {
                //execute query and store results in the reader
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //get values with column index
                    int locationListId = (int)reader.GetValue(0);
                    double locationListLat = (double)reader.GetValue(1);
                    double locationListLong = (double)reader.GetValue(2);
                    //create new user
                    TweetMessage tweet = new TweetMessage(locationListId, locationListLat, locationListLong);
                    //add user to list
                    tweetLocationList.Add(tweet);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            reader.Close();
            dbConnection.CloseConnection();
            return tweetLocationList;
        }

        public List<TweetMessage> getTweetLastDay()
        {
            //List to hold all the tweet locations
            List<TweetMessage> tweetLocationList = new List<TweetMessage>();
            //Query for selecting the locations from the tweetlist
            query = "SELECT idTweet,longitude,latitude,IsRead FROM tweet WHERE longitude != 0 && latitude != 0 AND createdAt >= DATE_SUB(CURDATE(), INTERVAL 1 DAY) AND CURDATE()";
            //Bind query into a command
            command = new MySqlCommand(query, dbConnection.GetConnection());
            try
            {
                //execute query and store results in the reader
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    //get values with column index
                    int locationListId = (int)reader.GetValue(0);
                    double locationListLat = (double)reader.GetValue(1);
                    double locationListLong = (double)reader.GetValue(2);
                    //create new user
                    TweetMessage tweet = new TweetMessage(locationListId, locationListLat, locationListLong);
                    tweet.IsRead = (int)reader.GetValue(3);
                    //add user to list
                    tweetLocationList.Add(tweet);
                }
            }
            catch (MySqlException se)
            {
                Debug.WriteLine(se);
            }
            reader.Close();

            return tweetLocationList;
        }
        #endregion

        #region splitTweet
        // Variables splitTweet
        public void splitTweet(ITweet tweet)
        {
            string[] splitTweet = tweet.Text.Split(' ');
            foreach (string word in splitTweet)
            {
                getSubject(word);
            }
        }
        #endregion


        #region getTweetSubject    
        // Function to get the subject, and add it to the database.   
        public void getSubject(string word)
        {
            if (TwitterStream.searchWords.Contains(word))
            {
                subject = word;
            }
        }
        #endregion

        #region saveTweet

        // The process to save the tweet to the database.
        public void saveTweet(ITweet tweet)
        {
            try
            {

                command = dbConnection.GetConnection().CreateCommand();
                command.Connection = dbConnection.GetConnection();

                // Check if the tweet has an location. (necessary).
                checkLocationTweet(tweet);
                // add value to the parameters.
                command.Parameters.AddWithValue("@createdAt", tweet.CreatedAt);
                command.Parameters.AddWithValue("@user", tweet.CreatedBy);
                command.Parameters.AddWithValue("@message", tweet.Text);
                command.Parameters.AddWithValue("@language", tweet.Language.ToString());
                command.Parameters.AddWithValue("@subject", subject);
                // Execute query
                command.ExecuteNonQuery();
                // Tweet saved to database.
                //Console.WriteLine("Tweet saved to database.");
                // Close connection.
                dbConnection.CloseConnection();
            }
            catch (MySqlException me)
            {
                Debug.WriteLine(me);
            }
        }
        #endregion

        #region checkLocationTweet
        // Check if a tweet has coordinates. 
        public void checkLocationTweet(ITweet tweet)
        {


            bool locationIsSet = true;

            try
            {
                latitude = tweet.Coordinates.Latitude;
                longitude = tweet.Coordinates.Longitude;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                locationIsSet = false;
            }

            command.CommandText = "INSERT INTO tweet (longitude, latitude, message, createdBy, createdAt, language, subject) VALUES (@lontitude, @latitude, @message, @user, @createdAt, @language, @subject)";

            if (locationIsSet)
            {
                command.Parameters.AddWithValue("@lontitude", tweet.Coordinates.Longitude);
                command.Parameters.AddWithValue("@latitude", tweet.Coordinates.Latitude);
            }
            else
            {
                command.Parameters.AddWithValue("@lontitude", longitude);
                command.Parameters.AddWithValue("@latitude", latitude);
            }
        }

        #endregion

        #region getSearchWords

        // Get searchwords that are active or inactive
        public List<string> getSearchWords(int active)
        {
            List<string> list = new List<string>();
            // Set the query and execute it.
            string query = "SELECT word FROM searchword WHERE active = " + active + "";
            command = new MySqlCommand(query, dbConnection.GetConnection());
            // Trying to execute the query
            try
            {
                reader = command.ExecuteReader();

                while (reader.Read())
                {

                    string word = reader.GetValue(0).ToString();
                    list.Add(word);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            // returns a list with words that are active or inactive.
            reader.Close();
            dbConnection.CloseConnection();
            return list;
        }
        #endregion

        #region saveSearchWord

        // Save new searchWord to the database.
        public void saveSearchWord(string word)
        {
            // Create connection and command.
            command = dbConnection.GetConnection().CreateCommand();
            command.Connection = dbConnection.GetConnection();

            command.CommandText = "INSERT INTO searchword (word, active) VALUES (@word, @active)";

            // Add value to the parameters.
            command.Parameters.AddWithValue("@word", word);
            command.Parameters.AddWithValue("@active", 0);

            // Execute query and close the database connection.
            command.ExecuteNonQuery();
            dbConnection.CloseConnection();
        }
        #endregion

        #region checkIfSearchWordExist
        // Check if searchWord already exist. We don't want duplicates in our database!
        public Boolean checkIfSearchWordExist(string word)
        {
            string query = "SELECT word FROM searchword WHERE word = '" + word + "'";
            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();
            // If word already exist returns false.
            if (reader.Read() == false)
            {
                reader.Close();
                dbConnection.CloseConnection();
                return false;
            }
            // If word does not exist returns true.
            else
            {
                reader.Close();
                dbConnection.CloseConnection();
                return true;
            }
        }
        #endregion

        #region changeSearchWord
        // Change searchword to active or inactive.
        public void changeSearchWord(int active, string word)
        {
            string query = "UPDATE searchword SET active = " + active + " WHERE word = '" + word + "'";
            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();
            // Close reader and database connection.
            reader.Close();
            dbConnection.CloseConnection();
        }
        #endregion

        #region deleteSearchWord
        // Delete searchWord
        public void deleteSearchWord(string word)
        {
            string query = "DELETE FROM searchword WHERE word = '" + word + "'";
            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();
            // Close reader and database connection.
            reader.Close();
            dbConnection.CloseConnection();
        }
        #endregion

        #region getTweets
        // Get single tweet
        public TweetMessage getTweetById(int id)
        {
            // Create query and command.
            string query = "SELECT  longitude, latitude,  createdAt, createdBy,message FROM tweet WHERE idTweet=" + id;
            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();

            TweetMessage tweetMessage = new TweetMessage(0d, 0d, 0);

            // While executing query.
            while (reader.Read())
            {
                double longitude = (double)reader.GetValue(0);
                double latitude = (double)reader.GetValue(1);
                DateTime datetime = (DateTime)reader.GetValue(2);
                string username = (string)reader.GetValue(3);
                string message = (string)reader.GetValue(4);

                tweetMessage = new TweetMessage(message, username, longitude, latitude, datetime);

                return tweetMessage;
            }

            // Return tweet and close reader and database connection.
            reader.Close();
            dbConnection.CloseConnection();

            return tweetMessage;
        }

        // Get tweets that are not older then 12 hours. 
        public List<TweetMessage> getTweets()
        {
            // Create TweetMessage List.
            List<TweetMessage> list = new List<TweetMessage>();

            // Create query and command.
            string query = "SELECT subject, longitude, latitude, idTweet, createdAt, isRead FROM tweet WHERE createdAt >= NOW() - Interval 12 HOUR";
            command = new MySqlCommand(query, dbConnection.GetConnection());
            reader = command.ExecuteReader();

            // Try executing query.
            try
            {
                while (reader.Read())
                {
                    double longitude = (double)reader.GetValue(1);
                    double latitude = (double)reader.GetValue(2);
                    int idTweet = (int)reader.GetValue(3);
                    DateTime datetime = (DateTime)reader.GetValue(4);

                    TweetMessage tweetMessage = new TweetMessage(longitude, latitude, idTweet);
                    tweetMessage.IsRead = (int)reader.GetValue(5);

                    list.Add(tweetMessage);
                }

            }
            catch (IndexOutOfRangeException ie)
            {
                // Datetime is not in range. Catching the execption.
                Debug.WriteLine(ie);
            }


            // Return list and close reader and database.
            reader.Close();
            dbConnection.CloseConnection();
            return list;
        }

        public List<TweetMessage> getTweetsForFeed()
        {
            List<TweetMessage> list = new List<TweetMessage>();

            try
            {
                string query = "SELECT createdBy, message, createdAt FROM tweet WHERE createdAt >= NOW() - Interval 12 HOUR";
                command = new MySqlCommand(query, dbConnection.GetConnection());
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string createdBy = reader.GetValue(0).ToString();
                    string message = reader.GetValue(1).ToString();
                    DateTime createdAt = (DateTime)reader.GetValue(2);

                    TweetMessage tweetMessage = new TweetMessage(createdBy, message, createdAt);
                    list.Add(tweetMessage);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            reader.Close();
            dbConnection.CloseConnection();
            return list;
        }

        // check if tweet already exists.
        public bool checkIfTweetAlreadyExists(ITweet tweet)
        {
            try
            {
                // Query and command.
                string query = "SELECT message FROM tweet WHERE message = '" + tweet.Text + "'";
                command = new MySqlCommand(query, dbConnection.GetConnection());
                reader = command.ExecuteReader();
                // Return true or false. 
                return reader.Read();

            }
            catch (Exception ex)
            {
                // for some reason sometimes it will have an exception.
                Debug.WriteLine(ex);
                return true;
            }

        }
        #endregion

        #region MoveTweetToHistory

        public void MoveTweetToHistory(TweetMessage tweet)
        {

            TweetMessage DBtweet = tweet;

            //get all tweet info
            try
            {
                //the query
                query = "SELECT langitude,longitude,createdAt,language,message,createdBy,subject,idTweet FROM tweet WHERE idTweet = " + tweet.idTweet;
                //bind query
                command = new MySqlCommand(query, dbConnection.GetConnection());
                //execute query and store results in the reader
                reader = command.ExecuteReader();
                //loop through results
                while (reader.Read())
                {
                    //get values with column index
                    double lang = (double)reader.GetValue(0);
                    double lon = (double)reader.GetValue(1);
                    DateTime createdAt = (DateTime)reader.GetValue(2);
                    string language = (string)reader.GetValue(3);
                    string message = (string)reader.GetValue(4);
                    string createdBy = (string)reader.GetValue(5);
                    string subject = (string)reader.GetValue(6);
                    int id = (int)reader.GetValue(7);

                    DBtweet = new TweetMessage(message, createdBy, subject, language, lon, lang, createdAt, id);
                }
                reader.Close();
                dbConnection.CloseConnection();
            }
            catch (MySqlException sqlE)
            {
                Debug.WriteLine(sqlE);
            }

            //insert into history table
            try
            {
                command = dbConnection.GetConnection().CreateCommand();
                command.Connection = dbConnection.GetConnection();

                command.CommandText = "INSERT INTO tweethistory (tweetId,longitude, latitude, message, createdBy, createdAt, language, subject) VALUES (@tweetId, @lontitude, @latitude, @message, @user, @createdAt, @language, @subject)";

                command.Parameters.AddWithValue("@tweetId", DBtweet.idTweet);
                command.Parameters.AddWithValue("@createdAt", DBtweet.createdAt);
                command.Parameters.AddWithValue("@lontitude", DBtweet.longitude);
                command.Parameters.AddWithValue("@latitude", DBtweet.latitude);
                command.Parameters.AddWithValue("@user", DBtweet.createdBy);
                command.Parameters.AddWithValue("@message", DBtweet.message);
                command.Parameters.AddWithValue("@language", DBtweet.language.ToString());
                command.Parameters.AddWithValue("@subject", DBtweet.subject);
                command.ExecuteNonQuery();

                dbConnection.CloseConnection();
            }
            catch (MySqlException sqlE)
            {
                Debug.WriteLine(sqlE);
            }

            //delete tweet uit tweet table
            try
            {
                query = "DELETE FROM tweet WHERE idTweet=" + tweet.idTweet;
                command = dbConnection.GetConnection().CreateCommand();
                command.CommandText = query;
                command.Connection = dbConnection.GetConnection();
                command.ExecuteNonQuery();

                dbConnection.CloseConnection();

            }
            catch (MySqlException e)
            {
                Debug.WriteLine(e);
            }
        }

        #endregion

    }
}
