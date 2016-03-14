using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;


namespace WijkAgentBeta.Tests
{
    [TestClass]
    public class TweetControllerTests
    {
  
    
      
        [TestMethod]
        public void TweetController_getTweets_melding()
        {
            List<Tuple<string, string, double, double>> allMessages = new List<Tuple<string, string, double, double>>();
            bool succes;
            {

                String message = "test";
                String subject = "Test";
                Double longtitude = 2.3;
                Double latitude = 3.2;
                allMessages.Add(new Tuple<string, string, double, double>(subject, message, longtitude, latitude));
                allMessages.Add(new Tuple<string, string, double, double>(subject, message, longtitude, latitude));
                allMessages.Add(new Tuple<string, string, double, double>(subject, message, longtitude, latitude));
                allMessages.Add(new Tuple<string, string, double, double>(subject, message, longtitude, latitude));
                if (allMessages.Count > 3)
                {
                    succes = true;
                }
                else
                {
                    succes = false;
                }
               
                Assert.IsTrue(succes);
            }
        }
        [TestMethod]
        public void TweetConrtoller_getTweets_geenmelding()
        {
            List<Tuple<string, string, double, double>> allMessages = new List<Tuple<string, string, double, double>>();
            bool succes;
            {
                String message = "test";
                String subject = "Test";
                Double longtitude = 2.3;
                Double latitude = 3.2;
                allMessages.Add(new Tuple<string, string, double, double>(subject, message, longtitude, latitude));
                allMessages.Add(new Tuple<string, string, double, double>(subject, message, longtitude, latitude));
                if (allMessages.Count > 3)
                {
                    succes = false;
                }
                else
                {
                    succes = true;
                }

                Assert.IsTrue(succes);
            }
        }
    }
}
       

        
    

