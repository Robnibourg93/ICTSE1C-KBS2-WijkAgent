using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using WijkAgentBeta.ContentHandling;

namespace WijkAgentBeta.Tests
{
    [TestClass]
    public class ChattenTests
    {
        [TestMethod]
        public void Chatten_sendMessage_noMessage()
        {
            // arrange
            ChatController chat = new ChatController();
            int from = 1;
            int to = 4;
            string message = null;
            // act
            chat.sendMessage(from, to, message);

            // assert
            
        }

        [TestMethod]
        public void receiveMessage()
        {
            // arrange

            // act

            // assert
        }

        [TestMethod]
        public void setReceived()
        {
            // arrange

            // act

            // assert
        }

        [TestMethod]
        public List<ChatMessage> getChatLog()
        {
            // arrange

            // act

            // assert
            return null;
        }
    }
}
