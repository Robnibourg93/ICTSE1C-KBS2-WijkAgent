using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WijkAgentBeta.ContentHandling
{
    class FeedController
    {
        /* 
             Author: S1079813 - Simon Brink
             Modified by: Simon Brink
             Description: This is the class for the live feed tweets and alerts. 
        */ 
        TweetController tweetController = new TweetController();
        AlertController alertController = new AlertController();

        List<TweetMessage> tweetMessages;
        List<Alert> alertMessages;

        // Get the tweets for the feed
        public List<TweetMessage> tweetFeed()
        {
            tweetMessages = tweetController.getTweetsForFeed();
            return tweetMessages;

        }

        // Get the alerts for the feed
        public List<Alert> alertFeed()
        {
            alertMessages = alertController.getAllAlertsFeed();

            return alertMessages;
        }


    }
}
