using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgentBeta.ContentHandling
{
    public class ChatMessage
    {
        /*
          Author: S1079813 - Simon Brink
          Modified by: Simon Brink

          The class ChatMessage is a default class for an message, whenever a person sends a message to another person an chatmessage will be created with the help from this class.
         */
        public int id { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string messageText { get; set; }
        public DateTime sendDate { get; set; }
        public int received { get; set; }

        public ChatMessage(int id, int from, int to, string messageText, DateTime sendDate, int received)
        {
            this.id = id;
            this.from = from;
            this.to = to;
            this.messageText = messageText;
            this.sendDate = sendDate;
            this.received = received;
        }

    }
}
