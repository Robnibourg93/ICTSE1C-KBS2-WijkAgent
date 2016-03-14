using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Credentials;

namespace WijkAgentBeta.ContentHandling
{
    class Connection
    {
        /*
            Author: Simon Brink
            Modified: Simon Brink
            This is the connection towards Twitter. It is bound to a real Twitter account.
            To connect to twitter the api needs four things. ConsumerKey, consumerSecret, accessSecret and accesToken.
        */

        public static string consumerKey = "lOn7QfiPpsVrWRZeyTRJXG7sp";
        public static string consumerSecret = "VzuwKuHD5F4f4uA9E5JF60CmalbkfNsCs8UIRqWxetxn19gkQU";
        public static string accesSecret = "73109885-fIyckgLexHHaPIH4fuCClCkjKc37fVgQqoS83aOBC";
        public static string accesToken = "uFKzXTYipQyQL7Fe2bAtVuE2xc0MK9qvM5KJD3WSQ8WGT";

        public static bool internetConnection;

        // Make connection to twitter.
        public static void Connect()
        {
            var creds = new TwitterCredentials(consumerKey, consumerSecret, accesSecret, accesToken);
            Auth.SetUserCredentials(consumerKey, consumerSecret, accesSecret, accesToken);
            Auth.ApplicationCredentials = creds;
        }

        // Check if the user has internet.
        public static void DnsTest()
        {
            try
            {
                System.Net.IPHostEntry ipHe =
                    System.Net.Dns.GetHostEntry("www.google.com"); // Get DNS from google.
                internetConnection = true; // Succeeded.
            }
            catch
            {
                internetConnection = false; // fail, no internet connection available.                
            }
        }

    }
}
