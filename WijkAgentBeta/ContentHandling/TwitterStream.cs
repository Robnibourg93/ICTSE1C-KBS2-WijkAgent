using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Core.Parameters;
using WijkAgentBeta.Database;

namespace WijkAgentBeta.ContentHandling
{
    public class TwitterStream
    {
        /* 
           Author: Simon Brink
           Modified by: Simon Brink, Melvin van de Velde & Rob Nibourg

           Creates a stream to search for the tweets in a specific location.
           Each tweet that the streams find will be added to a list and displayed in the console.
           This class needs a connection to Twitter, so be sure there is a valid connection.
        */

        IFilteredStream stream = Stream.CreateFilteredStream();
        private dbConnection db = new dbConnection();
        private TweetController tweetController = new TweetController();
        public static List<ITweet> allTweets = new List<ITweet>();
        private string subject { get; set; }

        // Coordinates for Top and Bottom of the Netherlands.
        Coordinates Top = new Coordinates(6.83448, 53.429965);
        Coordinates Bottom = new Coordinates(3.575661, 51.436004);
        public static List<string> searchWords;

        public static TweetMessage tweetMessage;


        public TwitterStream()
        {
            // Create new location and add it to search criteria.
            Location location = new Location(Top, Bottom);
            stream.AddLocation(location);

            Streaming();

        }

        private void Streaming()
        {

            // Whenever a tweet is found the class runs this code.
            stream.MatchingTweetReceived += (sender, args) =>
            {


                searchWords = tweetController.getSearchWords(1);
                // if the tweet found has the language Dutch post it in the console
                string[] splitTweet = args.Tweet.Text.Split(' ');

                if (searchWords.Any(splitTweet.Contains))
                {


                    if (args.Tweet.Coordinates != null && tweetController.checkIfTweetAlreadyExists(args.Tweet) == false)
                    {
                        // Post tweet in Console
                        //Console.WriteLine("\nPosted by: " + args.Tweet.CreatedBy);
                        //Console.WriteLine("Tweet: " + args.Tweet.Text);
                        //Console.WriteLine("Create Date: " + args.Tweet.CreatedAt);
                        //Console.WriteLine("Language: " + args.Tweet.Language.ToString());

                        //Console.WriteLine("Coordinates: " + args.Tweet.Coordinates.Latitude + args.Tweet.Coordinates.Longitude);

                        // split text from the tweet & get a subject
                        tweetController.splitTweet(args.Tweet);


                        tweetMessage = new TweetMessage(args.Tweet.Text, args.Tweet.CreatedBy.ToString(), args.Tweet.CreatedAt, args.Tweet.Place.FullName);
                        // save tweet to database
                        tweetController.saveTweet(args.Tweet);

                    }
                    else
                    {
                        //Console.WriteLine("No coordinates or Tweet already exists.");
                    }


                }
            };
            // Actually start the stream.
            stream.StartStreamMatchingAllConditionsAsync();
        }




    }
}
